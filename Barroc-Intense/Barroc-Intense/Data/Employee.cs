using Barroc_Intense.Pages.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class Employee
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
    }

    internal enum Department
    {
        Sales,
        Inkoop,
        Maintenance,
        Klantenservice,
        Finance,
        Manager
    }
}

