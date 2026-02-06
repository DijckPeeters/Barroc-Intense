using Barroc_Intense.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class KlantenservicePage : Page
    {
        public KlantenservicePage()
        {
            this.InitializeComponent();

            // Standaard geselecteerde items
            StatusComboBox.SelectedIndex = 0;
            PrioriteitCombo.SelectedIndex = 0;

            // Database aanmaken indien nodig
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }

        // Helper dialog
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

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            DateTime? gekozenDatum = null;

            // ==========================
            // Datum/Tijd validatie
            // ==========================
            if (GeenDatumCheckBox.IsChecked != true)
            {
                try
                {
                    var dateOffset = DatumPicker.Date;
                    var timeSpan = TijdPicker.Time;
                    gekozenDatum = dateOffset.Date + timeSpan;

                    if (gekozenDatum < DateTime.Now.AddYears(-5) || gekozenDatum > DateTime.Now.AddYears(5))
                    {
                        ToonDialog("Fout", "Kies een geldige datum en tijd of vink 'Geen datum/tijd' aan.");
                        return;
                    }
                }
                catch
                {
                    ToonDialog("Fout", "Kies een geldige datum en tijd of vink 'Geen datum/tijd' aan.");
                    return;
                }
            }

            // ==========================
            // Tekst validatie
            // ==========================
            if (string.IsNullOrWhiteSpace(AfdelingTextBox.Text))
            {
                ToonDialog("Validatiefout", "Afdeling is verplicht.");
                return;
            }

            if (string.IsNullOrWhiteSpace(KlantTextBox.Text))
            {
                ToonDialog("Validatiefout", "Klant is verplicht.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ProbleemTextBox.Text) || ProbleemTextBox.Text.Length < 5)
            {
                ToonDialog("Validatiefout", "Probleemomschrijving moet minimaal 5 tekens bevatten.");
                return;
            }

            // ==========================
            // Nummerieke validatie
            // ==========================
            if (!int.TryParse(MachineTextBox.Text, out int machineId) || machineId <= 0)
            {
                ToonDialog("Validatiefout", "Machine ID moet een geldig getal zijn.");
                return;
            }

            if (!int.TryParse(MonteurTextBox.Text, out int monteurId) || monteurId <= 0)
            {
                ToonDialog("Validatiefout", "Monteur ID moet een geldig getal zijn.");
                return;
            }

            // ==========================
            // ComboBox validatie
            // ==========================
            if (StatusComboBox.SelectedItem == null)
            {
                ToonDialog("Validatiefout", "Selecteer een status.");
                return;
            }

            if (PrioriteitCombo.SelectedItem == null)
            {
                ToonDialog("Validatiefout", "Selecteer een prioriteit.");
                return;
            }

            using var db = new AppDbContext();

            // ==========================
            // Dubbele datum check
            // ==========================
            if (gekozenDatum.HasValue)
            {
                bool bestaatAl = db.Meldingen.Any(m => m.Datum == gekozenDatum.Value);
                if (bestaatAl)
                {
                    ToonDialog("Validatiefout", "Er bestaat al een melding op exact dezelfde datum en tijd.");
                    return;
                }
            }

            // ==========================
            // Opslaan
            // ==========================
            var melding = new Melding
            {
                MachineId = machineId,
                MonteurId = monteurId,
                Prioriteit = ((ComboBoxItem)PrioriteitCombo.SelectedItem).Content.ToString(),
                Afdeling = AfdelingTextBox.Text.Trim(),
                Klant = KlantTextBox.Text.Trim(),
                Product = ProductTextBox.Text.Trim(),
                Probleemomschrijving = ProbleemTextBox.Text.Trim(),
                Status = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString(),
                Datum = gekozenDatum,
                IsOpgelost = false
            };

            db.Meldingen.Add(melding);
            db.SaveChanges();

            ToonDialog("Gelukt", "Melding is succesvol opgeslagen.");

            // ==========================
            // Reset velden
            // ==========================
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

        private void MaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MaintenanceMelding));
        }
    }
}
