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
            using var db = new AppDbContext();
            var products = db.Products.ToList();

            //  Bereken hoeveel keer een product geleverd is
            foreach (var p in products)
            {
                p.UsedCount = db.Deliveries
                    .Count(d => d.ProductID == p.Id && d.Status == "Delivered");
            }

            productListView.ItemsSource = products;
        }

        private void ShowProductDetails(Product selectedProduct)
        {
            detailsPanel.Visibility = Visibility.Visible;
            placeholderTextBlock.Visibility = Visibility.Collapsed;

            detailNameTextBlock.Text = selectedProduct.ProductName;
            detailLeaseContractTextBlock.Text =
                string.IsNullOrWhiteSpace(selectedProduct.LeaseContract) ? "Geen contract" : selectedProduct.LeaseContract;
            detailPriceTextBlock.Text = $"€ {selectedProduct.PricePerKg:0.00}";
            detailCategoryTextBlock.Text =
                string.IsNullOrWhiteSpace(selectedProduct.Category) ? "Geen categorie" : selectedProduct.Category;
            detailInstallationCostTextBlock.Text =
                selectedProduct.InstallationCost > 0 ? $"€ {selectedProduct.InstallationCost:0.00} per maand" : "Geen maandelijkse reparatiekosten";

            detailStockTextBlock.Text = selectedProduct.Category == "Koffieboon"
                ? $"{selectedProduct.Stock} kg op voorraad"
                : $"{selectedProduct.Stock} op voorraad";

            UsedTextBlock.Text = $"{selectedProduct.UsedCount}× in gebruik";

            
            materialsListButton.Content = selectedProduct.Category == "Koffieboon"
                ? "Ingrediënten"   
                : "Materialenlijst"; 

            // Gebruikte producten altijd zichtbaar
            usedProductsButton.Visibility = Visibility.Visible;
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

            using var db = new AppDbContext();

            // ?? Check of product nog gebruikt wordt in leveringen
            bool hasDeliveries = db.Deliveries.Any(d => d.ProductID == chosenProduct.Id);

            if (hasDeliveries)
            {
                var warningDialog = new ContentDialog
                {
                    Title = "Product kan niet worden verwijderd",
                    Content = "Dit product wordt nog gebruikt in één of meerdere leveringen en kan daarom niet worden verwijderd.",
                    CloseButtonText = "Ok",
                    XamlRoot = this.XamlRoot
                };

                await warningDialog.ShowAsync();
                return;
            }

            // ? Bevestiging vragen
            var confirmDialog = new ContentDialog
            {
                Title = "Weet u het zeker?",
                Content = $"Weet u zeker dat u het product \"{chosenProduct.ProductName}\" wilt verwijderen?",
                PrimaryButtonText = "Verwijderen",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            // ?? Product verwijderen
            var productToRemove = db.Products.FirstOrDefault(p => p.Id == chosenProduct.Id);

            if (productToRemove != null)
            {
                db.Products.Remove(productToRemove);
                db.SaveChanges();
            }

            // ?? UI resetten
            LoadProducts();
            detailsPanel.Visibility = Visibility.Collapsed;
            placeholderTextBlock.Visibility = Visibility.Visible;
            chosenProduct = null;
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            Frame.Navigate(typeof(ProductPage), chosenProduct);
        }

        private void TruckButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int productId = (int)button.Tag;
                Frame.Navigate(typeof(DeliveryPage), productId);
            }
        }

        private void MaterialsListButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null) return;

            // Koffieboon => Ingrediënten via ProductPage
            if (chosenProduct.Category == "Koffieboon")
            {
                Frame.Navigate(typeof(IngredientListPage), chosenProduct.Id);
            }
            else // Overige categorieën => Materialenlijst
            {
                Frame.Navigate(typeof(MaterialListPage), chosenProduct.Id);
            }
        }

        private void UsedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            Frame.Navigate(typeof(UsedProductPage), chosenProduct.Id);
        }
    }

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
