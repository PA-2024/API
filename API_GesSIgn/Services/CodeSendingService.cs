using Microsoft.AspNetCore.SignalR;
using Services;

namespace API_GesSIgn.Services
{
    /// <summary>
    /// envoi de code pour le qr code
    /// </summary>
    public class CodeSendingService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IHubContext<PresenceHub> _hubContext;

        public CodeSendingService(IHubContext<PresenceHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendCode, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return Task.CompletedTask;
        }

        private void SendCode(object state)
        {
            var activeSubjectHourIds = new List<int>(); // Replace with actual logic
            foreach (var subjectHourId in activeSubjectHourIds)
            {
                _hubContext.Clients.Group(subjectHourId.ToString()).SendAsync("ReceiveCode", GenerateRandomCode(15));
            }
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
