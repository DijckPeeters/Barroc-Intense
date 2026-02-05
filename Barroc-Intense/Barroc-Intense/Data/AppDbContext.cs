using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
namespace Barroc_Intense.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Melding> Meldingen { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<PlanningItem> PlanningItems { get; set; }
        public DbSet<MaintenanceMaterial> MaintenanceMaterials { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferItem> OfferItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                ConfigurationManager.ConnectionStrings["BarrocIntens"].ConnectionString,
                ServerVersion.Parse("8.0.30")
            );
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ==========================
            // EMPLOYEES
            // ==========================
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Username = "sarah", Password = "1234", Role = "Sales" },
                new Employee { Id = 2, Username = "john", Password = "1234", Role = "Inkoop" },
                new Employee { Id = 3, Username = "emma", Password = "1234", Role = "Finance" },
                new Employee { Id = 4, Username = "mike", Password = "1234", Role = "Maintenance" },
                new Employee { Id = 5, Username = "anna", Password = "1234", Role = "Klantenservice" },
                new Employee { Id = 6, Username = "marc", Password = "1234", Role = "Manager" }
            );
            // ==========================
            // PRODUCTS
            // ==========================
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductName = "Barroc Intens Italian Light", LeaseContract = "499,- excl. btw per maand", PricePerKg = 289, Stock = 2, Category = "Automaat", InstallationCost = 49.99m },
                new Product { Id = 2, ProductName = "Barroc Intens Italian", LeaseContract = "599,- excl. btw per maand", PricePerKg = 289, Stock = 5, Category = "Automaat", InstallationCost = 59.99m },
                new Product { Id = 3, ProductName = "Barroc Intens Italian Deluxe", LeaseContract = "799,- excl. btw per maand", PricePerKg = 375, Stock = 3, Category = "Automaat", InstallationCost = 79.99m },
                new Product { Id = 4, ProductName = "Barroc Intens Italian Deluxe Special", LeaseContract = "999,- excl. btw per maand", PricePerKg = 375, Stock = 2, Category = "Automaat", InstallationCost = 99.99m },
                new Product { Id = 5, ProductName = "Espresso Beneficio", LeaseContract = "", PricePerKg = 22, Stock = 50, Category = "Koffieboon", InstallationCost = 0m },
                new Product { Id = 6, ProductName = "Yellow Bourbon Brasil", LeaseContract = "", PricePerKg = 23, Stock = 1, Category = "Koffieboon", InstallationCost = 0m },
                new Product { Id = 7, ProductName = "Espresso Roma", LeaseContract = "", PricePerKg = 21, Stock = 5, Category = "Koffieboon", InstallationCost = 0m },
                new Product { Id = 8, ProductName = "Red Honey Honduras", LeaseContract = "", PricePerKg = 28, Stock = 3, Category = "Koffieboon", InstallationCost = 0m }
            );
            // ==========================
            // MACHINES
            // ==========================
            modelBuilder.Entity<Machine>().HasData(
                new Machine { Id = 1, Locatie = "Hal 1", Status = "Operationeel" },
                new Machine { Id = 2, Locatie = "Hal 3", Status = "Storend" },
                new Machine { Id = 3, Locatie = "Kantine", Status = "Onderhoud" },
                new Machine { Id = 4, Locatie = "Kantoor", Status = "Operationeel" }
            );
            // ==========================
            // MELDINGEN
            // ==========================
            modelBuilder.Entity<Melding>().HasData(
                new Melding
                {
                    Id = 1,
                    MonteurId = 2,
                    MachineId = 2,
                    Prioriteit = "Hoog",
                    Afdeling = "Productie",
                    Datum = DateTime.Now.AddDays(-1),
                    Klant = "CoolBlue",
                    Product = "Koffieautomaat",
                    Probleemomschrijving = "Machine lekt water",
                    Status = "Open",
                    IsOpgelost = false,
                    IsKeuring = false,
                    Storingscode = "E201",
                    StoringVerholpen = false,
                    Vervolgafspraak = "",
                    KorteBeschrijving = "",
                    GebruikteOnderdelen = "",
                    ChecklistVolledig = false,
                    KeuringGoedgekeurd = false,
                    KeuringOpmerkingen = ""
                },
                new Melding
                {
                    Id = 2,
                    MonteurId = 1,
                    MachineId = 1,
                    Prioriteit = "Middel",
                    Afdeling = "Logistiek",
                    Datum = DateTime.Now.AddDays(-4),
                    Klant = "Bol.com",
                    Product = "Koffieautomaat",
                    Probleemomschrijving = "Maalt niet goed",
                    Status = "Open",
                    IsOpgelost = false,
                    IsKeuring = false,
                    Storingscode = "E201",
                    StoringVerholpen = false,
                    Vervolgafspraak = "",
                    KorteBeschrijving = "",
                    GebruikteOnderdelen = "",
                    ChecklistVolledig = false,
                    KeuringGoedgekeurd = false,
                    KeuringOpmerkingen = ""
                },
                new Melding
                {
                    Id = 3,
                    MonteurId = 2,
                    MachineId = 3,
                    Prioriteit = "Laag",
                    Afdeling = "Maintenance",
                    Datum = DateTime.Now,
                    Klant = "NS",
                    Product = "Koffieautomaat",
                    Probleemomschrijving = "Periodieke controle",
                    Status = "Open",
                    IsOpgelost = true,
                    IsKeuring = true,
                    ChecklistVolledig = true,
                    KeuringGoedgekeurd = true,
                    KeuringOpmerkingen = "",
                    Storingscode = "",
                    StoringVerholpen = true,
                    Vervolgafspraak = "",
                    KorteBeschrijving = "",
                    GebruikteOnderdelen = ""
                }
            );
            // ==========================
            // DELIVERIES
            // ==========================
            modelBuilder.Entity<Delivery>().HasData(
                new Delivery
                {
                    DeliveryID = 1,
                    OrderID = 101,
                    ProductID = 1,
                    ProductName = "Barroc Intens Italian Light",
                    QuantityDelivered = 2,
                    QuantityExpected = 2,
                    PlannedDeliveryDate = DateTime.Today.AddDays(-3),
                    ActualDeliveryDate = DateTime.Today.AddDays(-2),
                    Status = "Delivered",
                    CustomerID = 1,
                    CustomerName = "CoolBlue",
                    DeliveryAddress = "Straat 1, 1000 AB Plaats",
                    CustomerContact = "0612345678",
                    CarrierName = "DHL",
                    DriverName = "Jan de Vries",
                    TrackingNumber = "DHL001",
                    Notes = "Geen bijzonderheden",
                    MachineId = 1
                },
                new Delivery
                {
                    DeliveryID = 2,
                    OrderID = 102,
                    ProductID = 2,
                    ProductName = "Barroc Intens Italian",
                    QuantityDelivered = 1,
                    QuantityExpected = 1,
                    PlannedDeliveryDate = DateTime.Today.AddDays(-2),
                    ActualDeliveryDate = DateTime.Today.AddDays(-1),
                    Status = "Delivered",
                    CustomerID = 2,
                    CustomerName = "Bol.com",
                    DeliveryAddress = "Straat 2, 2000 CD Plaats",
                    CustomerContact = "0698765432",
                    CarrierName = "PostNL",
                    DriverName = "Piet Janssen",
                    TrackingNumber = "PN002",
                    Notes = "Verpakking gecontroleerd",
                    MachineId = 2
                },
                new Delivery
                {
                    DeliveryID = 3,
                    OrderID = 103,
                    ProductID = 3,
                    ProductName = "Barroc Intens Italian Deluxe",
                    QuantityDelivered = 1,
                    QuantityExpected = 1,
                    PlannedDeliveryDate = DateTime.Today.AddDays(-1),
                    ActualDeliveryDate = DateTime.Today,
                    Status = "Delivered",
                    CustomerID = 3,
                    CustomerName = "NS",
                    DeliveryAddress = "Stationsplein 3, 3000 EF Plaats",
                    CustomerContact = "0623456789",
                    CarrierName = "FedEx",
                    DriverName = "Kees de Boer",
                    TrackingNumber = "FDX003",
                    Notes = "Let op handleiding toevoegen",
                    MachineId = 3
                },
                new Delivery
                {
                    DeliveryID = 4,
                    OrderID = 104,
                    ProductID = 5,
                    ProductName = "Espresso Beneficio",
                    QuantityDelivered = 10,
                    QuantityExpected = 10,
                    PlannedDeliveryDate = DateTime.Today.AddDays(-1),
                    ActualDeliveryDate = DateTime.Today,
                    Status = "Delivered",
                    CustomerID = 5,
                    CustomerName = "Klant E",
                    DeliveryAddress = "Hoofdstraat 5, 5000 GH Plaats",
                    CustomerContact = "0611223344",
                    CarrierName = "PostNL",
                    DriverName = "Piet de Groot",
                    TrackingNumber = "PN004",
                    Notes = "Extra koffiebonen meegeleverd",
                    MachineId = 4
                }
            );
            // ==========================
            // MATERIALS
            // ==========================
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
                  new Material { Id = 15, Name = "Reinigingsborsteltjes", Price = 8.45m },
                  new Material { Id = 16, Name = "Ontkalkingspijp", Price = 21.70m }
             );
            // ==========================
            // INGREDIENTS
            // ==========================
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, ProductId = 5, Name = "Arabica Bonen", AmountInKg = 0.5m },
                new Ingredient { Id = 2, ProductId = 5, Name = "Robusta Bonen", AmountInKg = 0.2m },
                new Ingredient { Id = 3, ProductId = 6, Name = "100% Arabica Yellow Bourbon", AmountInKg = 0.5m }
            );
        }
    }
}