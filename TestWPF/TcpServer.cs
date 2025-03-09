using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server;
using System.Windows;

namespace TestWPF
{
    public class ClientChats
    {
        public string Login { get; set; }
        public string ChatHistory { get; set; }
    }

    public class TcpServer
    {
        private TcpClient? _server;
        private NetworkStream? _stream;
        private string _login;

        public List<ClientChats> _clientChats;

        public bool IsAuthorization { get; private set; }

        public event Action<List<ClientChats>> ClientsUpdated;

        // метод подключения к серверу
        public async Task ConnectToServerAsync(string login, string password)
        {
            _login = login;
            _server = new TcpClient();
            _clientChats = new List<ClientChats>();
            
            // цикл подключения к серверу
            while (!_server.Connected)
            {
                try
                {
                    await _server.ConnectAsync("127.0.0.1", 45014);
                    if (_server.Connected)
                    {
                        _stream = _server.GetStream();
                        break;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            // 1. отправляем запрос авторизации
            TcpMessage authMessage = Authorization.Create(login, password);
            byte[] data = TcpMessage.ConvertToByte(authMessage);
            await _stream.WriteAsync(data, 0, data.Length);

            // 2. принять результат
            TcpMessage message = GetData();
            if (message.Type == MessageType.AuthorizationStatus)
            {
                AuthorizationStatus authStatusMessage = AuthorizationStatus.ConvertToObject(message.Data);
                if (authStatusMessage.Status == "OK")
                {
                    IsAuthorization = true;
                }
            }

            // запускаем отдельный процесс для получения сообщений
            if (IsAuthorization)
            {
                Task task = Task.Run(() => ReceiveMessagesAsync());
            }
            
        }

        // вспомогательный метод. Получить данные
        private TcpMessage GetData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            bytesRead = _stream.Read(buffer, 0, buffer.Length);
            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            TcpMessage message = JsonSerializer.Deserialize<TcpMessage>(json);
            return message;
        }

        // метод постоянного получения новых сообщений
        private async Task ReceiveMessagesAsync()
        {
            while (true)
            {
                TcpMessage message = GetData();

                if(message.Type == MessageType.OnlineUsers)
                {
                    OnlineUsers onlineUsersMessage = OnlineUsers.ConvertToObject(message.Data);
                    UpdateClientChats(onlineUsersMessage);
                }

                if (message.Type == MessageType.Text)
                {
                    Text textMessage = Text.ConvertToObject(message.Data);

                    foreach (var client in _clientChats)
                    {
                        if (client.Login == textMessage.From)
                        {
                            client.ChatHistory += $"{textMessage.From}: {message}\n";
                            ClientsUpdated(_clientChats);
                        }
                    }
                }
            }
        }

        // отправить сообщение
        public void Send(string to, string text)
        {
            TcpMessage status = Text.Create(_login, to, text);
            byte[] data = TcpMessage.ConvertToByte(status);
            _stream.WriteAsync(data, 0, data.Length);
        }

        // обновление списка чатов клиентов
        public void UpdateClientChats(OnlineUsers onlineUsersMessage)
        {
            foreach (var user in onlineUsersMessage.Users)
            {
                var existingChat = _clientChats.FirstOrDefault(chat => chat.Login == user);

                if (existingChat == null)
                {
                    // Если записи нет, добавляем новую
                    _clientChats.Add(new ClientChats
                    {
                        Login = user,
                        ChatHistory = string.Empty
                    });
                }
                else
                {
                }
            }
            _clientChats.RemoveAll(chat => !onlineUsersMessage.Users.Contains(chat.Login));

            // вызываем экшон
            ClientsUpdated(_clientChats);
        }
    }
}
