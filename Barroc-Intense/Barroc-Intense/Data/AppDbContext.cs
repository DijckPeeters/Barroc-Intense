using Microsoft.EntityFrameworkCore;

namespace Barroc_Intense.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=;database=Barroc",
                ServerVersion.Parse("8.0.30")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SeedProducts(modelBuilder);
        }

        private void SeedProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                // AUTOMATEN
                new Product
                {
                    Id = 1,
                    ProductName = "Barroc Intens Italian Light",
                    LeaseContract = "499,- excl. btw per maand",
                    PricePerKg = 289,
                    Stock = 5,
                    Category = "Automaat",
                    InstallationCost = 49.99m
                },
                new Product
                {
                    Id = 2,
                    ProductName = "Barroc Intens Italian",
                    LeaseContract = "599,- excl. btw per maand",
                    PricePerKg = 289,
                    Stock = 5,
                    Category = "Automaat",
                    InstallationCost = 59.99m
                },
                new Product
                {
                    Id = 3,
                    ProductName = "Barroc Intens Italian Deluxe",
                    LeaseContract = "799,- excl. btw per maand",
                    PricePerKg = 375,
                    Stock = 3,
                    Category = "Automaat",
                    InstallationCost = 79.99m
                },
                new Product
                {
                    Id = 4,
                    ProductName = "Barroc Intens Italian Deluxe Special",
                    LeaseContract = "999,- excl. btw per maand",
                    PricePerKg = 375,
                    Stock = 2,
                    Category = "Automaat",
                    InstallationCost = 99.99m
                },

                // KOFFIEBONEN
                new Product
                {
                    Id = 5,
                    ProductName = "Espresso Beneficio",
                    LeaseContract = "",
                    PricePerKg = 22,
                    Stock = 50,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                },
                new Product
                {
                    Id = 6,
                    ProductName = "Yellow Bourbon Brasil",
                    LeaseContract = "",
                    PricePerKg = 23,
                    Stock = 50,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                },
                new Product
                {
                    Id = 7,
                    ProductName = "Espresso Roma",
                    LeaseContract = "",
                    PricePerKg = 21,
                    Stock = 50,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                },
                new Product
                {
                    Id = 8,
                    ProductName = "Red Honey Honduras",
                    LeaseContract = "",
                    PricePerKg = 28,
                    Stock = 50,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                }
            );
        }
    }
}
