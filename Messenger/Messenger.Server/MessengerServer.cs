using Messenger.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Server
{
    public class MessengerServer
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private DatabaseManager db;
        private bool isRunning = false;
        private readonly object clientsLock = new object();

        public MessengerServer()
        {
            db = new DatabaseManager();
        }

        public void Start()
        {
            try
            {
                if (!db.TestConnection())
                {
                    Console.WriteLine("[ОШИБКА] Не удалось подключиться к базе данных!");
                    return;
                }

                int port = 8888;
                tcpListener = new TcpListener(IPAddress.Any, port);
                listenerThread = new Thread(ListenForClients);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                isRunning = true;
                Log("Сервер запущен на порту " + port);
                Log("Ожидание подключений...");
            }
            catch (Exception ex)
            {
                Log($"Ошибка запуска: {ex.Message}");
            }
        }

        public void Stop()
        {
            isRunning = false;

            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    client.Disconnect();
                }
                clients.Clear();
            }

            tcpListener?.Stop();
            Log("Сервер остановлен");
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (isRunning)
            {
                try
                {
                    var tcpClient = tcpListener.AcceptTcpClient();
                    var handler = new ClientHandler(tcpClient, this, db);

                    lock (clientsLock)
                    {
                        clients.Add(handler);
                    }

                    var clientThread = new Thread(handler.HandleClient);
                    clientThread.IsBackground = true;
                    clientThread.Start();

                    Log($"Новое подключение. Всего клиентов: {clients.Count}");
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        Log($"Ошибка приема клиента: {ex.Message}");
                }
            }
        }

        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public void BroadcastToDepartment(int departmentId, NetworkPacket packet, int excludeClientId = -1)
        {
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    if (client.User != null && client.User.DepartmentId == departmentId && client.User.Id != excludeClientId)
                    {
                        client.SendPacket(packet);
                    }
                }
            }
        }

        public void BroadcastToChat(int chatId, NetworkPacket packet, int excludeClientId = -1)
        {
            lock (clientsLock)
            {
                foreach (var client in clients)
                {
                    if (client.User != null && client.User.Id != excludeClientId)
                    {
                        if (db.UserHasAccessToChat(client.User.Id, chatId))
                        {
                            client.SendPacket(packet);
                        }
                    }
                }
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (clientsLock)
            {
                clients.Remove(client);
                Log($"Клиент отключен. Осталось: {clients.Count}");
            }
        }
    }
}
