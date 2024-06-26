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

        public PresenceHub(MonDbContext context)
        {
            _context = context;
        }

        public async Task JoinRoom(int subjectHourId)
        {
            try
            {
                PresenceHub.WriteToFile("log.txt", $"JoinRoom called with subjectHourId: {subjectHourId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, subjectHourId.ToString());
            }
            catch (Exception ex)
            {
                PresenceHub.WriteToFile("log.txt", $"Error in JoinRoom: {ex.Message}");
            }
            PresenceHub.WriteToFile("log.txt", $"Successfully joined room: {subjectHourId}");
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

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 15)
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
