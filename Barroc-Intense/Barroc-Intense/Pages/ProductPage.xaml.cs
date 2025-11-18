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
        public ProductPage()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Maak een nieuw Product-object op basis van de ingevoerde gegevens
            var product = new Product
            {
                ProductName = productNameTextBox.Text,
                ingredient = ingredientTextBox.Text,
                Price = int.TryParse(priceTextBox.Text, out var price) ? price : 0,
                Stock = int.TryParse(stockTextBox.Text, out var stock) ? stock : 0
            };

            // Valideer het object met DataAnnotations
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(product, context, results, true))
            {
                // Toon validatiefouten
                var errors = results.Select(r => r.ErrorMessage).ToList();
                validationResultsTextBlock.Text = string.Join(Environment.NewLine, errors);
            }
            else
            {
                validationResultsTextBlock.Text = "✅ Validatie geslaagd!";

                try
                {
                    using var db = new AppDbContext();
                    db.Products.Add(product);
                    db.SaveChanges();

                    validationResultsTextBlock.Text += "\n💾 Product succesvol opgeslagen!";
                }
                catch (Exception ex)
                {
                    errorsTextBlock.Text = $"❌ Fout bij opslaan: {ex.Message}";
                }
            }
        }

    }
}
