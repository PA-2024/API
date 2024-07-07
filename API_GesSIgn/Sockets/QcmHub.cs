using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using API_GesSIgn.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API_GesSIgn.Services
{
    public class QCMWebSocketHandler
    {
        private readonly HttpListener _listener;
        private readonly ConcurrentDictionary<string, CurrentQCM> _qcmSessions;
        private readonly IQcmService _qcmService;

        public QCMWebSocketHandler(string uriPrefix, IQcmService qcmService)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uriPrefix);
            _qcmSessions = new ConcurrentDictionary<string, CurrentQCM>();
            _qcmService = qcmService;
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("WebSocket server started...");
            Task.Run(() => AcceptConnections());
        }

        private async Task AcceptConnections()
        {
            while (true)
            {
                var context = await _listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    Task.Run(() => HandleClient(wsContext.WebSocket, context.Request.Url.AbsolutePath.Trim('/')));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClient(WebSocket webSocket, string qcmId)
        {
            byte[] buffer = new byte[1024];

            // Join the appropriate QCM session
            if (!_qcmSessions.TryGetValue(qcmId, out CurrentQCM qcm))
            {
                qcm = new CurrentQCM { Id = qcmId, Questions = new List<QuestionDto>(), Students = new List<StudentQcm>(), Professor = null };
                _qcmSessions[qcmId] = qcm;
            }

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessage(webSocket, qcm, message);
                }
            }
        }

        private async Task HandleMessage(WebSocket webSocket, CurrentQCM qcm, string message)
        {
            // Handle incoming messages from clients (students/professor)
            if (message.StartsWith("JOIN_STUDENT:"))
            {
                var parts = message.Substring(13).Split('|');
                var studentId = parts[0];
                var studentName = parts[1];
                var student = new StudentQcm(studentId, studentName);
                student.webSocket = webSocket; // Assign the WebSocket to the student
                qcm.Students.Add(student);
                Console.WriteLine($"Student {studentName} (ID: {studentId}) joined QCM {qcm.Id}");
            }
            else if (message.StartsWith("JOIN_PROFESSOR:"))
            {
                var professorName = message.Substring(15);
                qcm.Professor = new Professor(professorName, webSocket);
                Console.WriteLine($"Professor {professorName} joined QCM {qcm.Id}");
            }
            else if (message.StartsWith("ANSWER:"))
            {
                var parts = message.Substring(7).Split('|');
                var studentId = parts[0];
                var answer = int.Parse(parts[1]);

                var student = qcm.Students.Find(s => s.Student_Id == studentId);
                if (student != null)
                {
                    // Handle student answer
                    var question = qcm.Questions[qcm.CurrentQuestionIndex];
                    if (question != null && question.CorrectOption.Contains(answer))
                    {
                        student.Score += 10;
                    }
                    var feedback = question.CorrectOption.Contains(answer) ? "Correct" : "Incorrect";
                    await SendMessage(webSocket, feedback);
                }
            }
            else if (message.StartsWith("START:"))
            {
                var qcmId = int.Parse(message.Substring(6));
                await StartQCM(qcmId, qcm);
            }
            else if (message == "PAUSE")
            {
                qcm.IsRunning = false;
            }
        }

        private async Task StartQCM(int qcmId, CurrentQCM qcm)
        {
            var qcmDto = await _qcmService.GetQCMByIdAsync(qcmId);
            if (qcmDto == null)
            {
                Console.WriteLine("QCM not found.");
                return;
            }

            qcm.Title = qcmDto.Title;
            qcm.Questions = qcmDto.Questions;
            qcm.Students = qcmDto.Students;

            qcm.IsRunning = true;
            qcm.CurrentQuestionIndex = 0;
            await RunQCM(qcm);
        }

        private async Task RunQCM(CurrentQCM qcm)
        {
            while (qcm.CurrentQuestionIndex < qcm.Questions.Count && qcm.IsRunning)
            {
                var question = qcm.Questions[qcm.CurrentQuestionIndex];
                var questionMessage = $"{question.Id}|{question.Text}|{string.Join(",", question.Options)}";

                // Broadcast the question to all students and professor
                await BroadcastMessage(qcm, questionMessage);

                await Task.Delay(20000); // Wait 20 seconds for answers

                await SendRanking(qcm);

                await Task.Delay(10000); // Wait 10 seconds to display the ranking

                qcm.CurrentQuestionIndex++;
            }

            qcm.IsRunning = false;
        }

        private async Task SendRanking(CurrentQCM qcm)
        {
            qcm.Students.Sort((x, y) => y.Score.CompareTo(x.Score));
            var rankingMessage = "Ranking:\n";
            foreach (var student in qcm.Students)
            {
                rankingMessage += $"{student.Name}: {student.Score} points\n";
            }

            await BroadcastMessage(qcm, rankingMessage);
        }

        private async Task BroadcastMessage(CurrentQCM qcm, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            foreach (var student in qcm.Students)
            {
                var studentWebSocket = student.webSocket;
                if (studentWebSocket != null && studentWebSocket.State == WebSocketState.Open)
                {
                    await studentWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            var professorWebSocket = qcm.Professor?.WebSocket;
            if (professorWebSocket != null && professorWebSocket.State == WebSocketState.Open)
            {
                await professorWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task SendMessage(WebSocket webSocket, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public class CurrentQCM
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public List<StudentQcm> Students { get; set; }
        public Professor Professor { get; set; }
        public bool IsRunning { get; set; }
        public int CurrentQuestionIndex { get; set; }

        public CurrentQCM()
        {
            Students = new List<StudentQcm>();
        }
    }

    public class Professor
    {
        public string Name { get; set; }
        public WebSocket WebSocket { get; set; }

        public Professor(string name, WebSocket webSocket)
        {
            Name = name;
            WebSocket = webSocket;
        }
    }
}
