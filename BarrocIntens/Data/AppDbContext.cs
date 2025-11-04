using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrocIntens.Models;

namespace BarrocIntens.Data
{
    internal class AppDbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;user=c_sharp_dev;password=im@punchyouintheface;database=csd_hotelapp",
                ServerVersion.Parse("8.0.30")
            );
        }
    }
}
