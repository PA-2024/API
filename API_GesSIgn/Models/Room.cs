using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace API_GesSIgn.Models
{
    public class Room
    {
        public string Id { get; set; }
        public string CreatorSocketId { get; set; }

        public string code { get; set; }

        public int StudentCount { get; set; }

        public ConcurrentDictionary<string, WebSocket> Sockets { get; } = new ConcurrentDictionary<string, WebSocket>();
    }

}
