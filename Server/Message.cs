using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    // типы сообщений между сервером и клиентом
    public enum MessageType
    {
        Authorization,
        TextMessage
    }

    // основной класс "Сообщение"
    public class Message
    {
        public MessageType Type { get; set; }   // тип
        public byte[] Data { get; set; }        // данные

        // конструктор
        public Message(MessageType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        // создаем сообщение из обьекта (любого типа сообщения)
        public static Message Create(MessageType type, Object dataObject)
        {
            string json = JsonSerializer.Serialize(dataObject);
            byte[] data = Encoding.UTF8.GetBytes(json);
            return new Message(type, data);
        }
    }

    // Авторизация
    public class AuthorizationMassage
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public AuthorizationMassage(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static Message Create(string login, string password)
        {
            AuthorizationMassage authMessage = new AuthorizationMassage(login, password);
            return Message.Create(MessageType.Authorization, authMessage);
        }

        public static AuthorizationMassage Convert(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<AuthorizationMassage>(json);
        }
    }

    // Текстовое сообщение
    public class TextMessage
    {
        public string SendFrom { get; set; }
        public string SendTo { get; set; }
        public string Text { get; set; }
        public TextMessage(string sendFrom, string sendTo, string Text)
        {
            SendFrom = sendFrom;
            SendTo = sendTo;
            Text = Text;
        }
    }
}
