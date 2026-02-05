using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Barroc_Intense.Pages
{
    public sealed partial class InkoopDashBoard : Page
    {
        public InkoopDashBoard()
        {
            InitializeComponent();
        }

        // Navigeert naar de pagina met productvoorraad
        private void ViewStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }

        // Navigeert naar de pagina om een nieuw product toe te voegen
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage));
        }

        // Navigeert naar het leveringenoverzicht
        private void GoToDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DeliveryPage));
        }
    }
}