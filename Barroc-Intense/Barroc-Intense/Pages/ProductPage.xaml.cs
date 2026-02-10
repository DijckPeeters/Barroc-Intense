using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            leaseContractTextBox.BeforeTextChanging += LeaseContractTextBox_BeforeTextChanging;
        }

        private void LeaseContractTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            char[] forbidden = { '+', '/', '*', '=', '%', '^', '&', '$', '#' };
            if (args.NewText.Any(c => forbidden.Contains(c))) args.Cancel = true;
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
                UsedTextBox.Text = editingProduct.UsedCount.ToString();

                if (!string.IsNullOrWhiteSpace(editingProduct.Category))
                {
                    categoryComboBox.SelectedItem = editingProduct.Category;
                    ingredientsBorder.Visibility = editingProduct.Category == "Koffieboon"
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }

                editingProduct.Ingredients ??= new ObservableCollection<Ingredient>();
                ingredientsListControl.ItemsSource = editingProduct.Ingredients;
            }
            else
            {
                editingProduct = new Product
                {
                    Ingredients = new ObservableCollection<Ingredient>()
                };
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            editingProduct.Ingredients ??= new ObservableCollection<Ingredient>();
            editingProduct.Ingredients.Add(new Ingredient { Name = "", AmountInKg = 0.2m });
            ingredientsListControl.ItemsSource = editingProduct.Ingredients;
            ingredientsBorder.Visibility = Visibility.Visible;
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem?.ToString() == "Koffieboon")
            {
                ingredientsBorder.Visibility = Visibility.Visible;
                ingredientsListControl.ItemsSource = editingProduct.Ingredients;
            }
            else
            {
                ingredientsBorder.Visibility = Visibility.Collapsed;
                editingProduct.Ingredients.Clear();
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

            var validationMessages = new List<string>();

            if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
                validationMessages.Add("❌ Ongeldige prijs per kg");

            if (!decimal.TryParse(installationCostTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var installationCost))
                validationMessages.Add("❌ Ongeldige Reparatie kosten");

            if (!int.TryParse(stockTextBox.Text, out var stock))
                validationMessages.Add("❌ Ongeldige voorraad");

            if (!int.TryParse(UsedTextBox.Text, out var usedCount))
                validationMessages.Add("❌ Ongeldig aantal in gebruik");

            if (string.IsNullOrWhiteSpace(productNameTextBox.Text))
                validationMessages.Add("❌ Productnaam mag niet leeg zijn");

            if (string.IsNullOrWhiteSpace(leaseContractTextBox.Text))
                validationMessages.Add("❌ Leasecontract mag niet leeg zijn");

            if (categoryComboBox.SelectedItem == null)
                validationMessages.Add("❌ Selecteer een categorie");

            if (validationMessages.Any())
            {
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, validationMessages);
                return;
            }

            // Update productgegevens
            editingProduct.ProductName = productNameTextBox.Text;
            editingProduct.LeaseContract = leaseContractTextBox.Text;
            editingProduct.Category = categoryComboBox.SelectedItem.ToString();
            editingProduct.PricePerKg = price;
            editingProduct.InstallationCost = installationCost;
            editingProduct.Stock = stock;
            editingProduct.UsedCount = usedCount;

            // Alleen koffiebonen mogen ingrediënten hebben
            if (editingProduct.Category != "Koffieboon")
                editingProduct.Ingredients.Clear();

            try
            {
                using var db = new AppDbContext();

                bool isNieuwProduct = editingProduct.Id == 0;
                if (isNieuwProduct)
                {
                    db.Products.Add(editingProduct);
                    db.SaveChanges(); // Product krijgt nu een Id
                }
                else
                {
                    db.Products.Update(editingProduct);
                    db.SaveChanges();
                }

                // Ingrediënten correct koppelen
                foreach (var ingredient in editingProduct.Ingredients)
                {
                    ingredient.ProductId = editingProduct.Id;

                    if (ingredient.Id == 0)
                        db.Ingredients.Add(ingredient);
                    else
                        db.Ingredients.Update(ingredient);
                }
                db.SaveChanges();

                // Optioneel: Delivery automatisch toevoegen als UsedCount > 0
                if (isNieuwProduct && usedCount > 0)
                {
                    for (int i = 0; i < usedCount; i++)
                    {
                        var delivery = new Delivery
                        {
                            ProductID = editingProduct.Id,
                            ProductName = editingProduct.ProductName,
                            QuantityDelivered = 0,
                            QuantityExpected = 1,
                            PlannedDeliveryDate = DateTime.Today,
                            Status = "Not planned"
                        };
                        db.Deliveries.Add(delivery);
                    }
                    db.SaveChanges();
                }

                // Terug naar StockPage
                Frame.Navigate(typeof(StockPage), editingProduct.Id);
            }
            catch (Exception ex)
            {
                validationResultsTextBlock.Text = $"❌ Fout bij opslaan: {ex.Message}";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InkoopDashBoard));
        }
    }
}