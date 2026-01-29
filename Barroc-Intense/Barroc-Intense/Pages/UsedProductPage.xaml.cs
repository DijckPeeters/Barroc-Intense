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
                ProductTitleText.Text = $"{loadedProduct.ProductName} (Gebruikt: {loadedProduct.UsedCount}x)";
                // Gebruikte product instanties
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
            if (sender is Button btn && btn.Tag != null &&
                int.TryParse(btn.Tag.ToString(), out int meldingId))
            {
                Frame.Navigate(typeof(MaterialListPage), meldingId);
            }
        }
        private void BackToStockButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }
}