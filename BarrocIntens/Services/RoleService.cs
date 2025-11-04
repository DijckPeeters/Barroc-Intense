using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrocIntens.Data;
using BarrocIntens.Models;

namespace BarrocIntens.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
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
