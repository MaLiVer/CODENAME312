using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // запускам сервак в отдельном потоке, чтобы консоль была доступна
            Server server = new Server();
            var serverTask = server.StartAsync();

            string? command;
            // цикл для обработки консольных команд
            while (true)
            {
                command = Console.ReadLine();

                if (command?.ToLower() == "stop")
                {
                    // остановка сервера
                    server.Stop();
                    break;
                }
                else
                {
                    Console.WriteLine("Неизвестная команда."); // ntcn
                }
            }
        }
    }
}
