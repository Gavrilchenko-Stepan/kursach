using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Messenger Server - Завод 'МАРС'";
            var server = new MessengerServer();
            server.Start();

            Console.WriteLine("\nНажмите 'q' для остановки сервера...");
            while (Console.ReadKey().Key != ConsoleKey.Q) { }

            server.Stop();
        }
    }
}
