using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
namespace Barroc_Intense.Pages
{
    public sealed partial class MaterialListPage : Page
    {
        private int? meldingId;
        private Product product;
        public MaterialListPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int id)
            {
                // Komt van UsedProductPage ? gebruikte materialen
                meldingId = id;
                LoadUsedMaterials();
            }
            else if (e.Parameter is Product p)
            {
                // Komt van StockPage ? product-materialen
                product = p;
                LoadProductMaterials();
            }
        }
        private void LoadUsedMaterials()
        {
            using var db = new AppDbContext();
            var usedMaterials = db.MaintenanceMaterials
                .Where(mm => mm.MeldingId == meldingId)
                .Join(db.Materials,
                    mm => mm.MaterialId,
                    m => m.Id,
                    (mm, m) => new
                    {
                        m.Name,
                        PriceFormatted = $"€{m.Price:0.##}",
                        mm.QuantityUsed
                    })
                .ToList();
            materialsListView.ItemsSource = usedMaterials;
        }
        private void LoadProductMaterials()
        {
            using var db = new AppDbContext();
            // ?? ALLE materialen (machine-materialen)
            var allMaterials = db.Materials
                .Select(m => new
                {
                    m.Name,
                    PriceFormatted = $"€{m.Price:0.##}"
                })
                .ToList();
            materialsListView.ItemsSource = allMaterials;
        }
        private void BackToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}