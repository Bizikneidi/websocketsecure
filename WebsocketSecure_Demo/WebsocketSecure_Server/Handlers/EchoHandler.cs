using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using WebsocketSecure_Server.Entities;

namespace WebsocketSecure_Server.Handlers
{
    internal static class ChatHandler
    {
        static ChatHandler()
        {
            User kneidi = new User {Username = "Kneidi", Password = "Admin1234"};
            User richi = new User {Username = "Richi", Password = "Admin1234"};
            User bert = new User {Username = "Bert", Password = "Admin1234"};
            Users = new List<User>
            {
                kneidi,
                richi,
                bert
            };

            ChatMessages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    From = kneidi.Username, To = bert.Username, Content = "Hallo Bert!",
                    Timestamp = DateTime.Now.AddMinutes(-5)
                },
                new ChatMessage
                {
                    From = bert.Username, To = kneidi.Username, Content = "Hallo Kneidi!",
                    Timestamp = DateTime.Now.AddMinutes(-4)
                },
                new ChatMessage
                    {From = kneidi.Username, To = bert.Username, Content = "Wos los?", Timestamp = DateTime.Now},
                new ChatMessage
                {
                    From = richi.Username, To = kneidi.Username, Content = "Hallo.",
                    Timestamp = DateTime.Now.AddMinutes(-10)
                },
                new ChatMessage
                {
                    From = kneidi.Username, To = richi.Username, Content = "Hallo.",
                    Timestamp = DateTime.Now.AddMinutes(-9)
                },
                new ChatMessage
                {
                    From = kneidi.Username, To = richi.Username, Content = "Wos wüsd du?",
                    Timestamp = DateTime.Now.AddMinutes(-8)
                },
                new ChatMessage
                {
                    From = richi.Username, To = kneidi.Username, Content = "Nix, woit da nur am oasch geh.",
                    Timestamp = DateTime.Now.AddMinutes(-3)
                }
            };
        }

        private static readonly Dictionary<string, WebSocket> LoggedInSockets = new Dictionary<string, WebSocket>();

        private static readonly List<User> Users;

        private static readonly List<ChatMessage> ChatMessages;

        public static async Task HandleAsync(WebSocket ws)
        {
            User loggedInUser = null;
            while (true)
            {
                var receiveBuffer = new byte[4096];

                var wsRecResult =
                    await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (wsRecResult.CloseStatus.HasValue)
                {
                    await CloseConnection(ws, wsRecResult.CloseStatusDescription, wsRecResult.CloseStatus.Value,
                        loggedInUser);
                    break;
                }

                var msg = JsonConvert.DeserializeObject<Message>(Encoding.Default.GetString(receiveBuffer));

                switch (msg.Command.ToLower())
                {
                    case "login":
                        User requested = ((JObject) msg.Data).ToObject<User>();
                        if (requested != null && Users.Any(u => u.Equals(requested)) &&
                            !LoggedInSockets.ContainsKey(requested.Username))
                        {
                            LoggedInSockets.Add(requested.Username, ws);
                            loggedInUser = requested;

                            SendUsersAsync(requested);
                        }
                        else if (requested != null && LoggedInSockets.ContainsKey(requested.Username))
                        {
                            await CloseConnection(ws, "User already logged in!", WebSocketCloseStatus.PolicyViolation);
                        }

                        break;
                    case "send_message":
                        if (loggedInUser != null)
                            SendMessageAsync(((JObject) msg.Data).ToObject<ChatMessage>());
                        else
                            await CloseConnection(ws, "User was not logged in!", WebSocketCloseStatus.PolicyViolation);
                        break;
                    case "get_messages_by_user":
                        if (loggedInUser != null)
                            GetMessagesByUser(ws, loggedInUser.Username, (string) msg.Data);
                        else
                            await CloseConnection(ws, "User was not logged in!", WebSocketCloseStatus.PolicyViolation);
                        break;
                }
            }
        }

        private static async Task CloseConnection(WebSocket ws, string message, WebSocketCloseStatus closeStatus,
            User loggedInUser = null)
        {
            await ws.CloseAsync(closeStatus, message,
                CancellationToken.None);

            if (loggedInUser != null)
                LoggedInSockets.Remove(loggedInUser.Username);
        }

        private static async void GetMessagesByUser(WebSocket receiver, string u1, string u2)
        {
            List<ChatMessage> messagesToSend = ChatMessages
                .Where(cm => cm.To == u1 && cm.From == u2 || cm.To == u2 && cm.From == u1)
                .ToList();

            await SendAsync(receiver, messagesToSend);
        }

        private static async void SendUsersAsync(User user)
        {
            await SendAsync(LoggedInSockets[user.Username], Users.Where(u => u.Username != user.Username).Select(u => u.Username));
        }

        private static async void SendMessageAsync(ChatMessage chatMessage)
        {
            ChatMessages.Add(chatMessage);
            if (LoggedInSockets[chatMessage.To] != null)
                await SendAsync(LoggedInSockets[chatMessage.To], chatMessage);
        }

        private static async Task SendAsync(WebSocket receiver, object obj)
        {
            var msg = Encoding.Default.GetBytes(JsonConvert.SerializeObject(obj));
            await receiver.SendAsync(new ArraySegment<byte>(msg, 0, msg.Count(b => b != 0)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Message
    {
        public string Command { get; set; }
        public object Data { get; set; }
    }
}