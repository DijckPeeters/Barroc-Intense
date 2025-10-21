using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrocIntens.Models;

namespace BarrocIntens.Services
{
    public class UserService
    {
        private readonly List<User> _users = new()
        {
            new User("admin", "1234", "Sales"),
            new User("sarah", "abcd", "Finance"),
            new User("john", "pass", "")
        };

        public User? Login(string username, string password)
        {
            return _users.FirstOrDefault(u =>
            u.Username == username &&
            u.Password == password);
        }
    }
}
