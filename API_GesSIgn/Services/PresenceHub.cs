using API_GesSIgn.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PresenceHub : Hub
    {
        private static ConcurrentDictionary<int, string> SubjectHourCodes = new ConcurrentDictionary<int, string>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PresenceHub(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task JoinRoom(int subjectHourId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, subjectHourId.ToString());
        }

        public async Task SendCode(int subjectHourId)
        {
            var code = GenerateRandomCode(15);
            SubjectHourCodes[subjectHourId] = code;
            await Clients.Group(subjectHourId.ToString()).SendAsync("ReceiveCode", code);
        }

        public async Task<bool> ValidatePresence(int subjectHourId, string code, int studentId)
        {
            if (SubjectHourCodes.TryGetValue(subjectHourId, out var validCode) && validCode == code)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MonDbContext>();

                    // recherche de la présence
                    var presence = await context.Presences
                        .FirstOrDefaultAsync(p => p.Presence_Student_Id == studentId && p.Presence_SubjectsHour_Id == subjectHourId);

                    if (presence != null)
                    {
                        presence.Presence_Is = true;
                        presence.Presence_ScanDate = DateTime.Now;

                        await context.SaveChangesAsync();
                    }
                    else
                    {                      
                        presence = new Presence
                        {
                            Presence_Student_Id = studentId,
                            Presence_SubjectsHour_Id = subjectHourId,
                            Presence_Is = true,
                            Presence_ScanDate = DateTime.Now
                        };

                        context.Presences.Add(presence);
                        await context.SaveChangesAsync();
                    }
                }

                return true;
            }

            return false;
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}