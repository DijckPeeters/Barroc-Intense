using Barroc_Intense.Data;
using Microsoft.EntityFrameworkCore;
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
using static Barroc_Intense.Data.AppDbContext;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



namespace Barroc_Intense.Pages
{
    public sealed partial class MaintenancePagee : Page
    {
        private AppDbContext _db = new AppDbContext();

        public MaintenancePagee()
        {
            this.InitializeComponent();
            _db.Database.EnsureCreated();
            LaadMeldingen();
            LaadMachines();  // Rechterkolom Machines
            LaadWeekAgenda();
            LaadGebruikteProducten();  


        }

        private void LaadMachines()
        {
            MachinesListView.ItemsSource = _db.Machines
                .Include(m => m.Deliveries)
                .ToList();
        }




        private void LaadWeekAgenda()
        {
            var vandaag = DateTime.Today;
            var weekStart = vandaag.AddDays(-(int)vandaag.DayOfWeek + (int)DayOfWeek.Monday);
            var weekEnd = weekStart.AddDays(7);

            WeekAgendaControl.ItemsSource = _db.Meldingen
                 .Include(m => m.Machine)
                     .ThenInclude(mc => mc.Deliveries)
                 .Include(m => m.Delivery)                // ? Voeg dit toe!
                     .ThenInclude(d => d.Product)         // ? En laad ook het product
                 .Where(m => m.Datum >= weekStart && m.Datum < weekEnd)
                 .OrderBy(m => m.Datum)
                 .ToList();
        }




        private void LaadMeldingen()
        {
            MaintenanceListView.ItemsSource = _db.Meldingen
                .Include(m => m.Machine)       // laad de machine van de melding
                .Include(m => m.Delivery)      // laad de delivery van de melding
                .OrderByDescending(m => m.Datum)
                .ToList();
        }

        private void LaadGebruikteProducten()
        {
            var meldingen = _db.Meldingen
                .Include(m => m.Machine)
                .OrderByDescending(m => m.Datum)
                .ToList();

            MaintenanceListView.ItemsSource = meldingen;
        }




        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string zoekterm = (SearchBox.Text ?? string.Empty).ToLower();

            MaintenanceListView.ItemsSource = _db.Meldingen
                .Include(m => m.Product)
                .Where(m => (m.Klant ?? string.Empty).ToLower().Contains(zoekterm)
                         || (m.Product ?? string.Empty).ToLower().Contains(zoekterm)
                         || (m.Afdeling ?? string.Empty).ToLower().Contains(zoekterm))
                .OrderByDescending(m => m.Datum)
                .ToList();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LaadMeldingen();
        }

        private async void OnSaveClick(object sender, RoutedEventArgs e)
        {
            // Save changes (bijv. IsOpgelost checkbox changes)
            _db.SaveChanges();

            var dialog = new ContentDialog
            {
                Title = "Opgeslagen",
                Content = "Wijzigingen zijn opgeslagen.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
            LaadMeldingen();
        }
        private void KlantButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KlantenservicePage));
        }
    }
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var priority = (value as string)?.ToLower() ?? string.Empty;

            // Map mogelijke databasewaarden naar kleuren
            if (priority == "hoog" || priority == "high")
                return new SolidColorBrush(Microsoft.UI.Colors.Red);

            if (priority == "middel" || priority == "medium")
                return new SolidColorBrush(Microsoft.UI.Colors.Orange);

            if (priority == "laag" || priority == "low")
                return new SolidColorBrush(Microsoft.UI.Colors.Green);

            return new SolidColorBrush(Microsoft.UI.Colors.Black);
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HighPriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var priority = (value as string)?.ToLower() ?? string.Empty;

            if (priority == "hoog" || priority == "high")
                return Visibility.Visible;

            return Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }


}
