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
    public class TcpServer
    {
        private TcpClient? _server;
        private NetworkStream? _stream;

        public bool IsAuthorization { get; private set; }

        public event Action<List<string>> ClientsUpdated;

        // метод подключения к серверу
        public async Task ConnectToServerAsync(string login, string password)
        {
            _server = new TcpClient();
            
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
                    ClientsUpdated?.Invoke(onlineUsersMessage.Users);

                }

                if (message.Type == MessageType.Text)
                {
                    Text textMessage = Text.ConvertToObject(message.Data);
                }
            }
        }
    }
}
