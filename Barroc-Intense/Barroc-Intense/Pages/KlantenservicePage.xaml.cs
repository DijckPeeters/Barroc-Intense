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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages
{

        public sealed partial class KlantenservicePage : Page
        {
            public KlantenservicePage()
            {
                this.InitializeComponent();

                // Zorg dat comboboxen een standaard selected hebben
                StatusComboBox.SelectedIndex = 0;
                PrioriteitCombo.SelectedIndex = 0;

                using var db = new AppDbContext();
                db.Database.EnsureCreated();
            }

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
                // combineer datum en tijd
                // DatePicker.Date is DateTimeOffset, TimePicker.Time is TimeSpan
                DateTime gekozenDatum;

                try
                {
                    var dateOffset = DatumPicker.Date;
                    var timeSpan = TijdPicker.Time;

                    // dateOffset.Date is DateTime (00:00:00 time)
                    gekozenDatum = dateOffset.Date + timeSpan;
                }
                catch
                {
                    ToonDialog("Fout", "Kies een geldige datum en tijd.");
                    return;
                }

                // eenvoudige validatie (optioneel kun je uitgebreidere checks doen)
                if (string.IsNullOrWhiteSpace(AfdelingTextBox.Text) ||
                    string.IsNullOrWhiteSpace(KlantTextBox.Text))
                {
                    ToonDialog("Fout", "Vul minimaal Afdeling en Klant in.");
                    return;
                }

                using var db = new AppDbContext();

                // controle op exact dezelfde datum + tijd
                bool bestaatAl = db.Meldingen.Any(m => m.Datum == gekozenDatum);

                if (bestaatAl)
                {
                    ToonDialog("Fout", "Er bestaat al een melding op exact dezelfde datum en tijd.");
                    return;
                }

                var melding = new Melding
                {
                    MachineId = MachineTextBox.Text ?? string.Empty,
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

                // velden legen
                AfdelingTextBox.Text = "";
                KlantTextBox.Text = "";
                ProductTextBox.Text = "";
                ProbleemTextBox.Text = "";
                StatusComboBox.SelectedIndex = 0;
                PrioriteitCombo.SelectedIndex = 0;
                MachineTextBox.Text = "";
                // reset date/time naar nu (optioneel)
                DatumPicker.Date = DateTimeOffset.Now;
                TijdPicker.Time = DateTime.Now.TimeOfDay;

            }

        private void MaintenanceButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MaintenancePagee));
        }

    }
    }