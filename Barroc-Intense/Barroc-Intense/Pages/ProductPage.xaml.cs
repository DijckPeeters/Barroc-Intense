using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class ProductPage : Page
    {
        private Product editingProduct = null;
        private readonly string[] categories = { "Automaat", "Koffieboon" };

        public ProductPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            categoryComboBox.ItemsSource = categories;

            if (e.Parameter is Product productToEdit)
            {
                editingProduct = productToEdit;

                productNameTextBox.Text = editingProduct.ProductName;
                leaseContractTextBox.Text = editingProduct.LeaseContract;
                priceTextBox.Text = editingProduct.PricePerKg.ToString("0.00");
                installationCostTextBox.Text = editingProduct.InstallationCost.ToString("0.00");
                stockTextBox.Text = editingProduct.Stock.ToString();

                if (!string.IsNullOrWhiteSpace(editingProduct.Category))
                {
                    categoryComboBox.SelectedItem = editingProduct.Category;
                    ingredientsPanel.Visibility = editingProduct.Category == "Koffieboon"
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }

                if (editingProduct.Ingredients == null)
                    editingProduct.Ingredients = new ObservableCollection<Ingredient>();

                ingredientsListControl.ItemsSource = editingProduct.Ingredients;
                categoryComboBox_SelectionChanged(null, null);
            }
            else
            {
                editingProduct = new Product
                {
                    Ingredients = new ObservableCollection<Ingredient>()
                };
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string actie = editingProduct.Id == 0 ? "toevoegen" : "aanpassen";

            var dialog = new ContentDialog
            {
                Title = "Bevestiging",
                Content = $"Weet u zeker dat u dit product wilt {actie}?",
                PrimaryButtonText = "Ja",
                CloseButtonText = "Nee",
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return;

            // VALIDATIE
            if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price) ||
                !decimal.TryParse(installationCostTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var installationCost) ||
                !int.TryParse(stockTextBox.Text, out var stock))
            {
                validationResultsTextBlock.Text = "❌ Ongeldige invoer.";
                return;
            }

            editingProduct.ProductName = productNameTextBox.Text;
            editingProduct.LeaseContract = leaseContractTextBox.Text;
            editingProduct.Category = categoryComboBox.SelectedItem?.ToString();
            editingProduct.PricePerKg = price;
            editingProduct.InstallationCost = installationCost;
            editingProduct.Stock = stock;

            if (editingProduct.Category != "Koffieboon")
                editingProduct.Ingredients.Clear();

            var context = new ValidationContext(editingProduct);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(editingProduct, context, results, true))
            {
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                return;
            }

            try
            {
                using var db = new AppDbContext();

                bool isNieuwProduct = editingProduct.Id == 0;

                if (isNieuwProduct)
                    db.Products.Add(editingProduct);
                else
                    db.Products.Update(editingProduct);

                db.SaveChanges(); // Id wordt hier aangemaakt

                // ===============================
                // 🔹 AUTOMATISCHE DELIVERY
                // ===============================
                if (isNieuwProduct)
                {
                    var delivery = new Delivery
                    {
                        ProductID = editingProduct.Id,
                        ProductName = editingProduct.ProductName,
                        QuantityDelivered = 0,
                        QuantityExpected = 0,
                        PlannedDeliveryDate = DateTime.Today,
                        Status = "Not planned"
                    };

                    db.Deliveries.Add(delivery);
                    db.SaveChanges();
                }
                // ===============================

                Frame.Navigate(typeof(StockPage), editingProduct.Id);
            }
            catch (Exception ex)
            {
                validationResultsTextBlock.Text = $"❌ Fout: {ex.Message}";
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            editingProduct.Ingredients ??= new ObservableCollection<Ingredient>();
            editingProduct.Ingredients.Add(new Ingredient { Name = "", AmountInKg = 0.2m });
            ingredientsPanel.Visibility = Visibility.Visible;
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem?.ToString() == "Koffieboon")
            {
                ingredientsPanel.Visibility = Visibility.Visible;
                ingredientsListControl.ItemsSource = editingProduct?.Ingredients;
            }
            else
            {
                ingredientsPanel.Visibility = Visibility.Collapsed;
                editingProduct?.Ingredients.Clear();
                ingredientsListControl.ItemsSource = null;
            }
        }

        private void goToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InkoopDashBoard));
        }
    }
}
