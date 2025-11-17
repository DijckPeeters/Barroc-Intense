using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Barroc_Intense.Pages
{
    public sealed partial class InkoopDashBoard : Page
    {
        public InkoopDashBoard()
        {
            this.InitializeComponent();
        }

        private void ViewStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPage));
        }

        private void GoToDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DeliveryPage)); 
        }
    }
}
