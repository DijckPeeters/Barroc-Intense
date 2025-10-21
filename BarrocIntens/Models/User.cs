using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }

        public User(string username, string password, string department)
        {
            Username = username;
            Password = password;
            Department = department;
        }
    }
}
