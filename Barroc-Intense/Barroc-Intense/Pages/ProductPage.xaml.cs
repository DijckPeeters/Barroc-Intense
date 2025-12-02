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


        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string actie = editingProduct == null ? "toevoegen" : "aanpassen";

            var dialog = new ContentDialog
            {
                Title = "Bevestiging",
                Content = $"Weet u zeker dat u dit product wilt {actie}?",
                PrimaryButtonText = "Ja",
                CloseButtonText = "Nee",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;



            if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
            {
                validationResultsTextBlock.Text = "❌ Vul een geldig getal in bij 'Prijs'.";
                return;
            }

            string installationInput = installationCostTextBox.Text;

            var match = System.Text.RegularExpressions.Regex.Match(installationInput, @"\d+([.,]\d+)?");

            decimal installationCost;

            if (match.Success)
            {
                // parse matching number
                decimal.TryParse(match.Value.Replace(",", "."), System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out installationCost);
            }
            else
            {
                validationResultsTextBlock.Text = "❌ Vul een geldige prijs in bij 'Reparatiekosten', bv: 100 per maand.";
                return;
            }


            if (!int.TryParse(stockTextBox.Text, out var stock))
            {
                validationResultsTextBlock.Text = "❌ Vul een geldig getal in bij 'Voorraad'.";
                return;
            }



            if (editingProduct == null)
            {
                var product = new Product
                {
                    ProductName = productNameTextBox.Text,
                    LeaseContract = leaseContractTextBox.Text,
                    Category = categoryComboBox.SelectedItem?.ToString(),
                    PricePerKg = price,
                    InstallationCost = (decimal)installationCost,
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
                editingProduct.InstallationCost = (decimal)installationCost;
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
