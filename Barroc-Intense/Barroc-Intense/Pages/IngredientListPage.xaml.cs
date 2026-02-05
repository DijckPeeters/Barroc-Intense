using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class IngredientListPage : Page
    {
        private int productId;

        public IngredientListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // ProductId wordt meegegeven bij navigatie vanuit voorraadpagina
            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int id))
            {
                productId = id;
                LoadIngredients();
            }
        }

        private void LoadIngredients()
        {
            using var db = new AppDbContext();

            // Alleen ingrediënten ophalen die bij het geselecteerde product horen
            var ingredients = db.Ingredients
                                .Where(i => i.ProductId == productId)
                                .ToList();

            ingredientsListView.ItemsSource = ingredients;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }
}