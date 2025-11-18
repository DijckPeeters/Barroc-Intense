using Barroc_Intense.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Services
{
    internal class RoleServices
    {
        // Replace this in-memory list with your AppDbContext queries when ready.
        private readonly List<Employee> _employees = new()
        {
            new Employee { Id = 1, Username = "alice", Role = "Manager", Department = "Manager" },
            new Employee { Id = 2, Username = "bob", Role = "Sales", Department = "Sales" },
            new Employee { Id = 3, Username = "carol", Role = "Finance", Department = "Finance" }
        };

        // Authenticate should validate credentials; here it only finds by username for demo.
        public Employee? AuthenticateByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return _employees.FirstOrDefault(e =>
                e.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public Employee? GetByUsername(string username) => AuthenticateByUsername(username);

        public bool IsInRole(Employee? employee, string role)
            => employee != null && string.Equals(employee.Role, role, StringComparison.OrdinalIgnoreCase);
    }
}

