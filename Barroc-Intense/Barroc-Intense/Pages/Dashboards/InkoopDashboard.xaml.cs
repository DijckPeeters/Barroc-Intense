using Barroc_Intense.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InkoopDashboard : Page
    {
        private List<Product> _products = new();
        private Dictionary<Product, TextBox> _orderInputs = new();

        // List of invoices for delivery tracking
        private List<Invoice> _invoices = new();

        public InkoopDashboard()
        {
            this.InitializeComponent();
            LoadProducts();
            LoadInvoices();
            GenerateProductRows();
            GenerateInvoiceRows();
        }

        // ----------------------------
        // Load simple starting product list
        // ----------------------------
        private void LoadProducts()
        {
            _products = new List<Product>
            {
                new("Rubber (10 mm)", 0.39, 2),
                new("Rubber (14 mm)", 0.45, 5),
                new("Slang", 4.45, 1),
                new("Voeding (elektra)", 68.69, 10),
                new("Ontkalker", 4.00, 8),
                new("Waterfilter", 299.45, 2),
                new("Reservoir Sensor", 89.99, 6),
                new("Druppelstop", 122.43, 1),
                new("Elektrische pomp", 478.59, 3),
                new("Tandwiel 110mm", 5.45, 12),
                new("Tandwiel 70mm", 5.25, 10),
                new("Maalmotor", 119.20, 2),
                new("Zeef", 28.80, 7),
                new("Reinigingstabletten", 3.45, 1),
                new("Reinigingsborsteltjes", 8.45, 4),
                new("Ontkalkingspijp", 21.70, 2)
            };
        }

        // ----------------------------
        // Load example invoices
        // ----------------------------
        private void LoadInvoices()
        {
            _invoices = new List<Invoice>
            {
                new Invoice("FACT-2025-001", "Maandelijkse koffielevering", false),
                new Invoice("FACT-2025-002", "Nieuwe koffiemachine (Model X)", true),
                new Invoice("FACT-2025-003", "Reinigingsmiddelen pakket", false)
            };
        }

        // ----------------------------
        // Generate product table rows
        // ----------------------------
        private void GenerateProductRows()
        {
            ProductRows.Children.Clear();
            _orderInputs.Clear();

            foreach (var p in _products)
            {
                var row = new Grid();
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Product name
                row.Children.Add(new TextBlock
                {
                    Text = p.Name,
                    VerticalAlignment = VerticalAlignment.Center
                });

                // Price
                var price = new TextBlock
                {
                    Text = $"{p.Price:0.00}",
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(price, 1);
                row.Children.Add(price);

                // Stock
                var stockText = new TextBlock
                {
                    Text = p.Stock.ToString(),
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Low stock warning color
                if (p.Stock < 3)
                {
                    stockText.Foreground = new SolidColorBrush(Colors.Red);
                }

                Grid.SetColumn(stockText, 2);
                row.Children.Add(stockText);

                // Order amount textbox
                var orderBox = new TextBox
                {
                    PlaceholderText = "0",
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 35
                };
                Grid.SetColumn(orderBox, 3);
                row.Children.Add(orderBox);

                _orderInputs[p] = orderBox;
                ProductRows.Children.Add(row);
            }
        }

        // ----------------------------
        // Handle "Place order" button
        // ----------------------------
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            double total = 0;

            foreach (var item in _orderInputs)
            {
                var product = item.Key;
                var textbox = item.Value;

                if (int.TryParse(textbox.Text, out int amount) && amount > 0)
                {
                    total += amount * product.Price;
                    product.Stock += amount; // add stock after ordering
                }
            }

            TotalPriceText.Text = $"Totaal: €{total:0.00}";

            if (total > 5000)
            {
                ApprovalWarning.Text =
                    "Let op! Bestellingen boven €5000 moeten akkoord krijgen van Head Inkoop (John Vrees).";
            }
            else
            {
                ApprovalWarning.Text = "";
            }

            GenerateProductRows();
        }

        // ----------------------------
        // Generate invoice status section
        // ----------------------------
        private void GenerateInvoiceRows()
        {
            InvoicesPanel.Children.Clear();

            foreach (var invoice in _invoices)
            {
                var row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10
                };

                // Invoice info
                row.Children.Add(new TextBlock
                {
                    Text = $"{invoice.Id} - {invoice.Description}",
                    Width = 400
                });

                // Delivered checkbox
                var checkbox = new CheckBox
                {
                    Content = "Geleverd?",
                    IsChecked = invoice.IsDelivered
                };

                checkbox.Checked += (s, e) => invoice.IsDelivered = true;
                checkbox.Unchecked += (s, e) => invoice.IsDelivered = false;

                row.Children.Add(checkbox);

                InvoicesPanel.Children.Add(row);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session.IsLoggedIn = false;
            Session.Username = null;

            Frame.Navigate(typeof(LoginPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }

    // ----------------------------
    // Product model
    // ----------------------------
    public class Product
    {
        public string Name { get; }
        public double Price { get; }
        public int Stock { get; set; }

        public Product(string name, double price, int stock)
        {
            Name = name;
            Price = price;
            Stock = stock;
        }
    }

    // ----------------------------
    // Invoice model
    // ----------------------------
    public class Invoice
    {
        public string Id { get; }
        public string Description { get; }
        public bool IsDelivered { get; set; }

        public Invoice(string id, string description, bool isDelivered)
        {
            Id = id;
            Description = description;
            IsDelivered = isDelivered;
        }
    }
}
