using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class NewDeliveryPage : Page
    {
        private Delivery editingDelivery = null;

        public NewDeliveryPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Delivery d)
            {
                editingDelivery = d;
                LoadFieldsForEditing();
            }
        }

        private void LoadFieldsForEditing()
        {
            newProductId.Text = editingDelivery.ProductID.ToString();
            newOrderId.Text = editingDelivery.OrderID.ToString();
            newCustomerId.Text = editingDelivery.CustomerID.ToString();
            newCustomerContact.Text = editingDelivery.CustomerContact;
            newProductName.Text = editingDelivery.ProductName;
            newQuantity.Text = editingDelivery.QuantityDelivered.ToString();
            newExpectedQuantity.Text = editingDelivery.QuantityExpected.ToString();
            newCustomerName.Text = editingDelivery.CustomerName;
            newAddress.Text = editingDelivery.DeliveryAddress;
            newPlannedDate.Date = editingDelivery.PlannedDeliveryDate;
            newStatus.Text = editingDelivery.Status;
            newCarrier.Text = editingDelivery.CarrierName;
            newDriver.Text = editingDelivery.DriverName;
            newTrackingNumber.Text = editingDelivery.TrackingNumber;
            newNotes.Text = editingDelivery.Notes;
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
                newProductName.Text = productName; 

               
                if (editingDelivery != null)
                {
                    var d = db.Deliveries.FirstOrDefault(x => x.DeliveryID == editingDelivery.DeliveryID);

                    if (d != null)
                    {
                        d.ProductID = productId;
                        d.ProductName = productName;
                        d.OrderID = orderId;
                        d.CustomerID = customerId;
                        d.CustomerName = newCustomerName.Text;
                        d.CustomerContact = newCustomerContact.Text;
                        d.DeliveryAddress = newAddress.Text;
                        d.QuantityDelivered = quantityDelivered;
                        d.QuantityExpected = quantityExpected;
                        d.PlannedDeliveryDate = newPlannedDate.Date.DateTime;
                        d.Status = string.IsNullOrWhiteSpace(newStatus.Text) ? "Planned" : newStatus.Text;
                        d.CarrierName = newCarrier.Text;
                        d.DriverName = newDriver.Text;
                        d.TrackingNumber = newTrackingNumber.Text;
                        d.Notes = newNotes.Text;
                    }

                    db.SaveChanges();

                    var dialog = new ContentDialog
                    {
                        Title = "Succes",
                        Content = "Levering succesvol bijgewerkt! ?",
                        CloseButtonText = "Ok",
                        XamlRoot = this.XamlRoot
                    };

                    await dialog.ShowAsync();
                    Frame.GoBack();
                    return;
                }

               
                var delivery = new Delivery
                {
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
                    CustomerContact = newCustomerContact.Text,
                    DeliveryAddress = newAddress.Text,
                    CarrierName = newCarrier.Text,
                    DriverName = newDriver.Text,
                    TrackingNumber = newTrackingNumber.Text,
                    Notes = newNotes.Text
                };

                db.Deliveries.Add(delivery);
                db.SaveChanges();

                var addDialog = new ContentDialog
                {
                    Title = "Succes",
                    Content = "Nieuwe levering is succesvol toegevoegd! ?",
                    CloseButtonText = "Ok",
                    XamlRoot = this.XamlRoot
                };

                await addDialog.ShowAsync();
                Frame.GoBack();
            }
            catch (Exception ex)
            {
                validationResultsTextBlock.Text = $"? Fout bij opslaan: {ex.Message}";
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DeliveryPage));
        }

    }
}
