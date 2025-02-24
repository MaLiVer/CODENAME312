using System;
using System.Net.Sockets;
using System.Text;

namespace TestClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Подключение к серверу...");

            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 45014);
            Console.WriteLine("Подключено!");

            while (true)
            {
                Console.Write("Введите сообщение: "); // 3454
                string message = Console.ReadLine(); // 34gfgfg

                // отправка сообщения серверу
                byte[] data = Encoding.UTF8.GetBytes(message);
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
        }
    }
}
