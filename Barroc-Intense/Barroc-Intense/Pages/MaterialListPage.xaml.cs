using Barroc_Intense.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class MaterialListPage : Page
    {
        public MaterialListPage()
        {
            this.InitializeComponent();
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            using var db = new AppDbContext();
            materialsListView.ItemsSource = db.Materials.ToList();
        }

        private void BackToStockButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.GoBack(); // Of navigeren naar StockPage
        }
    }


}
