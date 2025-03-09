using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Server;

namespace TestClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // получаем параметры логин и пароль
            if (args.Length < 2)
            {
                Console.WriteLine("Не получены параметры логин и пароль");
                Console.ReadLine();
                return;
            }
            string login = args[0];
            string password = args[1];

            Console.WriteLine("Подключение к серверу...");

            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 45014);
            Console.WriteLine("Подключено!");

            // отправляем авторизацию
            Authorization authMessage = new Authorization(login, password);
            string authJson = JsonSerializer.Serialize(authMessage);
            byte[] authData = Encoding.UTF8.GetBytes(authJson);
            TcpMessage message = new TcpMessage(MessageType.Authorization, authData);
            string json = JsonSerializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await client.GetStream().WriteAsync(data, 0, data.Length);

            // основной цикл
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                // чтение данных от клиента
                bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // клиент отключился?

                string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                TcpMessage resultMmessage = JsonSerializer.Deserialize<TcpMessage>(json);
                if (message.Type == MessageType.Authorization)
                {
                    
                }
                else
                {
                    Console.WriteLine("Получено сообщение неизвестного типа.");
                }
            }
        }
    }
}
