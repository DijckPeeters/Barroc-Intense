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
using Barroc_Intense.Data;
using Barroc_Intense.Services;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages.Dashboards
{
    public sealed partial class FinanceDashboard : Page
    {
        private readonly AppDbContext _db = new();

        public FinanceDashboard()
        {
            InitializeComponent();
            LoadInvoices();
            UpdateTotalIncome();
        }

        // -------------------------
        // Create Offer
        // -------------------------
        private async void CreateOffer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Nieuwe offerte",
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleren",
                XamlRoot = this.XamlRoot
            };

            var companyBox = new TextBox { PlaceholderText = "Bedrijfsnaam" };
            var addressBox = new TextBox { PlaceholderText = "Adres" };
            var emailBox = new TextBox { PlaceholderText = "E-mailadres" };

            var productBox = new ComboBox
            {
                ItemsSource = _db.Products.ToList(),
                DisplayMemberPath = "ProductName",
                PlaceholderText = "Selecteer een product"
            };

            var quantityBox = new NumberBox { Minimum = 1, Value = 1 };

            dialog.Content = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    companyBox,
                    addressBox,
                    emailBox,
                    productBox,
                    new TextBlock { Text = "Aantal:" },
                    quantityBox
                }
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                CreateOffer(
                    companyBox.Text,
                    addressBox.Text,
                    emailBox.Text,
                    productBox.SelectedItem as Barroc_Intense.Data.Product,
                    (int)quantityBox.Value);
            }
        }

        private void CreateOffer(string company, string address, string email, Barroc_Intense.Data.Product product, int quantity)
        {
            if (product == null) return;

            int nextNumber = _db.Offers.Any()
                ? _db.Offers.Max(o => o.Id) + 1
                : 1;

            var offer = new Offer
            {
                OfferNumber = $"OFF-{nextNumber}",
                Date = DateTime.Now,
                CustomerName = company,
                CustomerAddress = address,
                CustomerEmail = email,
                Items = new List<OfferItem>
                {
                    new OfferItem
                    {
                        ProductId = product.Id,
                        Product = product,
                        Quantity = quantity
                    }
                }
            };

            _db.Offers.Add(offer);
            _db.SaveChanges();
        }

        // -------------------------
        // Load invoices
        // -------------------------
        private void LoadInvoices()
        {
            ContractListPanel.Children.Clear();

            var invoices = _db.Invoices
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Product)
                .OrderByDescending(i => i.Date)
                .ToList();

            foreach (var invoice in invoices)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 15 };

                row.Children.Add(new TextBlock
                {
                    Text = invoice.CustomerName,
                    Width = 200,
                    FontSize = 18
                });

                row.Children.Add(new TextBlock
                {
                    Text = $"€ {invoice.TotalAmount:0.00}",
                    Width = 120,
                    FontSize = 18,
                    TextAlignment = TextAlignment.Right
                });

                row.Children.Add(new TextBlock
                {
                    Text = invoice.IsPaid ? "Betaald" : "Openstaand",
                    Width = 120,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(invoice.IsPaid ? Colors.Green : Colors.Red)
                });

                var payButton = new Button
                {
                    Content = "Markeren als betaald",
                    Tag = invoice
                };
                payButton.Click += PayInvoice_Click;

                var pdfButton = new Button
                {
                    Content = "Maak PDF",
                    Tag = invoice
                };
                pdfButton.Click += CreateInvoicePdf_Click;

                row.Children.Add(payButton);
                row.Children.Add(pdfButton);

                ContractListPanel.Children.Add(row);
            }
        }

        private void PayInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Barroc_Intense.Data.Invoice invoice)
            {
                invoice.IsPaid = true;
                _db.SaveChanges();
                LoadInvoices();
                UpdateTotalIncome();
            }
        }

        private void CreateInvoicePdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Barroc_Intense.Data.Invoice invoice)
            {
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Invoice-{invoice.InvoiceNumber}.pdf");

                PdfService.CreateInvoicePdf(invoice, path);
            }
        }

        private void UpdateTotalIncome()
        {
            var total = _db.InvoiceItems
                .Where(ii => ii.Invoice.IsPaid)
                .Sum(ii => ii.Quantity * ii.Product.PricePerKg + ii.Product.InstallationCost);

            TotalIncomeText.Text = $"Totale inkomsten: € {total:0.00}";
        }
    }
}