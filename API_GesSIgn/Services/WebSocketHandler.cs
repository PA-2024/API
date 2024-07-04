using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using API_GesSIgn.Services;

namespace Services
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();
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

                await Receive(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await HandleMessage(socketId, message, webSocket);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        foreach (var room in _rooms.Values)
                        {
                            room.Sockets.TryRemove(socketId, out _);
                        }
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocketHandler", CancellationToken.None);
                    }
                });
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task SendMessage(WebSocket webSocket, string message)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

        private async Task HandleMessage(string socketId, string message, WebSocket webSocket)
        {
            var parts = message.Split(' ');
            if (parts.Length >= 2 && parts[0] == "createRoom")
            {
                var token = parts[1];
                var subjectHourId = parts.Length > 2 ? parts[2] : null;

                if (!string.IsNullOrEmpty(subjectHourId) && ValidateToken(token))
                {
                    var createRoomResult = CreateRoom(subjectHourId, socketId);
                    _rooms[subjectHourId].Sockets[socketId] = webSocket;
                    await SendMessage(webSocket, "Room created.");

                    // Start sending codes to the creator
                    var timer = new Timer(async _ => await SendCodeToCreator(subjectHourId), null, 0, 15000);
                    
                }
                else
                {
                    await SendMessage(webSocket, "Invalid token or subjectHourId.");
                }
            }
            else if (parts.Length == 3 && parts[0] == "validate")
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

        private async Task SendCodeToCreator(string roomId)
        {
            if (_rooms.TryGetValue(roomId, out var room))
            {
                if (room.Sockets.TryGetValue(room.CreatorSocketId, out var creatorSocket))
                {
                    var code = GenerateCode();
                    await SendMessage(creatorSocket, code);
                }
            }
        }

        /// <summary>
        /// Permet de valider un token JWT et regarde si le role est prof
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("VotreCléSécrèteSuperSécuriséeDe32CaractèresOuPlus");

            try
            {
                var tokenValidate =  tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var roleClaim = tokenValidate.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim != null && roleClaim.Value == "Professeur")
                {
                    return true;
                }
                return false; // le token est correct mais le rôle n'est pas Professeur

            }
            catch
            {
                return false;
            }
        }

        public IActionResult CreateRoom(string subjectHourId, string creatorSocketId)
        {
            if (_rooms.ContainsKey(subjectHourId))
            {
                // si room deja presente, update CreatorSocketId
                _rooms[subjectHourId].CreatorSocketId = creatorSocketId;
                return new OkObjectResult("Room already exists, CreatorSocketId updated.");
            }
            else
            {
                // Sinon, création room
                var room = new Room { Id = subjectHourId, CreatorSocketId = creatorSocketId };
                _rooms[subjectHourId] = room;
                return new OkObjectResult("Room created.");
            }
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 30)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
