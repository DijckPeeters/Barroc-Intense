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
        public StockPage()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                productListView.ItemsSource = db.Products
                    .ToList();
            }
        }


        // private void productListView_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var product = (Product)e.ClickedItem;
        //    var productId = product.Id;

            //    using var db = new AppDbContext();

            //    db.Entry(product)
            //        .Collection(p => p.Stock)
            //        .Load();

            //    StocksListView.ItemSource = product.Stocks;

            //}

        

        

        private void productListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var product = (Product)e.ClickedItem;

            var productId = product.Id;

            using var db = new AppDbContext();

            //if (e.ClickedItem is Product selectedProduct)
            //{
            //    detailNameTextBlock.Text = selectedProduct.ProductName;
            //    detailIngredientTextBlock.Text = selectedProduct.ingredient;
            //    detailPriceTextBlock.Text = $"€ {selectedProduct.Price}";
            //    detailStockTextBlock.Text = $"{selectedProduct.Stock} op voorraad";
            //}

            //db.Entry(product)
            //    .Collection(s => s.Products)
            //    .Load();

            productListView.ItemsSource = db.Products.ToList();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainWindow));
        }


    }
}

