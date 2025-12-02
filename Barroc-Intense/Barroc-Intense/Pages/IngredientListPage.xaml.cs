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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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

            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int id))
            {
                productId = id;
                LoadIngredients();
            }
        }

        private void LoadIngredients()
        {
            using var db = new AppDbContext();

            // Haal alle ingrediënten op die horen bij de gekozen koffieboon
            var ingredients = db.Ingredients
                                .Where(i => i.ProductId == productId)
                                .ToList();

            ingredientsListView.ItemsSource = ingredients;
        }

        private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }
    }
}
