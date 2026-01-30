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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinanceDashboard : Page
    {
        private List<Barroc_Intense.Data.Invoice> _invoices = new();
        private List<Barroc_Intense.Data.Product> _products = new();

        public FinanceDashboard()
        {
            this.InitializeComponent();

            LoadProducts();
            LoadDemoInvoice();
            DisplayInvoices();
            UpdateTotalIncome();
        }

        // --------------------------------------------------
        // Demo products (normally from database / inkoop)
        // --------------------------------------------------
        private void LoadProducts()
        {
            _products = new List<Barroc_Intense.Data.Product>
            {
                new Barroc_Intense.Data.Product
                {
                    Id = 1,
                    ProductName = "Koffiebonen Premium",
                    Stock = 100,
                    PricePerKg = 18.50m,
                    InstallationCost = 0
                },
                new Barroc_Intense.Data.Product
                {
                    Id = 2,
                    ProductName = "Espresso Automaat",
                    Stock = 5,
                    PricePerKg = 0,
                    InstallationCost = 750
                }
            };
        }

        // --------------------------------------------------
        // Demo: Offer > Invoice > Stock update
        // --------------------------------------------------
        private void LoadDemoInvoice()
        {
            var offer = new Barroc_Intense.Data.Offer
            {
                OfferNumber = "OFF-001",
                Date = DateTime.Now,
                CustomerName = "Bedrijf A",
                CustomerAddress = "Industrieweg 10, Breda",
                Items = new List<Barroc_Intense.Data.OfferItem>
                {
                    new Barroc_Intense.Data.OfferItem
                    {
                        Product = _products[0],
                        Quantity = 10
                    },
                    new Barroc_Intense.Data.OfferItem
                    {
                        Product = _products[1],
                        Quantity = 1
                    }
                }
            };

            // Convert offer to invoice (also updates stock!)
            var invoice = InvoiceService.CreateInvoiceFromOffer(offer);
            _invoices.Add(invoice);
        }

        // --------------------------------------------------
        // UI: show invoices
        // --------------------------------------------------
        private void DisplayInvoices()
        {
            ContractListPanel.Children.Clear();

            foreach (var invoice in _invoices)
            {
                var row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 15
                };

                var nameText = new TextBlock
                {
                    Text = invoice.CustomerName,
                    Width = 200,
                    FontSize = 18
                };

                var amountText = new TextBlock
                {
                    Text = $"€{invoice.TotalAmount:0.00}",
                    Width = 120,
                    FontSize = 18
                };

                var statusText = new TextBlock
                {
                    Text = invoice.IsPaid ? "Betaald" : "Openstaand",
                    Width = 120,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(
                        invoice.IsPaid ? Colors.Green : Colors.Red
                    )
                };

                var payButton = new Button
                {
                    Content = "Markeer als betaald",
                    Width = 180,
                    Height = 40,
                    Tag = invoice
                };
                payButton.Click += PayInvoice_Click;

                var pdfButton = new Button
                {
                    Content = "Maak PDF",
                    Width = 140,
                    Height = 40,
                    Tag = invoice
                };
                pdfButton.Click += CreateInvoicePdf_Click;

                row.Children.Add(nameText);
                row.Children.Add(amountText);
                row.Children.Add(statusText);
                row.Children.Add(payButton);
                row.Children.Add(pdfButton);

                ContractListPanel.Children.Add(row);
            }
        }

        // --------------------------------------------------
        // Mark invoice as paid
        // --------------------------------------------------
        private void PayInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Barroc_Intense.Data.Invoice invoice)
            {
                invoice.IsPaid = true;
                DisplayInvoices();
                UpdateTotalIncome();
            }
        }

        // --------------------------------------------------
        // Create PDF from invoice
        // --------------------------------------------------
        private void CreateInvoicePdf_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Barroc_Intense.Data.Invoice invoice)
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = Path.Combine(folder, $"{invoice.InvoiceNumber}.pdf");

                PdfService.CreateInvoicePdf(invoice, path);

                ContentDialog dialog = new ContentDialog
                {
                    Title = "PDF aangemaakt",
                    Content = $"De factuur is opgeslagen op:\n{path}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                _ = dialog.ShowAsync();
            }
        }

        // --------------------------------------------------
        // Total income calculation
        // --------------------------------------------------
        private void UpdateTotalIncome()
        {
            var total = _invoices
                .Where(i => i.IsPaid)
                .Sum(i => i.TotalAmount);

            TotalIncomeText.Text = $"Totale inkomsten: €{total:0.00}";
        }
    }
}
