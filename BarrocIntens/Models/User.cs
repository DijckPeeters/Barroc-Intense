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
        public string Role { get; set; }  // Bijvoorbeeld: "Admin", "Manager", "Medewerker"
        public string Department { get; set; } // Bijvoorbeeld: "IT", "HR", "Finance"

        public User(string username, string role, string department)
        {
            Username = username;
            Role = role;
            Department = department;
        }
    }
}
