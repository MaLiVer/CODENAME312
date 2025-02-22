using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Server
{
    class Server
    {
        public static bool isRunning = true;

        // конструктор сервера
        public Server()
        {
            // запускам сервак в отдельном потоке, чтобы консоль была доступна
            var serverTask = Task.Run(() => RunServerAsync());
        }

        // поток сервака
        static async Task RunServerAsync()
        {
            // старт сервака
            TcpListener server = new TcpListener(IPAddress.Any, 45014);
            server.Start();

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            // ждем подключений клиентов
            while (isRunning)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                var taskClient = RunClientSessionAsync(client);
            }
        }

        static async Task RunClientSessionAsync(TcpClient client)
        {

        }
    }
}
