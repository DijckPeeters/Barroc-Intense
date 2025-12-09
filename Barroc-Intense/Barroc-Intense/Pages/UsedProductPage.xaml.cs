using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class UsedProductPage : Page
    {
        private Product loadedProduct;

        public UsedProductPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int productId))
            {
                using var db = new AppDbContext();
                loadedProduct = db.Products.FirstOrDefault(p => p.Id == productId);

                if (loadedProduct == null)
                    return;

                // Aantal keer gebruikt
                loadedProduct.UsedCount = db.Deliveries
                    .Count(d => d.ProductID == loadedProduct.Id && d.Status == "Delivered");

                // Zet productnaam bovenaan
                ProductTitleText.Text = $"{loadedProduct.ProductName} (Gebruikt: {loadedProduct.UsedCount}x)";

                // Genereer "gebruikte product instanties"
                var usedList = db.Deliveries
                     .Where(d => d.ProductID == loadedProduct.Id && d.Status == "Delivered")
                     .Select(d => new
                     {
                         ProductName = loadedProduct.ProductName,
                         d.CustomerName,
                         d.DeliveryAddress,
                         d.PlannedDeliveryDate,
                         d.ActualDeliveryDate,
                         d.DeliveryID,
                         // ✅ button text afhankelijk van categorie
                         ButtonText = loadedProduct.Category == "Koffieboon"
                             ? "📋 Gebruikte ingrediënten"
                             : "📋 Gebruikte materialen"
                     })
                     .ToList();

                UsedBoxesList.ItemsSource = usedList;
            }
        }

        






        private void MaterialButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            // productnaam/instance doorgeven
            string selectedInstance = button.Tag.ToString();

            if (loadedProduct == null)
                return;

            // Check categorie
            if (loadedProduct.Category == "Koffieboon")
            {
                // Navigeren naar ingrediëntenpagina
                Frame.Navigate(typeof(IngredientListPage), loadedProduct.Id);
            }
            else
            {
                // Navigeren naar materialenpagina
                Frame.Navigate(typeof(MaterialListPage), loadedProduct.Id);
            }
        }


        private void BackToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }
}
