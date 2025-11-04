using Barroc_Intense.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Barroc_Intense.Data;

namespace Barroc_Intense.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AssignRoleAsync(string username, string role)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignDepartmentAsync(string username, string department)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user != null)
            {
                user.Department = department;
                await _context.SaveChangesAsync();
            }
        }
    }
}
