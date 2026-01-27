using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class DeliveryPage : Page
    {
        private int? productIdFilter = null;
        private List<Delivery> deliveries = new();
        private Delivery selectedDelivery = null;


        public DeliveryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int id)
                productIdFilter = id;

            LoadDeliveries();
        }
        private void LoadDeliveries(string statusFilter = "Alle")
        {
            using var db = new AppDbContext();
            var query = db.Deliveries.AsQueryable();

            // product filter
            if (productIdFilter.HasValue)
                query = query.Where(d => d.ProductID == productIdFilter.Value);

            // status filter
            if (statusFilter != "Alle")
                query = query.Where(d => d.Status == statusFilter);

            deliveries = query.ToList();
            deliveryListView.ItemsSource = deliveries;

            // reset selectie
            selectedDelivery = null;
            detailsPanel.Visibility = Visibility.Collapsed;
            placeholderText.Visibility = Visibility.Visible;
        }


        private void DeliveryListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Delivery delivery)
            {
                selectedDelivery = delivery;
                ShowDetails(delivery);
            }
        }

        private void ShowDetails(Delivery delivery)
        {
            detailsPanel.Visibility = Visibility.Visible;
            placeholderText.Visibility = Visibility.Collapsed;

            detailProductName.Text = delivery.ProductName;
            detailQuantity.Text = delivery.QuantityDelivered.ToString();
            detailCustomerName.Text = delivery.CustomerName;
            detailAddress.Text = delivery.DeliveryAddress;
            detailStatus.Text = delivery.Status;
            detailPlannedDate.Text = delivery.PlannedDeliveryDate.ToShortDateString();
            detailActualDate.Text = delivery.ActualDeliveryDate?.ToShortDateString() ?? "-";
            detailCarrier.Text = delivery.CarrierName;
            detailDriver.Text = delivery.DriverName;
            detailTrackingNumber.Text = delivery.TrackingNumber;
            detailNotes.Text = delivery.Notes;
        }

        private void BackToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InkoopDashBoard));
        }

        private void EditDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDelivery == null) return;

            Frame.Navigate(typeof(NewDeliveryPage), selectedDelivery);
        }


        private async void DeleteDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDelivery == null) return;

            ContentDialog confirm = new ContentDialog
            {
                Title = "Weet je het zeker?",
                Content = $"Weet je zeker dat je levering {selectedDelivery.DeliveryID} wilt verwijderen?",
                PrimaryButtonText = "Ja",
                CloseButtonText = "Nee",
                XamlRoot = this.XamlRoot   // << BELANGRIJK IN WINUI 3
            };

            var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                using var db = new AppDbContext();
                var deliveryToRemove = db.Deliveries.FirstOrDefault(d => d.DeliveryID == selectedDelivery.DeliveryID);

                if (deliveryToRemove != null)
                {
                    db.Deliveries.Remove(deliveryToRemove);
                    db.SaveChanges();
                }

                deliveries.Remove(selectedDelivery);
                selectedDelivery = null;

                deliveryListView.ItemsSource = null;
                deliveryListView.ItemsSource = deliveries;

                detailsPanel.Visibility = Visibility.Collapsed;
                placeholderText.Visibility = Visibility.Visible;
            }
        }

        private void FilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item)
            {
                string status = item.Text;

                LoadDeliveries(status);

                // knoptekst aanpassen
                filterButton.Content = status;
            }
        }



        //private void LoadUsedProducts()
        //{
        //    using var db = new AppDbContext();

        //    var usedProducts = (from d in db.Deliveries
        //                        where d.ActualDeliveryDate != null
        //                        orderby d.ActualDeliveryDate descending
        //                        select new UsedProductMeldingViewModel
        //                        {
        //                            Id = d.DeliveryID,
        //                            MachineId = d.ProductID, // als je machine info wilt aanpassen
        //                            Afdeling = "Algemeen",    // vul in als je een mapping hebt
        //                            Datum = d.ActualDeliveryDate.Value,
        //                            Klant = d.CustomerName,
        //                            Product = d.ProductName,
        //                            Probleemomschrijving = $"Aantal geleverd: {d.QuantityDelivered}. Notities: {d.Notes}",
        //                            Status = d.Status,
        //                            IsOpgelost = true
        //                        }).ToList();

        //    usedProductsListView.ItemsSource = usedProducts;
        //}


        private void AddDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigeer naar NewDeliveryPage
            Frame.Navigate(typeof(NewDeliveryPage));
        }
    }
}
