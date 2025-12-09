using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


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
            //LaadWeekAgenda();

        }

        private void LaadMachines()
        {
            MachinesListView.ItemsSource = _db.Machines.ToList();
        }


        
        //private void LaadWeekAgenda()
        //{
        //    var vandaag = DateTime.Today;
        //    var weekStart = vandaag.AddDays(-(int)vandaag.DayOfWeek + (int)DayOfWeek.Monday);
        //    var weekEnd = weekStart.AddDays(7);

        //    WeekAgendaControl.ItemsSource = _db.Meldingen
        //        .Where(m => m.Datum.HasValue && m.Datum.Value >= weekStart && m.Datum.Value < weekEnd)
        //        .OrderBy(m => m.Datum)
        //        .ToList();
        //}

        private void LaadMeldingen()
        {
            MaintenanceListView.ItemsSource = _db.Meldingen
                .OrderByDescending(m => m.Datum ?? DateTime.MinValue) // nulls onderaan
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
            // Check of er nog open keuringen zijn
            bool heeftOpenKeuringen = _db.Meldingen.Any(m => m.IsKeuring && !m.IsKeuringVoltooid);

            if (heeftOpenKeuringen)
            {
                var dialog = new ContentDialog
                {
                    Title = "Keuring nog niet afgerond",
                    Content = "Je kunt niet opslaan/verwijderen want er staan keuringen open die nog niet volledig zijn afgerond.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
                return;
            }

            // 🟢 Alle keuringen afgerond → nu mag verwijderen & her-inplannen
            var Meldingen = _db.Meldingen
                .Where(m =>  m.IsKeuringVoltooid)
                .ToList();

            foreach (var oud in Meldingen)
            {
                // Nieuwe her-inplanning
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

            var success = new ContentDialog
            {
                Title = "Opgeslagen",
                Content = $"{Meldingen.Count} melding(en) opnieuw ingepland voor volgende maand.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await success.ShowAsync();

            LaadMeldingen();
            //LaadWeekAgenda();
        }


        private void KlantButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KlantenservicePage));
        }
        private void OpenForm_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button.Tag == null)
                return;

            int id = (int)button.Tag;

            // Open detailpagina van melding
            Frame.Navigate(typeof(FormulierPage), id);
        }
        private async void VerwijderMelding_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int id = (int)button.Tag;

            var melding = _db.Meldingen.FirstOrDefault(m => m.Id == id);

            if (melding == null) return;

            var confirm = new ContentDialog
            {
                Title = "Weet je het zeker?",
                Content = $"Melding van {melding.Klant} verwijderen?",
                PrimaryButtonText = "Ja, verwijderen",
                CloseButtonText = "Nee",
                XamlRoot = this.XamlRoot
            };

            var result = await confirm.ShowAsync();

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
        // ================== KALENDER GEDEELTE ==================

        private DateTime _huidigeDatum = DateTime.Today;
        private List<Melding> _alleMeldingen;

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

        // 🔴 Rode bolletjes per datum
        private void Kalender_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Item == null) return;

            DateTimeOffset datum = args.Item.Date;
            DateTime day = datum.Date;

            bool heeftMeldingen = _alleMeldingen != null
                && _alleMeldingen.Any(m => m.Datum.HasValue && m.Datum.Value.Date == day);

            if (heeftMeldingen)
            {
                args.Item.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                args.Item.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
                args.Item.BorderThickness = new Thickness(2);

                try
                {
                    args.Item.CornerRadius = new CornerRadius(6);
                }
                catch { /* sommige WinUI builds laten dit niet toe */ }
            }
            else
            {
                args.Item.Background = null;
                args.Item.BorderBrush = null;
                args.Item.BorderThickness = new Thickness(0);

                try
                {
                    args.Item.CornerRadius = new CornerRadius(0);
                }
                catch { }
            }
        }



        // 📌 Klik op datum → toon meldingen onder kalender
        private void Kalender_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (sender.SelectedDates.Count > 0)
            {
                var gekozen = sender.SelectedDates[0].Date;
                LaadMeldingenVoorDatum(gekozen);
            }
        }

        private void LaadMeldingenVoorDatum(DateTime date)
        {
            var meldingen = _db.Meldingen
                .Where(m => m.Datum.HasValue && m.Datum.Value.Date == date.Date)
                .ToList();

            DagMeldingenControl.ItemsSource = meldingen;
        }

        // ◀️ Vorige maand
        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddMonths(-1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        // ▶️ Volgende maand
        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddMonths(1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        // ◀️◀️ Vorig jaar
        private void PrevYear_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddYears(-1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }

        // ▶️▶️ Volgend jaar
        private void NextYear_Click(object sender, RoutedEventArgs e)
        {
            _huidigeDatum = _huidigeDatum.AddYears(1);
            Kalender.SetDisplayDate(_huidigeDatum);
            UpdateKalenderHeader();
        }




    }
}
