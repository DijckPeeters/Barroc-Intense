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
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class MaintenancePagee : Page
    {
        private AppDbContext _db = new AppDbContext();
        private DateTime _huidigeDatum = DateTime.Today;
        private List<Melding> _alleMeldingen;

        public MaintenancePagee()
        {
            this.InitializeComponent();
            _db.Database.EnsureCreated();

            LaadMeldingen();
            LaadMachines();
            
        }

        // ================== Melding & Product ==================
        private void LaadMeldingen()
        {
            MaintenanceListView.ItemsSource = _db.Meldingen
                .Include(m => m.Machine)
                .Include(m => m.Delivery)
                .OrderByDescending(m => m.Datum)
                .ToList();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string zoekterm = (SearchBox.Text ?? string.Empty).ToLower();

            MaintenanceListView.ItemsSource = _db.Meldingen
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
            bool heeftOpenKeuringen = _db.Meldingen.Any(m => m.IsKeuring && !m.IsKeuringVoltooid);

            if (heeftOpenKeuringen)
            {
                await new ContentDialog
                {
                    Title = "Keuring nog niet afgerond",
                    Content = "Je kunt niet opslaan/verwijderen want er staan keuringen open.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
                return;
            }

            var meldingen = _db.Meldingen.Where(m => m.IsKeuringVoltooid).ToList();

            foreach (var oud in meldingen)
            {
                var nieuwe = new Melding
                {
                    MonteurId = oud.MonteurId,
                    MachineId = oud.MachineId,
                    Prioriteit = oud.Prioriteit,
                    Afdeling = oud.Afdeling,
                    Datum = oud.Datum.Value.AddMonths(1),
                    Klant = oud.Klant,
                    Product = oud.Product,
                    Probleemomschrijving = oud.Probleemomschrijving,
                    Status = "Open",
                    IsOpgelost = false,
                    IsKeuring = oud.IsKeuring,
                    ChecklistVolledig = null,
                    KeuringGoedgekeurd = null,
                    KeuringOpmerkingen = null,
                    IsKeuringVoltooid = false,
                    Handtekening = null
                };
                _db.Meldingen.Add(nieuwe);
                _db.Meldingen.Remove(oud);
            }

            _db.SaveChanges();

            await new ContentDialog
            {
                Title = "Opgeslagen",
                Content = $"{meldingen.Count} melding(en) opnieuw ingepland voor volgende maand.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();

            LaadMeldingen();
        }

        private void KlantButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KlantenservicePage));
        }

        private void OpenForm_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Tag == null) return;

            int id = (int)button.Tag;
            Frame.Navigate(typeof(FormulierPage), id);
        }

        private async void VerwijderMelding_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int id = (int)button.Tag;

            var melding = _db.Meldingen.FirstOrDefault(m => m.Id == id);
            if (melding == null) return;

            var result = await new ContentDialog
            {
                Title = "Weet je het zeker?",
                Content = $"Melding van {melding.Klant} verwijderen?",
                PrimaryButtonText = "Ja, verwijderen",
                CloseButtonText = "Nee",
                XamlRoot = this.XamlRoot
            }.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                _db.Meldingen.Remove(melding);
                _db.SaveChanges();
                LaadMeldingen();

                await new ContentDialog
                {
                    Title = "Verwijderd",
                    Content = "Melding succesvol verwijderd.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
            }
        }

        private void BewerkMelding_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int id = (int)button.Tag;
            Frame.Navigate(typeof(MeldingBewerkenPage), id);
        }

        // ================== Machines & WeekAgenda ==================
        private void LaadMachines()
        {
            MachinesListView.ItemsSource = _db.Machines
                .Include(m => m.Deliveries)
                .ToList();
        }

      

        // ================== Kalender ==================
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaadAlleMeldingenVoorKalender();
            UpdateKalenderHeader();
        }

        private void LaadAlleMeldingenVoorKalender()
        {
            _alleMeldingen = _db.Meldingen.ToList();
            Kalender.SelectedDates.Clear();
            Kalender.SetDisplayDate(_huidigeDatum);
        }

        private void UpdateKalenderHeader()
        {
            MonthYearText.Text = _huidigeDatum.ToString("MMMM yyyy");
        }

        private void Kalender_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Item == null) return;

            DateTime day = args.Item.Date.Date;
            bool heeftMeldingen = _alleMeldingen != null && _alleMeldingen.Any(m => m.Datum.HasValue && m.Datum.Value.Date == day);

            if (heeftMeldingen)
            {
                args.Item.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
                args.Item.BorderThickness = new Thickness(2);
                try { args.Item.CornerRadius = new CornerRadius(6); } catch { }
            }
            else
            {
                args.Item.BorderBrush = null;
                args.Item.BorderThickness = new Thickness(0);
                try { args.Item.CornerRadius = new CornerRadius(0); } catch { }
            }
        }

        private void Kalender_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (sender.SelectedDates.Count > 0)
            {
                LaadMeldingenVoorDatum(sender.SelectedDates[0].Date);
            }
        }

        private void LaadMeldingenVoorDatum(DateTime date)
        {
            DagMeldingenControl.ItemsSource = _db.Meldingen
                .Where(m => m.Datum.HasValue && m.Datum.Value.Date == date.Date)
                .ToList();
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddMonths(-1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddMonths(1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        private void PrevYear_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddYears(-1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        private void NextYear_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddYears(1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }
    }

    // ================== Converters ==================
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var priority = (value as string)?.ToLower() ?? string.Empty;

            if (priority == "hoog" || priority == "high") return new SolidColorBrush(Microsoft.UI.Colors.Red);
            if (priority == "middel" || priority == "medium") return new SolidColorBrush(Microsoft.UI.Colors.Orange);
            if (priority == "laag" || priority == "low") return new SolidColorBrush(Microsoft.UI.Colors.Green);
            return new SolidColorBrush(Microsoft.UI.Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

    public class HighPriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var priority = (value as string)?.ToLower() ?? string.Empty;
            return (priority == "hoog" || priority == "high") ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
