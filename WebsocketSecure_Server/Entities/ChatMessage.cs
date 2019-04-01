using System;

namespace WebsocketSecure_Server.Entities
{
    public class ChatMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}