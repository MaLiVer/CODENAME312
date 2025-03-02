using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        public User(string login, string password, bool isAdmin) 
        {
            Login = login;
            Password = password;
            IsAdmin = isAdmin;
        }
    }
}
