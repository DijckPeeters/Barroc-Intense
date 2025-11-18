using Microsoft.EntityFrameworkCore;
using System;

namespace Barroc_Intense.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Delivery> Deliveries { get; set; } 


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
            SeedDeliveries(modelBuilder); 

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

        private void SeedDeliveries(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Delivery>().HasData(
                new Delivery
                {
                    DeliveryID = 1,
                    OrderID = 101,
                    ProductID = 2,
                    ProductName = "Barroc Intens Italian",
                    QuantityDelivered = 5,
                    QuantityExpected = 5,
                    PlannedDeliveryDate = DateTime.Today.AddDays(1),
                    ActualDeliveryDate = null,
                    Status = "Underway",
                    CustomerID = 1,
                    CustomerName = "Klant A",
                    DeliveryAddress = "Straat 1, Stad",
                    CustomerContact = "0612345678",
                    CarrierName = "DHL",
                    DriverName = "Jan de Vries",
                    TrackingNumber = "DHL123456",
                    Notes = "Bel bij aankomst"
                },
                new Delivery
                {
                    DeliveryID = 2,
                    OrderID = 102,
                    ProductID = 5,
                    ProductName = "Espresso Beneficio",
                    QuantityDelivered = 10,
                    QuantityExpected = 10,
                    PlannedDeliveryDate = DateTime.Today.AddDays(2),
                    ActualDeliveryDate = null,
                    Status = "Planned",
                    CustomerID = 2,
                    CustomerName = "Klant B",
                    DeliveryAddress = "Straat 2, Stad",
                    CustomerContact = "0698765432",
                    CarrierName = "PostNL",
                    DriverName = "Piet Janssen",
                    TrackingNumber = "PN654321",
                    Notes = ""
                },
                new Delivery
                {
                    DeliveryID = 3,
                    OrderID = 103,
                    ProductID = 3,
                    ProductName = "Barroc Intens Italian Deluxe",
                    QuantityDelivered = 2,
                    QuantityExpected = 2,
                    PlannedDeliveryDate = DateTime.Today,
                    ActualDeliveryDate = DateTime.Today,
                    Status = "Delivered",
                    CustomerID = 3,
                    CustomerName = "Klant C",
                    DeliveryAddress = "Straat 3, Stad",
                    CustomerContact = "0623456789",
                    CarrierName = "FedEx",
                    DriverName = "Kees de Boer",
                    TrackingNumber = "FDX789012",
                    Notes = "Laat bij receptie achter"
                }
            );
        }
    }
}