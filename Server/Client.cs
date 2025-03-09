using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public string Login { get; private set; }
        public bool IsAuthorization { get; private set; } // Авторизован?

        // конструктор клиента
        public Client(TcpClient tcpClient)
        {
            _client = tcpClient;
            _stream = tcpClient.GetStream();
            IsAuthorization = false;

            Console.WriteLine("- новый клиент");
        }

        public async Task Auth(Server server)
        {
            TcpMessage message = GetData();

            if (message.Type == MessageType.Authorization)
            {
                Authorization authMessage = Authorization.ConvertToObject(message.Data);
                if (server.IsUser(authMessage.Login, authMessage.Password))
                {
                    IsAuthorization = true;
                    Login = authMessage.Login;
                    Console.WriteLine("- верный логин и пароль");
                }
            }
            if (IsAuthorization)
            {
                TcpMessage status = AuthorizationStatus.Create("OK");
                byte[] data = TcpMessage.ConvertToByte(status);
                await _stream.WriteAsync(data, 0, data.Length);

                Console.WriteLine("- отправлен OK");
            }
            else
            {
                TcpMessage status = AuthorizationStatus.Create("ERROR");
                byte[] data = TcpMessage.ConvertToByte(status);
                await _stream.WriteAsync(data, 0, data.Length);

                Console.WriteLine("- отправлен ERROR");

                _client.Close();
            }
        }

        private TcpMessage GetData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            bytesRead = _stream.Read(buffer, 0, buffer.Length);
            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            TcpMessage message = JsonSerializer.Deserialize<TcpMessage>(json);
            return message;
        }

        // метод для работы с клиентом
        public async Task Start(Server server)
        {
            Console.WriteLine($"{Login} подключен - {_client.Client.RemoteEndPoint}");

            try
            {
                // отправляем список пользователей в онлайне
                TcpMessage OnlineUsersMessage = OnlineUsers.Create(server.GetOnlineUsers());
                Send(OnlineUsersMessage);

                // основной цикл работы
                while (true)
                {
                    TcpMessage message = GetData();

                    if (message.Type == MessageType.Text)
                    {
                        Text textMessage = Text.ConvertToObject(message.Data);

                        Console.WriteLine($"сообщение: от - {textMessage.From}, кому - {textMessage.To}, {textMessage.Message}");

                        TcpMessage newMessage = Text.Create(textMessage.From, textMessage.To, textMessage.Message);
                        server.SendMessage(textMessage.To, newMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Login} отключен");

                _client.Close();
                server.DeleteClient(this.Login);
            }
        }

        // отправить сообщение
        public void Send(TcpMessage tcpMessage)
        {
            byte[] data = TcpMessage.ConvertToByte(tcpMessage);
            _stream.WriteAsync(data, 0, data.Length);
        }
    }
}
