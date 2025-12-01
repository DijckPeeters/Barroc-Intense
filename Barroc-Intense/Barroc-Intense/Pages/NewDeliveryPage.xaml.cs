using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class NewDeliveryPage : Page
    {
        public NewDeliveryPage()
        {
            this.InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validatie van numerieke velden
            if (!int.TryParse(newProductId.Text, out int productId) ||
                !int.TryParse(newQuantity.Text, out int quantityDelivered) ||
                !int.TryParse(newExpectedQuantity.Text, out int quantityExpected) ||
                !int.TryParse(newOrderId.Text, out int orderId) ||
                !int.TryParse(newCustomerId.Text, out int customerId))
            {
                validationResultsTextBlock.Text = "? Zorg dat alle ID's en aantallen correcte getallen zijn.";
                return;
            }

            // Validatie verplichte velden
            if (string.IsNullOrWhiteSpace(newCustomerName.Text) || string.IsNullOrWhiteSpace(newAddress.Text))
            {
                validationResultsTextBlock.Text = "? Klantnaam en afleveradres zijn verplicht.";
                return;
            }

            try
            {
                using var db = new AppDbContext();

                // Haal productnaam op uit database
                var product = db.Products.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    validationResultsTextBlock.Text = "? Ongeldig Product ID!";
                    return;
                }
                var productName = product.ProductName;

                // Optioneel: laat ProductName zien in de TextBox
                newProductName.Text = productName;

                // Maak nieuwe Delivery
                var delivery = new Delivery
                {
                    // EF zal DeliveryID automatisch genereren als identity
                    DeliveryID = 0,
                    OrderID = orderId,
                    ProductID = productId,
                    ProductName = productName,
                    QuantityDelivered = quantityDelivered,
                    QuantityExpected = quantityExpected,
                    PlannedDeliveryDate = newPlannedDate.Date.DateTime,
                    ActualDeliveryDate = null,
                    Status = string.IsNullOrWhiteSpace(newStatus.Text) ? "Planned" : newStatus.Text,
                    CustomerID = customerId,
                    CustomerName = newCustomerName.Text,
                    DeliveryAddress = newAddress.Text,
                    CustomerContact = string.IsNullOrWhiteSpace(newCustomerContact.Text) ? "" : newCustomerContact.Text,
                    CarrierName = string.IsNullOrWhiteSpace(newCarrier.Text) ? "Onbekend" : newCarrier.Text,
                    DriverName = string.IsNullOrWhiteSpace(newDriver.Text) ? "Onbekend" : newDriver.Text,
                    TrackingNumber = string.IsNullOrWhiteSpace(newTrackingNumber.Text) ? "" : newTrackingNumber.Text,
                    Notes = string.IsNullOrWhiteSpace(newNotes.Text) ? "" : newNotes.Text
                };

                // Voeg toe aan database
                db.Deliveries.Add(delivery);
                db.SaveChanges();

                // Succesmelding
                var dialog = new ContentDialog
                {
                    Title = "Succes",
                    Content = "Nieuwe levering is succesvol toegevoegd!",
                    CloseButtonText = "Ok",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();

                // Terug naar overzicht
                Frame.GoBack();
            }
            catch (Exception ex)
            {
                validationResultsTextBlock.Text = $"? Fout bij opslaan: {ex.Message}";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigeer terug naar de DeliveryPage
            Frame.Navigate(typeof(DeliveryPage));
        }

    }
}
