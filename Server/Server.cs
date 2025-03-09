using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.IO;

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
                    TcpClient newConnectClient = await _server.AcceptTcpClientAsync();

                    Client client = new Client(newConnectClient);
                    client.Auth(this);

                    if (client.IsAuthorization)
                    {
                        _clients.TryAdd(client.Login, client); // Добавляем клиента в словарь
                        client.Start(this);
                    }
                }
                catch (Exception ex)
                {
                    if (!_isRunning) break;
                    Console.WriteLine($"Ошибка при подключении клиента: {ex.Message}");
                }
            }
        }

        //
        public void DeleteClient(string login)
        {
            _clients.TryRemove(login, out _);


        }

        // метод остановки сервера
        public void Stop()
        {
            _isRunning = false;
            _server.Stop();
            Console.WriteLine("Сервер остановлен.");
        }

        // проверка логина и пароля
        public bool IsUser(string login, string password)
        {
            if (_users.TryGetValue(login, out User user))
            {
                if (user.Password == password)
                {
                    return true;
                }
            }
            return false;
        }

        // метод отправки сообщения
        public void SendMessage(string to, TcpMessage tcpMessage)
        {
            if (_clients.TryGetValue(to, out Client client))
            {
                client.Send(tcpMessage);
            }
        }

        // получить список всех клиентов
        public List<string> GetOnlineUsers()
        {
            return _clients.Values.Select(client => client.Login).ToList();
        }
    }
}
