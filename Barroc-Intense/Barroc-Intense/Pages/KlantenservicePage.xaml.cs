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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KlantenservicePage : Page
    {
        // Lijst van meldingen (bijvoorbeeld gedeeld met MaintenancePage)
        public static List<Melding> AlleMeldingen { get; set; } = new List<Melding>();

        private int _volgendeId = 1; // Automatisch ID bijhouden

        public KlantenservicePage()
        {
            this.InitializeComponent();
            using var db = new AppDbContext();
        
            db.Database.EnsureCreated();
        }

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            var melding = new Melding
            {
                Afdeling = AfdelingTextBox.Text,
                Klant = KlantTextBox.Text,
                Product = ProductTextBox.Text,
                Probleemomschrijving = ProbleemTextBox.Text,
                Status = ((ComboBoxItem)StatusComboBox.SelectedItem)?.Content.ToString() ?? "Open",
                Datum = DateTime.Now,
                IsOpgelost = false
            };

            using var db = new AppDbContext();
            db.Meldingen.Add(melding);
            db.SaveChanges(); // <- hierdoor wordt de melding opgeslagen in de database

            // Leeg velden
            AfdelingTextBox.Text = "";
            KlantTextBox.Text = "";
            ProductTextBox.Text = "";
            ProbleemTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;

            // Feedback
            var dialog = new ContentDialog
            {
                Title = "Gelukt",
                Content = "Melding is toegevoegd aan de database.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}