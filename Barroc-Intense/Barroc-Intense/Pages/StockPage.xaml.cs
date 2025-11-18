using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            productListView.ItemsSource = db.Products.ToList();
        }

        private void ShowProductDetails(Product selectedProduct)
        {
            detailsPanel.Visibility = Visibility.Visible;
            placeholderTextBlock.Visibility = Visibility.Collapsed;

            detailNameTextBlock.Text = selectedProduct.ProductName;
            detailLeaseContractTextBlock.Text =
                string.IsNullOrWhiteSpace(selectedProduct.LeaseContract)
                ? "Geen contract"
                : selectedProduct.LeaseContract;
            detailPriceTextBlock.Text = $"€ {selectedProduct.PricePerKg:0.00}";
            detailCategoryTextBlock.Text = string.IsNullOrWhiteSpace(selectedProduct.Category)
                ? "Geen categorie"
                : selectedProduct.Category;

            detailInstallationCostTextBlock.Text = selectedProduct.InstallationCost > 0
    ? $"€ {selectedProduct.InstallationCost:0.00} per maand"
    : "Geen maandelijkse reparatiekosten";



            detailStockTextBlock.Text = $"{selectedProduct.Stock} op voorraad";
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
            //Frame.GoBack();
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

            var dialog = new ContentDialog
            {
                Title = "Weet u het zeker?",
                Content = $"Weet u zeker dat u het product \"{chosenProduct.ProductName}\" wilt verwijderen?",
                PrimaryButtonText = "Verwijderen",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                using var db = new AppDbContext();
                var productToRemove = db.Products.FirstOrDefault(p => p.Id == chosenProduct.Id);

                if (productToRemove != null)
                {
                    db.Products.Remove(productToRemove);
                    db.SaveChanges();
                }

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
    }
}
