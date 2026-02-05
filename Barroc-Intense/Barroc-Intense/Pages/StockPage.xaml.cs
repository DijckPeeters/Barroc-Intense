using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class StockPage : Page
    {
        private Product chosenProduct = null;

        public StockPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            //  laad alle producten uit database
            using var db = new AppDbContext();
            var products = db.Products.ToList();

            foreach (var p in products)
            {
                //  check of er deliveries bestaan voor dit product
                p.IsPlanned = db.Deliveries.Any(d => d.ProductID == p.Id);

                //  tel alleen de daadwerkelijk geleverde deliveries
                p.UsedCount = db.Deliveries.Count(d => d.ProductID == p.Id && d.Status == "Delivered");
            }

            productListView.ItemsSource = products;
        }

        private void ShowProductDetails(Product selectedProduct)
        {
            detailsPanel.Visibility = Visibility.Visible;
            placeholderTextBlock.Visibility = Visibility.Collapsed;

            //  toon basisgegevens product
            detailNameTextBlock.Text = selectedProduct.ProductName;
            detailLeaseContractTextBlock.Text = string.IsNullOrWhiteSpace(selectedProduct.LeaseContract) ? "Geen contract" : selectedProduct.LeaseContract;
            detailPriceTextBlock.Text = $"€ {selectedProduct.PricePerKg:0.00}";
            detailCategoryTextBlock.Text = string.IsNullOrWhiteSpace(selectedProduct.Category) ? "Geen categorie" : selectedProduct.Category;
            detailInstallationCostTextBlock.Text = selectedProduct.InstallationCost > 0 ? $"€ {selectedProduct.InstallationCost:0.00} per maand" : "Geen maandelijkse reparatiekosten";
            detailStockTextBlock.Text = selectedProduct.Category == "Koffieboon"
                ? $"{selectedProduct.Stock} kg op voorraad"
                : $"{selectedProduct.Stock} op voorraad";

            //  status gebruikte producten afhankelijk van planning en geleverd aantal
            if (!selectedProduct.IsPlanned)
            {
                UsedTextBlock.Text = "Product niet in gebruik";
                usedProductsButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (selectedProduct.UsedCount == 0)
                {
                    UsedTextBlock.Text = "Nog niet geleverd";
                    usedProductsButton.Visibility = Visibility.Visible;
                }
                else
                {
                    UsedTextBlock.Text = $"{selectedProduct.UsedCount}× in gebruik";
                    usedProductsButton.Visibility = Visibility.Visible;
                }
            }

            //  knoptekst aanpassen op basis van categorie
            materialsListButton.Content = selectedProduct.Category == "Koffieboon"
                ? "Ingrediënten"
                : "Materialenlijst";
        }

        private void productListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Product selectedProduct)
            {
                chosenProduct = selectedProduct;
                ShowProductDetails(selectedProduct);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadProducts();

            //  indien navigatie met productId, toon details direct
            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int productId))
            {
                using var db = new AppDbContext();
                var product = db.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    chosenProduct = product;
                    ShowProductDetails(product);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InkoopDashBoard));
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage));
        }

        private async void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            //  bevestigingsdialog tonen voor verwijderen
            var dialog = new ContentDialog
            {
                Title = "Weet u het zeker?",
                Content = $"Weet u zeker dat u het product \"{chosenProduct.ProductName}\" wilt verwijderen?",
                PrimaryButtonText = "Verwijderen",
                CloseButtonText = "Annuleren",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                using var db = new AppDbContext();
                var productToRemove = db.Products.FirstOrDefault(p => p.Id == chosenProduct.Id);
                if (productToRemove != null)
                {
                    // product verwijderen uit database
                    db.Products.Remove(productToRemove);
                    db.SaveChanges();
                }

                //  herlaad producten en reset UI
                LoadProducts();
                detailsPanel.Visibility = Visibility.Collapsed;
                placeholderTextBlock.Visibility = Visibility.Visible;
                chosenProduct = null;
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            Frame.Navigate(typeof(ProductPage), chosenProduct);
        }

        private void TruckButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                Frame.Navigate(typeof(DeliveryPage), productId);
            }
        }

        private void MaterialsListButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            Frame.Navigate(typeof(MaterialListPage), chosenProduct);
        }

        private void UsedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            Frame.Navigate(typeof(UsedProductPage), chosenProduct.Id);
        }
    }

    //  converter toont waarschuwing als stock laag is (<4)
    public class LowStockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int stock && stock < 4)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}