using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum MessageType
    {
        Authorization,
        TextMessage
    }

    public class Message
    {
        // Тип сообщения
        public MessageType Type { get; set; }

        // Байтовые данные
        public byte[] Data { get; set; }

        // Конструктор для удобного создания объекта
        public Message(MessageType type, byte[] data)
        {
            Type = type;
            Data = data;
        }
    }
    public class AuthorizationMassage
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public AuthorizationMassage(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }

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
