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
        AuthorizationStatus,
        Text,
        OnlineUsers
    }

    // основной класс "Сообщение"
    public class TcpMessage
    {
        public MessageType Type { get; set; }   // тип
        public byte[] Data { get; set; }        // данные

        // конструктор
        public TcpMessage(MessageType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        // создаем сообщение из обьекта (любого типа сообщения)
        public static TcpMessage Create(MessageType type, Object dataObject)
        {
            string json = JsonSerializer.Serialize(dataObject);
            byte[] data = Encoding.UTF8.GetBytes(json);
            return new TcpMessage(type, data);
        }

        // создаем байтовые данные
        public static byte[] ConvertToByte(TcpMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(json);
        }
    }

    // авторизация
    public class Authorization
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public Authorization(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static TcpMessage Create(string login, string password)
        {
            Authorization authMessage = new Authorization(login, password);
            return TcpMessage.Create(MessageType.Authorization, authMessage);
        }

        public static Authorization ConvertToObject(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<Authorization>(json);
        }
    }

    // статус авторизации
    public class AuthorizationStatus
    {
        public string Status { get; set; }
        public AuthorizationStatus(string status)
        {
            Status = status;
        }
        public static TcpMessage Create(string status)
        {
            AuthorizationStatus authStatusMessage = new AuthorizationStatus(status);
            return TcpMessage.Create(MessageType.AuthorizationStatus, authStatusMessage);
        }

        public static AuthorizationStatus ConvertToObject(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<AuthorizationStatus>(json);
        }
    }

    // текстовое сообщение
    public class Text
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public Text(string from, string to, string message)
        {
            From = from;
            To = to;
            Message = message;
        }
        public static TcpMessage Create(string from, string to, string message)
        {
            Text TextMessage = new Text(from, to, message);
            return TcpMessage.Create(MessageType.Text, TextMessage);
        }
        public static Text ConvertToObject(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<Text>(json);
        }
    }

    // юзеры онлайн
    public class OnlineUsers
    {
        public List<string> Users { get; set; }

        public OnlineUsers(List<string> users)
        {
            Users = users;
        }
        public static TcpMessage Create(List<string> users)
        {
            OnlineUsers OnlineUsersMessage = new OnlineUsers(users);
            return TcpMessage.Create(MessageType.OnlineUsers, OnlineUsersMessage);
        }
        public static OnlineUsers ConvertToObject(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<OnlineUsers>(json);
        }

    }
}
