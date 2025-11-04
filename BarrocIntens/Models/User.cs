using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
    }
}
