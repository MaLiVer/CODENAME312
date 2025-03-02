using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace Server
{
    class Server
    {
        private TcpListener _server;
        private bool _isRunning;
        private ConcurrentDictionary<string, Client> _clients; // Словарь для хранения клиентов
        private ConcurrentDictionary<string, User> _users;

        // конструктор сервера
        public Server()
        {
            _server = new TcpListener(IPAddress.Any, 45014);
            _isRunning = true;
            _clients = new ConcurrentDictionary<string, Client>();// Инициализвация словаря
            _users = new ConcurrentDictionary<string, User>();
            
            User user1 = new User("user1", "password1", false);
            User user2 = new User("user2", "password2", true);
            User user3 = new User("admin", "admin123", true);

            // Добавляем пользователей в словарь
            _users.TryAdd(user1.Login, user1);
            _users.TryAdd(user2.Login, user2);
            _users.TryAdd(user3.Login, user3);
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
                    Client client = new Client(tcpClient, "Client" + _clients.Count); // Присваиваем имя клиенту
                    _clients.TryAdd(client.Name, client); // Добавляем клиента в словарь
                    var clientTask = client.StartAsync(this); // Передаем словарь клиенту
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
