using Barroc_Intense.Data;
using Barroc_Intense.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace Barroc_Intense
{
    public sealed partial class MeldingBewerkenPage : Page
    {
        private readonly AppDbContext _db = new();
        private Melding _melding;

        public MeldingBewerkenPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = (int)e.Parameter;

            _melding = _db.Meldingen.First(m => m.Id == id);

            // Vul tekstvelden met bestaande waarden
            AfdelingBox.Text = _melding.Afdeling;
            KlantBox.Text = _melding.Klant;
            ProductBox.Text = _melding.Product;
            ProbleemBox.Text = _melding.Probleemomschrijving;
            StatusCombo.SelectedIndex = 0;

            //  datum/tijd instellen, gebruik huidige datum als er geen datum in de DB staat
            if (_melding.Datum.HasValue)
            {
                DatumPicker.Date = _melding.Datum.Value;
                TijdPicker.Time = _melding.Datum.Value.TimeOfDay;
            }
            else
            {
                DatumPicker.Date = DateTime.Now;
                TijdPicker.Time = DateTime.Now.TimeOfDay;
            }
        }

        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            //  update object met nieuwe waarden uit UI
            _melding.Klant = KlantBox.Text;
            _melding.Product = ProductBox.Text;
            _melding.Probleemomschrijving = ProbleemBox.Text;
            _melding.Status = ((ComboBoxItem)StatusCombo.SelectedItem).Content.ToString();

            //  combineer datum en tijd van pickers tot één DateTime
            var nieuweDatum = DatumPicker.Date.Date + TijdPicker.Time;
            _melding.Datum = nieuweDatum;

            _db.SaveChanges();

            //  toon feedback aan gebruiker
            await new ContentDialog
            {
                Title = "Succes",
                Content = $"Melding opgeslagen op {nieuweDatum:dd MMM yyyy HH:mm}.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();

            Frame.Navigate(typeof(MaintenanceMelding));
        }
    }
}