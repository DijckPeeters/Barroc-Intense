using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class ProductPage : Page
    {
        private Product editingProduct = null;

        public ProductPage()
        {
            this.InitializeComponent();
        }

        private readonly string[] categories = { "Automaat", "Koffieboon" };

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Vul de ComboBox
            categoryComboBox.ItemsSource = categories;

            if (e.Parameter is Product productToEdit)
            {
                editingProduct = productToEdit;

                productNameTextBox.Text = editingProduct.ProductName;
                leaseContractTextBox.Text = editingProduct.LeaseContract;
                priceTextBox.Text = editingProduct.PricePerKg.ToString("0.00");
                installationCostTextBox.Text = editingProduct.InstallationCost.ToString("0.00");

                stockTextBox.Text = editingProduct.Stock.ToString();

                // Selecteer de juiste categorie
                if (!string.IsNullOrWhiteSpace(editingProduct.Category))
                {
                    categoryComboBox.SelectedItem = editingProduct.Category;
                }
            }
        }


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Parse prijs als decimal
            if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
            {
                price = 0;
            }
            decimal? installationCost = null;
            if (decimal.TryParse(installationCostTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var tempCost))
            {
                installationCost = tempCost;
            }

            // Parse voorraad als int
            if (!int.TryParse(stockTextBox.Text, out var stock))
            {
                stock = 0;
            }

            if (editingProduct == null)
            {
                var product = new Product
                {
                    ProductName = productNameTextBox.Text,
                    LeaseContract = leaseContractTextBox.Text,
                    Category = categoryComboBox.SelectedItem?.ToString(),
                    PricePerKg = price,
                    InstallationCost = (decimal)installationCost, // ✅ correct
                    Stock = stock
                };

                SaveProduct(product, isNew: true);
            }
            else
            {
                editingProduct.ProductName = productNameTextBox.Text;
                editingProduct.LeaseContract = leaseContractTextBox.Text;
                editingProduct.Category = categoryComboBox.SelectedItem?.ToString();
                editingProduct.PricePerKg = price;
                editingProduct.InstallationCost = (decimal)installationCost; // ✅ correct
                editingProduct.Stock = stock;

                SaveProduct(editingProduct, isNew: false);
            }


        }

        private void SaveProduct(Product product, bool isNew)
        {
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(product, context, results, true))
            {
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            }
            else
            {
                try
                {
                    using var db = new AppDbContext();
                    if (isNew)
                        db.Products.Add(product);
                    else
                        db.Products.Update(product);

                    db.SaveChanges();

                    validationResultsTextBlock.Text = "✅ Product succesvol opgeslagen!";
                    Frame.Navigate(typeof(StockPage), product.Id);
                }
                catch (Exception ex)
                {
                    errorsTextBlock.Text = $"❌ Fout bij opslaan: {ex.Message}";
                }
            }
        }

        private void goToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.GoBack();
            Frame.Navigate(typeof(InkoopDashBoard));

        }
    }
}
