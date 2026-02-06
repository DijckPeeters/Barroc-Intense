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

            // Event handler toevoegen
            leaseContractTextBox.BeforeTextChanging += LeaseContractTextBox_BeforeTextChanging;
        }

        private void LeaseContractTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            // Verboden tekens
            char[] forbidden = { '+', '/', '*', '=', '%', '^', '&', '$', '#' };

            foreach (char c in args.NewText)
            {
                if (forbidden.Contains(c))
                {
                    args.Cancel = true; // blokkeer deze tekens
                    break;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            categoryComboBox.ItemsSource = categories;

            if (e.Parameter is Product productToEdit)
            {
                editingProduct = productToEdit;

                // Vul de velden met bestaande productwaarden
                productNameTextBox.Text = editingProduct.ProductName;
                leaseContractTextBox.Text = editingProduct.LeaseContract;
                priceTextBox.Text = editingProduct.PricePerKg.ToString("0.00");
                installationCostTextBox.Text = editingProduct.InstallationCost.ToString("0.00");
                stockTextBox.Text = editingProduct.Stock.ToString();
                UsedTextBox.Text = editingProduct.UsedCount.ToString();

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
                //  nieuw product aanmaken met lege ingredientenlijst
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

            // Lijst om foutmeldingen te verzamelen
            var validationMessages = new List<string>();

            // Controleer prijs
            if (!decimal.TryParse(priceTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
                validationMessages.Add("❌ Ongeldige prijs per kg");

            // Controleer installatiekosten
            if (!decimal.TryParse(installationCostTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var installationCost))
                validationMessages.Add("❌ Ongeldige Reparatie kosten");

            // Controleer stock
            if (!int.TryParse(stockTextBox.Text, out var stock))
                validationMessages.Add("❌ Ongeldige voorraad");

            // Controleer gebruikt aantal
            if (!int.TryParse(UsedTextBox.Text, out var usedCount))
                validationMessages.Add("❌ Ongeldig aantal in gebruik");

            // Productnaam controleren
            if (string.IsNullOrWhiteSpace(productNameTextBox.Text))
                validationMessages.Add("❌ Productnaam mag niet leeg zijn");

            // LeaseContract controleren
            if (string.IsNullOrWhiteSpace(leaseContractTextBox.Text))
            {
                validationMessages.Add("❌ Leasecontract mag niet leeg zijn");
            }
            else
            {
                // Verboden tekens in LeaseContract
                string forbiddenChars = "+-*/=^&%$#";
                if (leaseContractTextBox.Text.Any(c => forbiddenChars.Contains(c)))
                    validationMessages.Add("❌ LeaseContract mag geen +, -, *, / of andere symbolen bevatten");
            }

            // Categorie controleren
            if (categoryComboBox.SelectedItem == null)
                validationMessages.Add("❌ Selecteer een categorie");

            // Als er foutmeldingen zijn, tonen en stoppen met opslaan
            if (validationMessages.Any())
            {
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, validationMessages);
                return;
            }

            // Vul productobject
            editingProduct.ProductName = productNameTextBox.Text;
            editingProduct.LeaseContract = leaseContractTextBox.Text;
            editingProduct.Category = categoryComboBox.SelectedItem?.ToString();
            editingProduct.PricePerKg = price;
            editingProduct.InstallationCost = installationCost;
            editingProduct.Stock = stock;
            editingProduct.UsedCount = usedCount;

            if (editingProduct.Category != "Koffieboon")
                editingProduct.Ingredients.Clear();

            // DataAnnotations validatie
            var context = new ValidationContext(editingProduct);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(editingProduct, context, results, true))
            {
                var daMessages = results.Select(r => "❌ " + r.ErrorMessage);
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, daMessages);
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

                db.SaveChanges();

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

                Frame.Navigate(typeof(StockPage), editingProduct.Id);
            }
            catch (Exception ex)
            {
                validationResultsTextBlock.Text = $"❌ Fout bij opslaan: {ex.Message}";
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            //  nieuw ingredient toevoegen aan de ingredientenlijst
            editingProduct.Ingredients ??= new ObservableCollection<Ingredient>();
            editingProduct.Ingredients.Add(new Ingredient { Name = "", AmountInKg = 0.2m });
            ingredientsPanel.Visibility = Visibility.Visible;
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //  toon of verberg ingredientenpaneel op basis van category
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