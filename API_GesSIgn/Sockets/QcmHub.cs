using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using API_GesSIgn.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_GesSIgn.Sockets
{
    public class QcmHub : Hub
    {
        private static ConcurrentDictionary<string, StudentQcm> students = new ConcurrentDictionary<string, StudentQcm>();
        private static QCMDto currentQCM;
        private static bool isQCMRunning = false;
        private static int currentQuestionIndex = 0;
        private static string professorConnectionId = null;
        private readonly IQcmService _qcmService;

        public QcmHub(IQcmService qcmService)
        {
            _qcmService = qcmService;
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Professeur"))
            {
                professorConnectionId = Context.ConnectionId;
                Console.WriteLine("Professor connected: " + Context.User.Identity.Name);
            }
            else
            {
                var student = new StudentQcm { Student_Id = Context.ConnectionId, Name = Context.User.Identity.Name };
                students.TryAdd(Context.ConnectionId, student);
                Console.WriteLine("Student connected: " + student.Name);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.IsInRole("Professeur"))
            {
                professorConnectionId = null;
                Console.WriteLine("Professor disconnected: " + Context.ConnectionId);
            }
            else
            {
                students.TryRemove(Context.ConnectionId, out _);
                Console.WriteLine("Student disconnected: " + Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task StartQCM(int qcmId)
        {
            if (Context.ConnectionId != professorConnectionId)
            {
                await Clients.Caller.SendAsync("Error", "Only the professor can start the QCM.");
                return;
            }

            var qcm = await _qcmService.GetQCMByIdAsync(qcmId);
            if (qcm == null)
            {
                await Clients.Caller.SendAsync("Error", "QCM not found.");
                return;
            }

            currentQCM = qcm;
            currentQuestionIndex = 0;
            isQCMRunning = true;

            await Clients.All.SendAsync("QcmStarted", qcm);

            await RunQCM();
        }

        public async Task SendAnswer(string answer)
        {
            if (students.TryGetValue(Context.ConnectionId, out var student))
            {
                var question = currentQCM.Questions[currentQuestionIndex];
                if (question != null)
                {
                    var correctAnswers = question.CorrectOption.Select(opt => opt.ToString()).ToArray();
                    bool isCorrect = correctAnswers.Contains(answer);
                    if (isCorrect)
                    {
                        student.Score += 10; // Adding 10 points for a correct answer
                    }
                    await Clients.Caller.SendAsync("AnswerFeedback", isCorrect ? "Correct" : "Incorrect");
                }
            }
        }

        private async Task RunQCM()
        {
            while (currentQuestionIndex < currentQCM.Questions.Count)
            {
                if (!isQCMRunning) break;

                var question = currentQCM.Questions[currentQuestionIndex];
                await Clients.All.SendAsync("NewQuestion", question);

                await Task.Delay(20000); // Wait 20 seconds before sending the next question

                await SendRanking();

                await Task.Delay(10000); // Delay 10 seconds for displaying the ranking

                currentQuestionIndex++;
            }

            isQCMRunning = false;
            await Clients.All.SendAsync("QcmEnded");
        }

        private async Task SendRanking()
        {
            var ranking = students.Values.OrderByDescending(s => s.Score).ToList();
            await Clients.All.SendAsync("Ranking", ranking);
        }

        public async Task PauseQCM()
        {
            if (Context.ConnectionId != professorConnectionId)
            {
                await Clients.Caller.SendAsync("Error", "Only the professor can pause the QCM.");
                return;
            }

            isQCMRunning = false;
            await Clients.All.SendAsync("QcmPaused");
        }

        public async Task ResumeQCM()
        {
            if (Context.ConnectionId != professorConnectionId)
            {
                await Clients.Caller.SendAsync("Error", "Only the professor can resume the QCM.");
                return;
            }

            isQCMRunning = true;
            await Clients.All.SendAsync("QcmResumed");
            await RunQCM();
        }
    }
}
