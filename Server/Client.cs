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
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public string Name { get; private set; }
        public bool IsOnline { get; private set; } // Статус пользователя

        // конструктор клиента
        public Client(TcpClient tcpClient, string name)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();

            Name = name; // Инициализируется переданным именем клиента
            IsOnline = true; // По умолчанию пользователь онлайн

            
        
        }

        // метод для обработки клиента
        public async Task StartAsync(Server server)
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

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Message message = JsonSerializer.Deserialize<Message>(json);
                    if (message.Type == MessageType.Authorization)
                    {
                        string authJson = Encoding.UTF8.GetString(message.Data);
                        AuthorizationMassage authMessage = JsonSerializer.Deserialize<AuthorizationMassage>(authJson);

                        // Обрабатываем данные авторизации
                        Console.WriteLine($"Получены данные авторизации: Логин = {authMessage.Login}, Пароль = {authMessage.Password}");
                    }
                    else
                    {
                        Console.WriteLine("Получено сообщение неизвестного типа.");
                    }


                    Console.WriteLine($"{Name} отправил: {message}");

                    // ответ на клиента
                    //string response = $"Сообщение от {Name}: {message}";
                    //byte[] responseData = Encoding.UTF8.GetBytes(response);
                    //await _stream.WriteAsync(responseData, 0, responseData.Length);

                    //foreach (var client in clients.Values)
                    //{
                    //    if (client.Name != Name) // Не отправляем сообщение самому себе
                    //    {
                    //        byte[] responseData = Encoding.UTF8.GetBytes($"{Name}: {message}");
                    //        await client._stream.WriteAsync(responseData, 0, responseData.Length);
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Name}: Ошибка: {ex.Message}");
            }
            finally
            {
                IsOnline = false; // Статус оффлайн
                Console.WriteLine($"{Name} отключен.");
                _tcpClient.Close();
                //clients.TryRemove(Name, out _); // Удаляем клиента из словаря
            }
        }

        internal object GetStream()
        {
            throw new NotImplementedException();
        }
    }
}
