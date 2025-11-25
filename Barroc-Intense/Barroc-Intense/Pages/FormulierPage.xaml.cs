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
    public sealed partial class FormulierPage : Page
    {
        private Melding _melding;

        public FormulierPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = (int)e.Parameter;

            using var db = new AppDbContext();
            _melding = db.Meldingen.First(x => x.Id == id);

            TitleBlock.Text = _melding.IsKeuring ? "Keuring" : "Melding";

            if (_melding.IsKeuring)
            {
                MeldingPanel.Visibility = Visibility.Collapsed;

                ChecklistBox.IsChecked = _melding.ChecklistVolledig;
                GoedgekeurdBox.IsChecked = _melding.KeuringGoedgekeurd;
                KeuringOpmerkingenBox.Text = _melding.KeuringOpmerkingen;
            }
            else
            {
                KeuringPanel.Visibility = Visibility.Collapsed;

                KlantBox.Text = _melding.Klant;
                DatumBox.Text = _melding.Datum.ToString("dd-MM-yyyy HH:mm");
                InitieleBox.Text = _melding.Probleemomschrijving;
                MachineBox.Text = _melding.Product;

                StoringscodeBox.Text = _melding.Storingscode;
                VerholpenBox.IsChecked = _melding.StoringVerholpen;
                VervolgBox.Text = _melding.Vervolgafspraak;
                BeschrijvingBox.Text = _melding.KorteBeschrijving;
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var m = db.Meldingen.First(x => x.Id == _melding.Id);

            if (m.IsKeuring)
            {
                m.ChecklistVolledig = ChecklistBox.IsChecked ?? false;
                m.KeuringGoedgekeurd = GoedgekeurdBox.IsChecked ?? false;
                m.KeuringOpmerkingen = KeuringOpmerkingenBox.Text;
            }
            else
            {
                m.Storingscode = StoringscodeBox.Text;
                m.StoringVerholpen = VerholpenBox.IsChecked ?? false;
                m.Vervolgafspraak = VervolgBox.Text;
                m.KorteBeschrijving = BeschrijvingBox.Text;
            }

            // FIXED → SelectedItems geeft DIRECT objecten
            var onderdelen = OnderdelenList.SelectedItems
                .Select(i => i?.ToString());

            m.GebruikteOnderdelen = string.Join(",", onderdelen);

            db.SaveChanges();

            // FIXED → handle no Frame
            if (Frame != null && Frame.CanGoBack)
                Frame.GoBack();
        }

        private void Handtekening_Click(object sender, RoutedEventArgs e)
        {
            // later kan je InkCanvas toevoegen
        }
    }
}
