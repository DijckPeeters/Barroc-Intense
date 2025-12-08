using Barroc_Intense.Data;
using Barroc_Intense.Pages;
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

            AfdelingBox.Text = _melding.Afdeling;
            MonteurBox.Text = _melding.MonteurId;
            KlantBox.Text = _melding.Klant;
            ProductBox.Text = _melding.Product;
            ProbleemBox.Text = _melding.Probleemomschrijving;
            StatusCombo.SelectedIndex = 0;
        }

        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            _melding.Afdeling = AfdelingBox.Text;
            _melding.MonteurId = MonteurBox.Text;
            _melding.Klant = KlantBox.Text;
            _melding.Product = ProductBox.Text;
            _melding.Probleemomschrijving = ProbleemBox.Text;
            _melding.Status = ((ComboBoxItem)StatusCombo.SelectedItem).Content.ToString();

            _db.SaveChanges();

            await new ContentDialog
            {
                Title = "Succes",
                Content = "Melding bijgewerkt.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();

            Frame.Navigate(typeof(MaintenancePagee));
        }
    }
}