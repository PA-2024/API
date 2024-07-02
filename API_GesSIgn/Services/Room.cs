﻿using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace API_GesSIgn.Services
{
    public class Room
    {
        public string Id { get; set; }
        public string CreatorSocketId { get; set; }
        public ConcurrentDictionary<string, WebSocket> Sockets { get; } = new ConcurrentDictionary<string, WebSocket>();
    }

}
