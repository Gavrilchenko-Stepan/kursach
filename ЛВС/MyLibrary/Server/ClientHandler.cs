using Messenger.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Messenger.Server
{
    public class ClientHandler
    {
        private TcpClient client;
        private MessengerServer server;
        private DatabaseManager db;
        private NetworkStream stream;
        private BinaryFormatter formatter;
        private bool isConnected;

        public User User { get; private set; }

        public ClientHandler(TcpClient client, MessengerServer server, DatabaseManager db)
        {
            this.client = client;
            this.server = server;
            this.db = db;
            this.stream = client.GetStream();
            this.formatter = new BinaryFormatter();
            this.isConnected = true;
        }

        public void HandleClient()
        {
            try
            {
                while (isConnected && client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        var packet = (NetworkPacket)formatter.Deserialize(stream);
                        ProcessPacket(packet);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                server.Log($"Ошибка обработки клиента: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private void ProcessPacket(NetworkPacket packet)
        {
            switch (packet.Command)
            {
                case NetworkCommand.Login:
                    HandleLogin(packet);
                    break;

                case NetworkCommand.GetChats:
                    HandleGetChats();
                    break;

                case NetworkCommand.GetChatMessages:
                    HandleGetChatMessages(packet);
                    break;

                case NetworkCommand.SendMessage:
                    HandleSendMessage(packet);
                    break;

                case NetworkCommand.CreateExternalChat:
                    HandleCreateExternalChat(packet);
                    break;

                case NetworkCommand.MessagesRead:
                    HandleMessagesRead(packet);
                    break;

                case NetworkCommand.Logout:
                    Disconnect();
                    break;
            }
        }

        private void HandleLogin(NetworkPacket packet)
        {
            var credentials = packet.Data as Dictionary<string, string>;
            if (credentials == null) return;

            string username = credentials["username"];
            string password = credentials["password"];

            User = db.AuthenticateUser(username, password);

            if (User != null)
            {
                db.UpdateUserStatus(User.Id, true);

                SendPacket(new NetworkPacket
                {
                    Command = NetworkCommand.Login,
                    Data = new Dictionary<string, object> { { "success", true }, { "user", User } }
                });

                var statusPacket = new NetworkPacket
                {
                    Command = NetworkCommand.UserStatusChanged,
                    Data = User
                };

                server.BroadcastToDepartment(User.DepartmentId, statusPacket, User.Id);

                server.Log($"Пользователь {User.FullName} ({User.DepartmentName}) вошел в систему");
            }
            else
            {
                SendPacket(new NetworkPacket
                {
                    Command = NetworkCommand.Login,
                    Data = new Dictionary<string, object> { { "success", false }, { "message", "Неверный логин или пароль" } }
                });
            }
        }

        private void HandleGetChats()
        {
            if (User == null) return;

            var chats = db.GetUserChats(User.Id);
            SendPacket(new NetworkPacket
            {
                Command = NetworkCommand.GetChats,
                Data = chats
            });
        }

        private void HandleGetChatMessages(NetworkPacket packet)
        {
            if (User == null) return;

            int chatId = (int)packet.Data;
            var messages = db.GetChatMessages(chatId, User.Id);

            SendPacket(new NetworkPacket
            {
                Command = NetworkCommand.GetChatMessages,
                Data = messages
            });
        }

        private void HandleSendMessage(NetworkPacket packet)
        {
            if (User == null) return;

            var message = packet.Data as Message;
            if (message == null) return;

            message.SenderId = User.Id;
            message.SenderName = User.FullName;
            message.SenderPosition = User.Position;
            message.SenderDepartmentId = User.DepartmentId;
            message.SentAt = DateTime.Now;

            int messageId = db.SaveMessage(message);
            message.Id = messageId;

            var newMessagePacket = new NetworkPacket
            {
                Command = NetworkCommand.NewMessage,
                Data = message
            };

            server.BroadcastToChat(message.ChatId, newMessagePacket, User.Id);
            db.UpdateChatLastMessage(message.ChatId, message.Text, message.SentAt);

            server.Log($"Сообщение от {User.FullName} в чат #{message.ChatId}");
        }

        private void HandleCreateExternalChat(NetworkPacket packet)
        {
            if (User == null) return;

            int otherDepartmentId = (int)packet.Data;

            var existingChat = db.GetExternalChat(User.DepartmentId, otherDepartmentId);

            if (existingChat != null)
            {
                SendPacket(new NetworkPacket
                {
                    Command = NetworkCommand.CreateExternalChat,
                    Data = existingChat
                });
                return;
            }

            var newChat = db.CreateExternalChat(User.DepartmentId, otherDepartmentId);

            var chatPacket = new NetworkPacket
            {
                Command = NetworkCommand.GetChats,
                Data = db.GetUserChats(User.Id)
            };

            server.BroadcastToDepartment(User.DepartmentId, chatPacket);
            server.BroadcastToDepartment(otherDepartmentId, chatPacket);

            SendPacket(new NetworkPacket
            {
                Command = NetworkCommand.CreateExternalChat,
                Data = newChat
            });

            server.Log($"Создан новый чат между отделами {User.DepartmentId} и {otherDepartmentId}");
        }

        private void HandleMessagesRead(NetworkPacket packet)
        {
            if (User == null) return;

            var data = packet.Data as Dictionary<string, int>;
            if (data == null) return;

            int chatId = data["chatId"];
            int lastReadMessageId = data["lastReadMessageId"];

            db.MarkMessagesAsRead(chatId, User.Id, lastReadMessageId);
        }

        public void SendPacket(NetworkPacket packet)
        {
            try
            {
                formatter.Serialize(stream, packet);
            }
            catch (Exception ex)
            {
                server.Log($"Ошибка отправки пакета: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (User != null)
                {
                    db.UpdateUserStatus(User.Id, false);

                    var statusPacket = new NetworkPacket
                    {
                        Command = NetworkCommand.UserStatusChanged,
                        Data = User
                    };

                    server.BroadcastToDepartment(User.DepartmentId, statusPacket, User.Id);

                    server.Log($"Пользователь {User.FullName} вышел");
                }

                stream?.Close();
                client?.Close();
            }
            catch (Exception ex)
            {
                server.Log($"Ошибка при отключении: {ex.Message}");
            }
        }
    }
}
