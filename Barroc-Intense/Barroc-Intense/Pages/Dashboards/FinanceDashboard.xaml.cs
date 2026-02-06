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
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinanceDashboard : Page
    {
        private readonly AppDbContext _db = new();

        public FinanceDashboard()
        {
            this.InitializeComponent();

            LoadInvoices();
            UpdateTotalIncome();
        }

        // --------------------------------------------------
        // Load invoices from database
        // --------------------------------------------------
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
                    Text = $"€ {invoice.TotalAmount:0.00}",
                    Width = 120,
                    FontSize = 18,
                    TextAlignment = TextAlignment.Right
                };

                var statusText = new TextBlock
                {
                    Text = invoice.IsPaid ? "Betaald" : "Openstaand",
                    Width = 120,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(
                        invoice.IsPaid ? Colors.Green : Colors.Red)
                };

                var payButton = new Button
                {
                    Content = "Markeren als betaald",
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
        // Mark invoice as paid (DATABASE)
        // --------------------------------------------------
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
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = Path.Combine(folder, $"Invoice-{invoice.InvoiceNumber}.pdf");

                PdfService.CreateInvoicePdf(invoice, path);

                ContentDialog dialog = new ContentDialog
                {
                    Title = "PDF aangemaakt",
                    Content = $"De PDF is opgeslagen op het bureaublad: \n{path}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                _ = dialog.ShowAsync();
            }
        }

        // --------------------------------------------------
        // Total income calculation (DATABASE)
        // --------------------------------------------------
        private void UpdateTotalIncome()
        {
            // Sum each InvoiceItem's contribution for invoices that are marked as paid.
            var total = _db.InvoiceItems
                .Where(ii => ii.Invoice.IsPaid)
                .Sum(ii => ii.Quantity * ii.Product.PricePerKg + ii.Product.InstallationCost);

            TotalIncomeText.Text = $"Totale inkomsten: € {total:0.00}";
        }
    }
}
