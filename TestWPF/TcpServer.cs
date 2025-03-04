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

namespace TestWPF
{
    public class TcpServer
    {
        private TcpClient? _server;
        private NetworkStream? _stream;

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
                        Task task = Task.Run(() => ReceiveMessagesAsync());
                        break;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            // авторизация
            Server.Message authMessage = AuthorizationMassage.Create(login, password);
            string json = JsonSerializer.Serialize(authMessage);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(data, 0, data.Length);


        }

        // метод постоянного получения новых сообщений
        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}
