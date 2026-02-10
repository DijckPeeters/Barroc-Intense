using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Barroc_Intense.Data;
using Barroc_Intense.Services;
using Microsoft.EntityFrameworkCore;
using Data = Barroc_Intense.Data;

namespace Barroc_Intense.Pages.Dashboards
{
    public sealed partial class FinanceDashboard : Page
    {
        private readonly AppDbContext _db = new();

        public FinanceDashboard()
        {
            InitializeComponent();
            LoadOffers();
            LoadInvoices();
            UpdateTotalIncome();
        }

        // -------------------------
        // CREATE OFFER
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
                DisplayMemberPath = "ProductName"
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
                    quantityBox
                }
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (productBox.SelectedItem is Data.Product product)
                {
                    var offer = new Data.Offer
                    {
                        OfferNumber = $"OFF-{DateTime.Now.Ticks}",
                        Date = DateTime.Now,
                        CustomerName = companyBox.Text,
                        CustomerAddress = addressBox.Text,
                        CustomerEmail = emailBox.Text,
                        Items = new List<Data.OfferItem>
                        {
                            new Data.OfferItem
                            {
                                Product = product,
                                Quantity = (int)quantityBox.Value
                            }
                        }
                    };

                    _db.Offers.Add(offer);
                    _db.SaveChanges();
                    LoadOffers();
                }
            }
        }

        // -------------------------
        // LOAD OFFERS
        // -------------------------
        private void LoadOffers()
        {
            if (OfferListPanel == null) return;

            OfferListPanel.Children.Clear();

            var offers = _db.Offers
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.Date)
                .ToList();

            foreach (var offer in offers)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 15 };

                row.Children.Add(new TextBlock { Text = offer.CustomerName, Width = 200 });
                row.Children.Add(new TextBlock { Text = offer.OfferNumber, Width = 150 });

                var convertBtn = new Button
                {
                    Content = "Converteer naar factuur",
                    Tag = offer
                };
                convertBtn.Click += ConvertOffer_Click;

                row.Children.Add(convertBtn);
                OfferListPanel.Children.Add(row);
            }
        }

        private void ConvertOffer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Data.Offer offer)
            {
                var invoice = InvoiceService.CreateInvoiceFromOffer(offer);

                _db.Invoices.Add(invoice);
                _db.Offers.Remove(offer);
                _db.SaveChanges();

                LoadOffers();
                LoadInvoices();
                UpdateTotalIncome();
            }
        }

        // -------------------------
        // LOAD INVOICES
        // -------------------------
        private void LoadInvoices()
        {
            if (ContractListPanel == null) return;

            ContractListPanel.Children.Clear();

            var invoices = _db.Invoices
                .Include(i => i.Items)
                .ThenInclude(ii => ii.Product)
                .OrderByDescending(i => i.Date)
                .ToList();

            foreach (var invoice in invoices)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 15 };

                row.Children.Add(new TextBlock { Text = invoice.CustomerName, Width = 200 });
                row.Children.Add(new TextBlock { Text = $"€ {invoice.TotalAmount:0.00}", Width = 120 });

                row.Children.Add(new TextBlock
                {
                    Text = invoice.IsPaid ? "Betaald" : "Openstaand",
                    Foreground = new SolidColorBrush(invoice.IsPaid ? Colors.Green : Colors.Red)
                });

                var payBtn = new Button { Content = "Markeer als betaald", Tag = invoice };
                payBtn.Click += PayInvoice_Click;

                var pdfBtn = new Button { Content = "Maak PDF", Tag = invoice };
                pdfBtn.Click += CreateInvoicePdf_Click;

                row.Children.Add(payBtn);
                row.Children.Add(pdfBtn);

                ContractListPanel.Children.Add(row);
            }
        }

        private void PayInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Data.Invoice invoice)
            {
                invoice.IsPaid = true;
                _db.SaveChanges();
                LoadInvoices();
                UpdateTotalIncome();
            }
        }

        private void CreateInvoicePdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Data.Invoice invoice)
            {
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Invoice-{invoice.InvoiceNumber}.pdf");

                PdfService.CreateInvoicePdf(invoice, path);
            }
        }

        private void UpdateTotalIncome()
        {
            // Perform server-side aggregation using InvoiceItems so EF can translate to SQL.
            var total = _db.InvoiceItems
                .Where(ii => ii.Invoice.IsPaid)
                .Sum(ii => ii.Quantity * ii.Product.PricePerKg + ii.Product.InstallationCost);

            TotalIncomeText.Text = $"Totale inkomsten: € {total:0.00}";
        }
    }
}
