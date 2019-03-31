using Newtonsoft.Json;
using System;
using System.Net.WebSockets;

namespace WebsocketSecure_Client
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();

            using (WebSocket ws = new WebSocket("ws://localhost:5000/chat/" + username))
            {
                ws.Connect();

                ws.OnMessage += (sender, e) => { Console.WriteLine("Message: " + e.Data); };

                while (true)
                {
                    Console.Write("> ");
                    string msg = Console.ReadLine();
                    Console.Write("To: ");
                    string to = Console.ReadLine();

                    if (msg != null && msg.ToLower() == "exit")
                    {
                        break;
                    }

                    ws.Send(JsonConvert.SerializeObject(new Message {From = username, To = to, MessageText = msg}));
                }
            }
        }
    }

    class Message
    {
        public string From { get; set; }
        public string MessageText { get; set; }
        public string To { get; set; }
    }
}