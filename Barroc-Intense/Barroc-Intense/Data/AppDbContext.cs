using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Material> Materials { get; set; }



        public DbSet<Melding> Meldingen { get; set; }
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

            SeedProducts(modelBuilder);
            SeedDeliveries(modelBuilder);
            SeedMaterials(modelBuilder); // <- Voeg dit toe

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
                    Stock = 2,
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
                    Stock = 1,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                },
                new Product
                {
                    Id = 7,
                    ProductName = "Espresso Roma",
                    LeaseContract = "",
                    PricePerKg = 21,
                    Stock = 5,
                    Category = "Koffieboon",
                    InstallationCost = 0m
                },
                new Product
                {
                    Id = 8,
                    ProductName = "Red Honey Honduras",
                    LeaseContract = "",
                    PricePerKg = 28,
                    Stock = 3,
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
                    ProductID = 1,
                    ProductName = "Barroc Intens Italian Light",
                    QuantityDelivered = 2,
                    QuantityExpected = 2,
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
                    ProductID = 2,
                    ProductName = "Barroc Intens Italian",
                    QuantityDelivered = 5,
                    QuantityExpected = 5,
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
                    QuantityDelivered = 1,
                    QuantityExpected = 1,
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
                },
                new Delivery
                {
                    DeliveryID = 4,
                    OrderID = 104,
                    ProductID = 4,
                    ProductName = "Barroc Intens Italian Deluxe Special",
                    QuantityDelivered = 2,
                    QuantityExpected = 2,
                    PlannedDeliveryDate = DateTime.Today.AddDays(1),
                    ActualDeliveryDate = null,
                    Status = "Underway",
                    CustomerID = 4,
                    CustomerName = "Klant D",
                    DeliveryAddress = "Straat 4, Stad",
                    CustomerContact = "0654321098",
                    CarrierName = "DHL",
                    DriverName = "Henk de Wit",
                    TrackingNumber = "DHL654321",
                    Notes = ""
                },
                new Delivery
                {
                    DeliveryID = 5,
                    OrderID = 105,
                    ProductID = 5,
                    ProductName = "Espresso Beneficio",
                    QuantityDelivered = 10,
                    QuantityExpected = 10,
                    PlannedDeliveryDate = DateTime.Today.AddDays(2),
                    ActualDeliveryDate = null,
                    Status = "Planned",
                    CustomerID = 5,
                    CustomerName = "Klant E",
                    DeliveryAddress = "Straat 5, Stad",
                    CustomerContact = "0611223344",
                    CarrierName = "PostNL",
                    DriverName = "Piet de Groot",
                    TrackingNumber = "PN987654",
                    Notes = ""
                },
                new Delivery
                {
                    DeliveryID = 6,
                    OrderID = 106,
                    ProductID = 6,
                    ProductName = "Yellow Bourbon Brasil",
                    QuantityDelivered = 1,
                    QuantityExpected = 1,
                    PlannedDeliveryDate = DateTime.Today.AddDays(3),
                    ActualDeliveryDate = null,
                    Status = "Planned",
                    CustomerID = 6,
                    CustomerName = "Klant F",
                    DeliveryAddress = "Straat 6, Stad",
                    CustomerContact = "0622334455",
                    CarrierName = "DHL",
                    DriverName = "Jan Smit",
                    TrackingNumber = "DHL321654",
                    Notes = ""
                },
                new Delivery
                {
                    DeliveryID = 7,
                    OrderID = 107,
                    ProductID = 7,
                    ProductName = "Espresso Roma",
                    QuantityDelivered = 5,
                    QuantityExpected = 5,
                    PlannedDeliveryDate = DateTime.Today.AddDays(4),
                    ActualDeliveryDate = null,
                    Status = "Underway",
                    CustomerID = 7,
                    CustomerName = "Klant G",
                    DeliveryAddress = "Straat 7, Stad",
                    CustomerContact = "0633445566",
                    CarrierName = "FedEx",
                    DriverName = "Kees van Dijk",
                    TrackingNumber = "FDX654987",
                    Notes = ""
                },
                new Delivery
                {
                    DeliveryID = 8,
                    OrderID = 108,
                    ProductID = 8,
                    ProductName = "Red Honey Honduras",
                    QuantityDelivered = 3,
                    QuantityExpected = 3,
                    PlannedDeliveryDate = DateTime.Today.AddDays(5),
                    ActualDeliveryDate = null,
                    Status = "Planned",
                    CustomerID = 8,
                    CustomerName = "Klant H",
                    DeliveryAddress = "Straat 8, Stad",
                    CustomerContact = "0644556677",
                    CarrierName = "DHL",
                    DriverName = "Henk de Vries",
                    TrackingNumber = "DHL852963",
                    Notes = ""
                }
            );

        }

            private void SeedMaterials(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "Rubber (10 mm)", Price = 0.39m },
                new Material { Id = 2, Name = "Rubber (14 mm)", Price = 0.45m },
                new Material { Id = 3, Name = "Slang", Price = 4.45m },
                new Material { Id = 4, Name = "Voeding (elektra)", Price = 68.69m },
                new Material { Id = 5, Name = "Ontkalker", Price = 4.00m },
                new Material { Id = 6, Name = "Waterfilter", Price = 299.45m },
                new Material { Id = 7, Name = "Reservoir sensor", Price = 89.99m },
                new Material { Id = 8, Name = "Druppelstop", Price = 122.43m },
                new Material { Id = 9, Name = "Electrische pomp", Price = 478.59m },
                new Material { Id = 10, Name = "Tandwiel 110mm", Price = 5.45m },
                new Material { Id = 11, Name = "Tandwiel 70mm", Price = 5.25m },
                new Material { Id = 12, Name = "Maalmotor", Price = 119.20m },
                new Material { Id = 13, Name = "Zeef", Price = 28.80m },
                new Material { Id = 14, Name = "Reinigingstabletten", Price = 3.45m },
                new Material { Id = 15, Name = "Reiningsborsteltjes", Price = 8.45m },
                new Material { Id = 16, Name = "Ontkalkingspijp", Price = 21.70m }
            );
        }

        
    }

}

