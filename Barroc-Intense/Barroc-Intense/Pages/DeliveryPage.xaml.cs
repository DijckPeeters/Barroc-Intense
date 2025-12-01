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
        private List<Delivery> deliveries = new List<Delivery>();
        private Delivery selectedDelivery = null;
        private int? productIdFilter = null;

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

        private void LoadDeliveries()
        {
            using var db = new AppDbContext();
            deliveries = db.Deliveries.ToList();
            deliveryListView.ItemsSource = deliveries;

            if (productIdFilter.HasValue)
            {
                var deliveryToSelect = deliveries.FirstOrDefault(d => d.ProductID == productIdFilter.Value);
                if (deliveryToSelect != null)
                {
                    deliveryListView.SelectedItem = deliveryToSelect;
                    ShowDetails(deliveryToSelect);
                }
            }
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

            ContentDialog dialog = new ContentDialog
            {
                Title = "Aanpassen levering",
                Content = $"Je gaat levering {selectedDelivery.DeliveryID} aanpassen (nog niet geïmplementeerd).",
                CloseButtonText = "Ok"
            };
            _ = dialog.ShowAsync();
        }

        private void DeleteDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDelivery == null) return;

            ContentDialog confirm = new ContentDialog
            {
                Title = "Weet je het zeker?",
                Content = $"Weet je zeker dat je levering {selectedDelivery.DeliveryID} wilt verwijderen?",
                PrimaryButtonText = "Ja",
                CloseButtonText = "Nee"
            };

            var result = confirm.ShowAsync();
            result.AsTask().ContinueWith(t =>
            {
                if (t.Result == ContentDialogResult.Primary)
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

                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        deliveryListView.ItemsSource = null;
                        deliveryListView.ItemsSource = deliveries;
                        detailsPanel.Visibility = Visibility.Collapsed;
                        placeholderText.Visibility = Visibility.Visible;
                    });
                }
            });
        }

        private void AddDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigeer naar NewDeliveryPage
            Frame.Navigate(typeof(NewDeliveryPage));
        }
    }
}
