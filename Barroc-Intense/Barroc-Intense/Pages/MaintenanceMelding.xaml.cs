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
    public sealed partial class MaintenanceMelding : Page
    {
        private AppDbContext _db = new AppDbContext();
        private DateTime _huidigeDatum = DateTime.Today;
        private List<Melding> _alleMeldingen;

        public MaintenanceMelding()
        {
            this.InitializeComponent();
            _db.Database.EnsureCreated();

            //  Laad alle meldingen bij openen van de pagina
            LaadMeldingen();
        }

        private void LaadMeldingen()
        {
            //  Haal meldingen op met machine & delivery info
            MaintenanceListView.ItemsSource = _db.Meldingen
                .Include(m => m.Machine)
                .Include(m => m.Delivery)
                .OrderByDescending(m => m.Datum)
                .ToList();
        }

        //  Zoeken/filteren van meldingen op klant, product of afdeling
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

        //  Reset zoekveld en herlaad alle meldingen
        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LaadMeldingen();
        }

        //  Opslaan van afgeronde keuringen en opnieuw inplannen voor volgende maand
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

        //  Navigatie naar Klantenservice pagina
        private void KlantButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KlantenservicePage));
        }

        //  Open formulierpagina voor specifieke melding
        private void OpenForm_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Tag == null) return;

            int id = (int)button.Tag;
            Frame.Navigate(typeof(FormulierPage), id);
        }

        //  Verwijderen van een melding met bevestiging
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

        //  Navigatie naar bewerkpagina van een melding
        private void BewerkMelding_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int id = (int)button.Tag;
            Frame.Navigate(typeof(MeldingBewerkenPage), id);
        }

        //  Terug naar MaintenanceDashboard
        private void BackToMaintenanceDashboard(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MaintenanceDashboard));
        }
    }
}