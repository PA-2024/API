using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;


namespace Services
{
    public class PresenceHub : Hub
    {
        private readonly MonDbContext _context;
        private static readonly Random random = new Random();
        private static Dictionary<string, Timer> teacherTimers = new Dictionary<string, Timer>();

        //
        public PresenceHub(MonDbContext context)
        {
            _context = context;
        }

        public async Task JoinRoom(int subjectHourId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, subjectHourId.ToString());

            await SendCode(subjectHourId);

            if (teacherTimers.ContainsKey(connectionId))
            {
                teacherTimers[connectionId].Change(0, 15000);
            }
            else
            {
                var timer = new Timer(async _ => await SendCode(subjectHourId), null, 0, 15000);
                teacherTimers[connectionId] = timer;
            }
         }

        public async Task SendCode(int subjectHourId)
        {
            string code = GenerateCode();
            await Clients.Group(subjectHourId.ToString()).SendAsync("ReceiveCode", code);
            PresenceHub.WriteToFile("log.txt", $"SendCode called with code: {code}");
        }

        public async Task ValidatePresence(int subjectHourId, string code, int studentId)
        {
            var presence = _context.Presences
                .FirstOrDefault(p => p.Presence_Student_Id == studentId && p.Presence_SubjectsHour_Id == subjectHourId);

            if (presence != null)
            {
                presence.Presence_Is = true;
                presence.Presence_ScanDate = DateTime.UtcNow;
                presence.Presence_ScanInfo = code;
                _context.Presences.Update(presence);
                await _context.SaveChangesAsync();
            }
        }

         public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            if (teacherTimers.ContainsKey(connectionId))
            {
                teacherTimers[connectionId].Dispose();
                teacherTimers.Remove(connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 30)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void WriteToFile(string fileName, string content)
        {
            // Définir le chemin du fichier
            string filePath = Path.Combine(@"C:\tmp", fileName);

            try
            {
                // Vérifier si le répertoire existe, sinon le créer
                if (!Directory.Exists(@"C:\tmp"))
                {
                    Directory.CreateDirectory(@"C:\tmp");
                }

                // Écrire le contenu dans le fichier
                //File.WriteAllLines(filePath, content);
                Console.WriteLine("Le fichier a été écrit avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
        }
    }


}
