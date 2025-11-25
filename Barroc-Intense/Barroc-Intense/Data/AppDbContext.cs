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
    internal class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Melding> Meldingen { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<PlanningItem> PlanningItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                ConfigurationManager.ConnectionStrings["BarrocIntense"].ConnectionString,
                ServerVersion.Parse("8.0.30")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================
            //      SEED: MACHINES
            // ==========================
            modelBuilder.Entity<Machine>().HasData(
                new Machine { Id = 1, Naam = "Koffieautomaat A1", Locatie = "Hal 1", Status = "Operationeel" },
                new Machine { Id = 2, Naam = "Koffieautomaat B2", Locatie = "Hal 3", Status = "Storend" },
                new Machine { Id = 3, Naam = "Koffieautomaat C3", Locatie = "Kantine", Status = "Onderhoud" }
            );

            // ==========================
            //         SEED: MELDINGEN
            // ==========================
            modelBuilder.Entity<Melding>().HasData(
                new Melding
                {
                    Id = 1,
                    MachineId = "2",
                    Prioriteit = "Hoog",
                    Afdeling = "Productie",
                    Datum = DateTime.Now.AddDays(-1),
                    Klant = "CoolBlue",
                    Product = "Koffieautomaat",
                    Probleemomschrijving = "Machine lekt water",
                    Status = "Open",
                    IsOpgelost = false
                },
                new Melding
                {
                    Id = 2,
                    MachineId = "1",
                    Prioriteit = "Middel",
                    Afdeling = "Logistiek",
                    Datum = DateTime.Now.AddDays(-4),
                    Klant = "Bol.com",
                    Product = "Koffieautomaat",
                    Probleemomschrijving = "Maalt niet goed",
                    Status = "In behandeling",
                    IsOpgelost = false
                },
             
                 new Melding
                 {
                     Id = 4,
                     MachineId = "3",
                     Prioriteit = "Laag",
                     Afdeling = "Service",
                     Datum = DateTime.Now.AddDays(0),
                     Klant = "NS",
                     Product = "Koffieautomaat",
                     Probleemomschrijving = "Periodieke controle",
                     Status = "Gesloten",
                     IsOpgelost = true
                 }
            );



            // ==========================
            //         SEED: PLANNING
            // ==========================
         
        }
    }
}



