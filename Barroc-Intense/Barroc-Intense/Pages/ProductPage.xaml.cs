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
using System.ComponentModel.DataAnnotations;
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
    public sealed partial class ProductPage : Page
    {
        private Product editingProduct = null;

        public ProductPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Product productToEdit)
            {
                editingProduct = productToEdit;

                productNameTextBox.Text = editingProduct.ProductName;
                ingredientTextBox.Text = editingProduct.ingredient;
                priceTextBox.Text = editingProduct.Price.ToString();
                stockTextBox.Text = editingProduct.Stock.ToString();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editingProduct == null)
            {
                var product = new Product
                {
                    ProductName = productNameTextBox.Text,
                    ingredient = ingredientTextBox.Text,
                    Price = int.TryParse(priceTextBox.Text, out var price) ? price : 0,
                    Stock = int.TryParse(stockTextBox.Text, out var stock) ? stock : 0
                };

                SaveProduct(product, isNew: true);
            }
            else
            {
                editingProduct.ProductName = productNameTextBox.Text;
                editingProduct.ingredient = ingredientTextBox.Text;
                editingProduct.Price = int.TryParse(priceTextBox.Text, out var price) ? price : 0;
                editingProduct.Stock = int.TryParse(stockTextBox.Text, out var stock) ? stock : 0;

                SaveProduct(editingProduct, isNew: false);
            }

        }

        private void SaveProduct(Product product, bool isNew)
        {
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(product, context, results, true))
            {
                var errors = results.Select(r => r.ErrorMessage).ToList();
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, errors);
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
            Frame.Navigate(typeof(StockPage), product.Id);

        }


        private void goToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }

}
