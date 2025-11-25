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

                // Zet productnaam bovenaan
                ProductTitleText.Text = $"{loadedProduct.ProductName} (Gebruikt: {loadedProduct.Used}x)";

                // Maak een lijst van placeholder items (1 per "Used" aantal)
                var usedList = Enumerable.Range(1, loadedProduct.Used)
                                         .Select(i => $"{loadedProduct.ProductName} #{i}")
                                         .ToList();

                UsedBoxesList.ItemsSource = usedList;
            }
        }

        private void MaterialButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            // productnaam/instance doorgeven
            string selectedInstance = button.Tag.ToString();

            // Navigeer naar materialenlijst van dit product
            Frame.Navigate(typeof(MaterialListPage), loadedProduct.Id);
        }
    }
}
