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
        private TcpListener _server;
        private bool _isRunning;

        // конструктор сервера
        public Server()
        {
            _server = new TcpListener(IPAddress.Any, 45014);
            _isRunning = true;
        }

        // метод старта сервака
        public async Task StartAsync()
        {
            // старт сервака
            _server.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            // ждем подключений клиентов
            while (_isRunning)
            {
                try
                {
                    TcpClient tcpClient = await _server.AcceptTcpClientAsync();
                    Client client = new Client(tcpClient);
                    var clientTask = client.StartAsync();
                }
                catch (Exception ex)
                {
                    if (!_isRunning) break;
                    Console.WriteLine($"Ошибка при подключении клиента: {ex.Message}");
                }
            }
        }

        // метод остановки сервера
        public void Stop()
        {
            _isRunning = false;
            _server.Stop();
            Console.WriteLine("Сервер остановлен.");
        }
    }
}
