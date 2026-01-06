using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class MaterialListPage : Page
    {
        private int _meldingId;

        public MaterialListPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Parameter is de MeldingId
            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int meldingId))
            {
                _meldingId = meldingId;
                LoadMaterialsForMelding(_meldingId);
            }
        }

        private void LoadMaterialsForMelding(int meldingId)
        {
            using var db = new AppDbContext();

            // Alleen materialen die gebruikt zijn bij deze melding
            var usedMaterials = db.MaintenanceMaterials
                                  .Where(mm => mm.MeldingId == meldingId)
                                  .Join(db.Materials,
                                        mm => mm.MaterialId,
                                        m => m.Id,
                                        (mm, m) => new
                                        {
                                            m.Name,
                                            m.Price,
                                            PriceFormatted = $"€{m.Price:0.##}",
                                            mm.QuantityUsed
                                        })
                                  .ToList();

            materialsListView.ItemsSource = usedMaterials;
        }

        private void BackToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }
}
