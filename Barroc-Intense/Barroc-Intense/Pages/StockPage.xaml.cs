using Barroc_Intense.Data;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockPage : Page
    {
        private Product chosenProduct = null;

        public StockPage()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                productListView.ItemsSource = db.Products
                    .ToList();
            }
        }


        private void ShowProductDetails(Product selectedProduct)
        {
            detailsPanel.Visibility = Visibility.Visible;
            placeholderTextBlock.Visibility = Visibility.Collapsed;

            detailNameTextBlock.Text = selectedProduct.ProductName;
            detailIngredientTextBlock.Text = selectedProduct.ingredient;
            detailPriceTextBlock.Text = $"€ {selectedProduct.Price}";
            detailStockTextBlock.Text = $"{selectedProduct.Stock} op voorraad";
        }
        private void productListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var product = (Product)e.ClickedItem;
            var productId = product.Id;

            chosenProduct = product;

            using var db = new AppDbContext();
            if (e.ClickedItem is Product selectedProduct)
            {
                chosenProduct = selectedProduct;
                ShowProductDetails(selectedProduct);
            }

            //if (e.ClickedItem is Product selectedProduct)
            //{
            //    detailsPanel.Visibility = Visibility.Visible;
            //    placeholderTextBlock.Visibility = Visibility.Collapsed;

            //    detailNameTextBlock.Text = selectedProduct.ProductName;
            //    detailIngredientTextBlock.Text = selectedProduct.ingredient;
            //    detailPriceTextBlock.Text = $"€ {selectedProduct.Price}";
            //    detailStockTextBlock.Text = $"{selectedProduct.Stock} op voorraad";
            //}

            productListView.ItemsSource = db.Products.ToList();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            using var db = new AppDbContext();
            productListView.ItemsSource = db.Products.ToList();

            // Controleer of er een parameter is
            if (e.Parameter != null)
            {
                // Probeer parameter als int te gebruiken
                if (int.TryParse(e.Parameter.ToString(), out int productId))
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == productId);
                    if (product != null)
                    {
                        chosenProduct = product;

                        detailsPanel.Visibility = Visibility.Visible;
                        placeholderTextBlock.Visibility = Visibility.Collapsed;

                        detailNameTextBlock.Text = product.ProductName;
                        detailIngredientTextBlock.Text = product.ingredient;
                        detailPriceTextBlock.Text = $"€ {product.Price}";
                        detailStockTextBlock.Text = $"{product.Stock} op voorraad";
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage));
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage));
        }


        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var product = chosenProduct; 

            if (product != null)
            {
                var productToRemove = db.Products.FirstOrDefault(p => p.Id == product.Id);
                if (productToRemove != null)
                {
                    db.Products.Remove(productToRemove);
                    db.SaveChanges();
                }

                productListView.ItemsSource = db.Products.ToList();

                detailsPanel.Visibility = Visibility.Collapsed;
                placeholderTextBlock.Visibility = Visibility.Visible;

                chosenProduct = null;
            }

        }
        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenProduct == null)
                return;

            // Navigeren naar ProductPage en product doorgeven
            Frame.Navigate(typeof(ProductPage), chosenProduct);
        }





    }
}

