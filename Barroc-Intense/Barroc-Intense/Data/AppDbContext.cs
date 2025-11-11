using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class AppDbContext :DbContext
    {
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=;database=Barrroc",
                ServerVersion.Parse("8.0.30")
            );
        }

        public static class MeldingSeeder
        {
            public static List<Melding> Seed()
            {
                return new List<Melding>
            {
                new Melding
                {
                    Id = 1,
                    Afdeling = "Maintenance",
                    Datum = DateTime.Now.AddDays(-1),
                    Klant = "Bakkerij Jansen",
                    Product = "Koffiemachine Deluxe 3000",
                    Probleemomschrijving = "Machine lekt water bij het reservoir.",
                    Status = "Open"
                },
                new Melding
                {
                    Id = 2,
                    Afdeling = "Finance",
                    Datum = DateTime.Now.AddDays(-3),
                    Klant = "Hotel de Ster",
                    Product = "Factuur #3021",
                    Probleemomschrijving = "Betaling dubbel uitgevoerd.",
                    Status = "In behandeling"
                },
                new Melding
                {
                    Id = 3,
                    Afdeling = "Sales",
                    Datum = DateTime.Now.AddDays(-2),
                    Klant = "Café 't Pleintje",
                    Product = "Nieuwe bestelling",
                    Probleemomschrijving = "Onjuiste prijs in offerte.",
                    Status = "Opgelost"
                },
                new Melding
                {
                    Id = 4,
                    Afdeling = "Maintenance",
                    Datum = DateTime.Now.AddDays(-5),
                    Klant = "Koffiecorner Breda",
                    Product = "EspressoMaster 500",
                    Probleemomschrijving = "Temperatuursensor defect.",
                    Status = "In behandeling"
                }
            };
            }
        }
    }

}

