﻿using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using API_GesSIgn.Services;
using API_GesSIgn.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API_GesSIgn.Services
{
    public class QCMWebSocketHandler
    {
        private readonly ConcurrentDictionary<string, CurrentQCM> _qcmSessions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public QCMWebSocketHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _qcmSessions = new ConcurrentDictionary<string, CurrentQCM>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string qcmId = context.Request.Path.ToString().Trim('/');
                await HandleClient(webSocket, qcmId);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task HandleClient(WebSocket webSocket, string qcmId)
        {
            byte[] buffer = new byte[1024];
            Console.WriteLine($"QCM {_qcmSessions.Count} session avant");
            Console.WriteLine(qcmId);
            // Ensure the QCM session is created and added to the dictionary
            var qcm = _qcmSessions.GetOrAdd(qcmId, id => new CurrentQCM
            {
                Id = id,
                Questions = new List<QuestionDto>(),
                Students = new List<StudentQcm>(),
                Professor = null
            });
            Console.WriteLine($"QCM {_qcmSessions.Count} session en cours.");

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
                    Console.WriteLine($"Received message: {message}");  // Log received message
                    await HandleMessage(webSocket, qcmId, message);
                }
            }
        }

        private async Task HandleMessage(WebSocket webSocket, string session_qcmId, string message)
        {
            CurrentQCM qcm = _qcmSessions[session_qcmId];
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var qcmService = scope.ServiceProvider.GetRequiredService<IQcmService>();

                dynamic parsedMessage = JsonConvert.DeserializeObject(message);
                string action = parsedMessage.action;

                if (action == "JOIN_PROFESSOR")
                {
                    string token = parsedMessage.token;
                    string professorName = parsedMessage.professorName;

                    if (WebSocketHandler.ValidateToken(token))
                    {
                        if (qcm.Professor != null)
                        {
                            qcm.Professor.WebSocket = webSocket;
                            Console.WriteLine($"Professor {qcm.Professor.Name} reconnected to QCM {qcm.Id}");
                        }
                        else
                        {
                            qcm.Professor = new Professor(professorName, webSocket);
                            Console.WriteLine($"Professor {professorName} joined QCM {qcm.Id}");
                        }
                    }
                    else // Invalid token
                    {
                        var errorMessage = new { action = "ERROR", message = "Invalid token." };
                        await SendMessage(webSocket, errorMessage); // Send error message to the professor
                        Console.WriteLine(JsonConvert.SerializeObject(errorMessage));  // Log message
                    }
                }
                else if (action == "JOIN_STUDENT")
                {
                    if (qcm.Professor == null)
                    {
                        var errorMessage = new { action = "ERROR", message = "Professor has not joined the QCM yet." };
                        await SendMessage(webSocket, errorMessage); // Send error message to the student
                        Console.WriteLine(JsonConvert.SerializeObject(errorMessage));  // Log message
                        return;
                    }

                    string studentId = parsedMessage.studentId;
                    string studentName = parsedMessage.studentName;
                    var findStudent = qcm.Students.Find(s => s.Student_Id == studentId);

                    if (findStudent != null)
                    {
                        findStudent.webSocket = webSocket;
                        Console.WriteLine($"Student {findStudent.Name} (ID: {findStudent.Student_Id}) reconnected to QCM {qcm.Id}");
                    }
                    else
                    {
                        var student = new StudentQcm(studentId, studentName);
                        student.webSocket = webSocket; // Assign the WebSocket to the student
                        qcm.Students.Add(student);
                        Console.WriteLine($"Student {studentName} (ID: {studentId}) joined QCM {qcm.Id}");
                    }

                    // Notify the professor
                    await NotifyProfessor(qcm);
                }
                else if (action == "ANSWER")
                {
                    string studentId = parsedMessage.studentId;
                    int answer = parsedMessage.answer;

                    var student = qcm.Students.Find(s => s.Student_Id == studentId);
                    if (student != null)
                    {
                        // Handle student answer
                        var question = qcm.Questions[qcm.CurrentQuestionIndex];
                        Console.WriteLine($"Corect reponse is : " + question.CorrectOption[0]);
                        if (question != null && question.CorrectOption.Contains(answer))
                        {
                            student.Score += 10;
                        }
                        var feedback = new { result = question.CorrectOption.Contains(answer) ? "Correct" : "Incorrect" };
                        await SendMessage(webSocket, feedback);
                    }
                }
                else if (action == "START")
                {
                    int qcm_Id = parsedMessage.qcmId;
                    await StartQCM(qcm_Id, qcmService, session_qcmId);
                }
                else if (action == "PAUSE")
                {
                    qcm.IsRunning = false;
                }
                _qcmSessions[session_qcmId] = qcm;
            }
        }

        private async Task StartQCM(int qcmId, IQcmService qcmService, string session_qcmId)
        {
            CurrentQCM qcm = _qcmSessions[session_qcmId];
            var qcmDto = await qcmService.GetQCMByIdAsync(qcmId);
            if (qcmDto == null)
            {
                Console.WriteLine("QCM not found.");

                // Notify the professor about the error
                if (qcm.Professor != null && qcm.Professor.WebSocket.State == WebSocketState.Open)
                {
                    var errorMessage = new { action = "ERROR", message = "QCM not found." };
                    await SendMessage(qcm.Professor.WebSocket, errorMessage);
                } else
                {
                    qcm.IsRunning = false;
                }

                return;
            }

            qcm.Title = qcmDto.Title;
            qcm.Questions = qcmDto.Questions;

            qcm.IsRunning = true;
            qcm.CurrentQuestionIndex = 0;

            // Notify the professor that the QCM is starting
            if (qcm.Professor != null && qcm.Professor.WebSocket.State == WebSocketState.Open)
            {
                var startMessage = new { action = "INFO", message = "QCM is starting." };
                await SendMessage(qcm.Professor.WebSocket, startMessage);
                Console.WriteLine("Sent start message to professor.");  // Log message
            }
            Console.WriteLine("Nombre d'étudiants: " + qcm.Students.Count);
            await RunQCM(qcm);
        }

        private async Task RunQCM(CurrentQCM qcm)
        {
            while (qcm.CurrentQuestionIndex < qcm.Questions.Count && qcm.IsRunning)
            {
                var question = qcm.Questions[qcm.CurrentQuestionIndex];
                var listOption = question.Options.Select(s => new { id = s.Id, Text = s.Text }).ToList();
                var questionMessage = new { action = "QUESTION", id = question.Id, text = question.Text, options = question.Options };

                // Broadcast the question to all students and professor
                await BroadcastMessage(qcm, questionMessage);
                Console.WriteLine("Sent question to all clients.");  // Log message

                await Task.Delay(20000); // Wait 20 seconds for answers

                await SendRanking(qcm);

                await Task.Delay(10000); // Wait 10 seconds to display the ranking

                qcm.CurrentQuestionIndex++;
            }
            var endMessage = new { action = "END", message = "End of the questions" };
            BroadcastMessage(qcm, endMessage).Wait();
            qcm.IsRunning = false;
        }

        private async Task SendRanking(CurrentQCM qcm)
        {
            qcm.Students.Sort((x, y) => y.Score.CompareTo(x.Score));
            var rankingMessage = new
            {
                action = "RANKING",
                ranking = qcm.Students.Select(s => new { name = s.Name, score = s.Score }).ToList()
            };

            await BroadcastMessage(qcm, rankingMessage);
            Console.WriteLine("Sent ranking to all clients.");  // Log message
        }

        /// <summary>
        /// Envoie un message à tous les étudiants et au professeur 
        /// </summary>
        /// <param name="qcm"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task BroadcastMessage(CurrentQCM qcm, object message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            Console.WriteLine(qcm.Students.Count);
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
                Console.WriteLine("Sent message to professor.");  // Log message
            }
        }

        private async Task SendMessage(WebSocket webSocket, object message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }


        private async Task NotifyProfessor(CurrentQCM qcm)
        {
            if (qcm.Professor != null && qcm.Professor.WebSocket.State == WebSocketState.Open)
            {
                var students = qcm.Students.Select(s => new { id = s.Student_Id, name = s.Name }).ToList();
                var message = new { action = "STUDENT_LIST", students };
                await SendMessage(qcm.Professor.WebSocket, message);
                Console.WriteLine("Sent student list to professor.");  // Log message
            }
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