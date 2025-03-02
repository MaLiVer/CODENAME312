﻿using System;
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
            Console.WriteLine("Подключение к серверу...");

            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 45014);
            Console.WriteLine("Подключено!");

            //while (true)
            //{
                AuthorizationMassage authMessage = new AuthorizationMassage("user1", "password1");
                string authJson = JsonSerializer.Serialize(authMessage);
                byte[] authData = Encoding.UTF8.GetBytes(authJson);
                Message message = new Message(MessageType.Authorization, authData);

                // отправка сообщения серверу
                string json = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(json);
                await client.GetStream().WriteAsync(data, 0, data.Length);
            //}
        }
    }
}
