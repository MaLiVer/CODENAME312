using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum MessageType
    {
        Authorization
        
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
}
