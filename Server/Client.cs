using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public string Name { get; private set; }

        // конструктор клиента
        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();

            Name = "test";
        }

        // метод для обработки клиента
        public async Task StartAsync()
        {
            Console.WriteLine($"{Name} подключен: {_tcpClient.Client.RemoteEndPoint}");

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (true)
                {
                    // чтение данных от клиента
                    bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // клиент отключился?

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{Name} отправил: {message}");

                    // ответ на клиента
                    //string response = $"Сообщение от {Name}: {message}";
                    //byte[] responseData = Encoding.UTF8.GetBytes(response);
                    //await _stream.WriteAsync(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Name}: Ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"{Name} отключен.");
                _tcpClient.Close();
            }
        }
    }
}
