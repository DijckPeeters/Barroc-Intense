using Barroc_Intense.Data;
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

namespace Barroc_Intense.Pages
{
    public sealed partial class KlantenservicePage : Page
    {
        public KlantenservicePage()
        {
            this.InitializeComponent();

            //  Zet standaard geselecteerde items voor comboboxen
            StatusComboBox.SelectedIndex = 0;
            PrioriteitCombo.SelectedIndex = 0;

            //  Zorg dat de database bestaat bij eerste keer gebruik
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }

        //  Helper functie voor pop-up dialogen
        private async void ToonDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        //  Toevoegen van een nieuwe melding aan de database
        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            DateTime? gekozenDatum = null;

            //  Controleer of de gebruiker geen datum/tijd wil invullen
            if (GeenDatumCheckBox.IsChecked != true)
            {
                try
                {
                    //  Combineer DatePicker en TimePicker tot één DateTime
                    var dateOffset = DatumPicker.Date;
                    var timeSpan = TijdPicker.Time;
                    gekozenDatum = dateOffset.Date + timeSpan;
                }
                catch
                {
                    ToonDialog("Fout", "Kies een geldige datum en tijd of vink 'Geen datum/tijd' aan.");
                    return;
                }
            }

            //  Basisvalidatie van verplichte velden
            if (string.IsNullOrWhiteSpace(AfdelingTextBox.Text) ||
                string.IsNullOrWhiteSpace(KlantTextBox.Text))
            {
                ToonDialog("Fout", "Vul minimaal Afdeling en Klant in.");
                return;
            }

            using var db = new AppDbContext();

            //  Controleer dat er niet al een melding bestaat op exact dezelfde datum/tijd
            if (gekozenDatum.HasValue)
            {
                bool bestaatAl = db.Meldingen.Any(m => m.Datum == gekozenDatum.Value);
                if (bestaatAl)
                {
                    ToonDialog("Fout", "Er bestaat al een melding op exact dezelfde datum en tijd.");
                    return;
                }
            }

            //  Maak de nieuwe melding aan en vul alle velden vanuit de UI
            var melding = new Melding
            {
                MachineId = int.Parse(MachineTextBox.Text),
                MonteurId = int.Parse(MonteurTextBox.Text),
                Prioriteit = ((ComboBoxItem)PrioriteitCombo.SelectedItem)?.Content.ToString() ?? "Laag",
                Afdeling = AfdelingTextBox.Text,
                Klant = KlantTextBox.Text,
                Product = ProductTextBox.Text,
                Probleemomschrijving = ProbleemTextBox.Text,
                Status = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Content.ToString() ?? "Open",
                Datum = gekozenDatum,
                IsOpgelost = false
            };

            db.Meldingen.Add(melding);
            db.SaveChanges();

            ToonDialog("Gelukt", "Melding is toegevoegd aan de database.");

            //  Reset alle velden na succesvol opslaan
            AfdelingTextBox.Text = "";
            MonteurTextBox.Text = "";
            KlantTextBox.Text = "";
            ProductTextBox.Text = "";
            ProbleemTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;
            PrioriteitCombo.SelectedIndex = 0;
            MachineTextBox.Text = "";
            DatumPicker.Date = DateTimeOffset.Now;
            TijdPicker.Time = TimeSpan.Zero;
            GeenDatumCheckBox.IsChecked = false;
        }

        //  Navigatie naar MaintenanceMelding pagina
        private void MaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MaintenanceMelding));
        }
    }
}