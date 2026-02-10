using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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

                // Laad het product uit de database
                loadedProduct = db.Products.FirstOrDefault(p => p.Id == productId);
                if (loadedProduct == null) return;

                // Bereken totaal aantal keer gebruikt (alle delivered)
                loadedProduct.UsedCount = db.Deliveries
                    .Where(d => d.ProductID == loadedProduct.Id && d.Status == "Delivered")
                    .Sum(d => d.QuantityDelivered);

                // Titel tonen met totaal gebruikt
                ProductTitleText.Text = $"{loadedProduct.ProductName} (Gebruikt: {loadedProduct.UsedCount}x)";

                // Laad alle gebruikte product-instanties
                var usedList = db.Deliveries
                    .Where(d => d.ProductID == loadedProduct.Id)
                    .Select(d => new
                    {
                        ProductName = loadedProduct.ProductName,
                        d.CustomerName,
                        d.DeliveryAddress,
                        Status = d.Status,
                        PlannedDeliveryDate = d.PlannedDeliveryDate.ToShortDateString(),
                        ActualDeliveryDate = d.ActualDeliveryDate.HasValue
                            ? d.ActualDeliveryDate.Value.ToShortDateString()
                            : "Nog te plannen", // fallback als delivery nog niet geleverd
                        d.DeliveryID,
                        ButtonText = loadedProduct.Category == "Koffieboon"
                            ? "📋 Gebruikte ingrediënten"
                            : "📋 Gebruikte materialen"
                    })
                    .ToList();

                UsedBoxesList.ItemsSource = usedList;
            }
        }

        // navigeer naar de materialenlijst van deze specifieke delivery
        private void MaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (loadedProduct == null)
                return;

            if (loadedProduct.Category == "Koffieboon")
            {
                // Ingrediëntenlijst van dit product
                Frame.Navigate(typeof(IngredientListPage), loadedProduct.Id);
            }
            else
            {
                // Materialenlijst van dit product
                Frame.Navigate(typeof(MaterialListPage), loadedProduct.Id);
            }
        }

        private void BackToStockButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockPage));
        }

        private string GetUsedStatusText(int productId)
        {
            using var db = new AppDbContext();
            var deliveries = db.Deliveries.Where(d => d.ProductID == productId).ToList();

            int inGebruik = deliveries.Count(d => d.Status == "Delivered");
            int onderweg = deliveries.Count(d => d.Status == "Underway");
            int ingepland = deliveries.Count(d => d.Status == "Planned");
            int moetIngepland = deliveries.Count(d => d.Status == "Not planned");

            var statusLines = new List<string>();
            if (inGebruik > 0) statusLines.Add($"{inGebruik}× in gebruik");
            if (onderweg > 0) statusLines.Add($"{onderweg}× onderweg");
            if (ingepland > 0) statusLines.Add($"{ingepland}× ingepland");
            if (moetIngepland > 0) statusLines.Add($"{moetIngepland}× moet ingepland worden");

            if (statusLines.Count == 0)
                statusLines.Add("0× in gebruik");

            return string.Join(Environment.NewLine, statusLines);
        }
    }
}