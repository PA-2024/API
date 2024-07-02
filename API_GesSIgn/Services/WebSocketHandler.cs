using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API_GesSIgn.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly IServiceProvider _serviceProvider;

        public WebSocketHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string socketId = Guid.NewGuid().ToString();
                _sockets[socketId] = webSocket;

                await SendInitialCode(webSocket);
                var timer = new Timer(async _ => await SendCode(webSocket), null, 0, 15000);

                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await HandleMessage(socketId, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _sockets.TryRemove(socketId, out _);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketHandler", CancellationToken.None);
                    }
                });

                timer.Dispose();
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task SendInitialCode(WebSocket webSocket)
        {
            string code = GenerateCode();
            await SendMessage(webSocket, code);
        }

        private async Task SendCode(WebSocket webSocket)
        {
            string code = GenerateCode();
            await SendMessage(webSocket, code);
        }

        private async Task SendMessage(WebSocket webSocket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        private async Task HandleMessage(string socketId, string message)
        {
            var parts = message.Split(' ');
            if (parts.Length == 3 && parts[0] == "validate")
            {
                int subjectHourId = int.Parse(parts[1]);
                int studentId = int.Parse(parts[2]);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MonDbContext>();

                    var presence = context.Presences
                        .FirstOrDefault(p => p.Presence_Student_Id == studentId && p.Presence_SubjectsHour_Id == subjectHourId);

                    if (presence != null)
                    {
                        presence.Presence_Is = true;
                        presence.Presence_ScanDate = DateTime.UtcNow;
                        presence.Presence_ScanInfo = message;
                        context.Presences.Update(presence);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 15)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
