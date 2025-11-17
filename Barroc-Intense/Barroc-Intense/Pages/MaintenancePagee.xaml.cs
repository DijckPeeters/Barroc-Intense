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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static Barroc_Intense.Data.AppDbContext;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



namespace Barroc_Intense.Pages
{
    public sealed partial class MaintenancePagee : Page
    {
        // Context die blijft bestaan
        private AppDbContext _db = new AppDbContext();

        // Lijst die is gekoppeld aan de database (tracked)
        private List<Melding> _alleMeldingen;

        public MaintenancePagee()
        {
            this.InitializeComponent();

            // Laad meldingen één keer (database blijft bestaan)
            _alleMeldingen = _db.Meldingen.ToList();
            MaintenanceListView.ItemsSource = _alleMeldingen;
        }

      

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            string zoekterm = SearchBox.Text.ToLower();

            var gefilterd = _db.Meldingen
                .Where(m => m.Klant.ToLower().Contains(zoekterm) ||
                            m.Product.ToLower().Contains(zoekterm))
                .ToList();

            MaintenanceListView.ItemsSource = gefilterd;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            // Update van meldingen met IsOpgelost = True eerst opslaan
            _db.SaveChanges();

            // Zoek opgeloste meldingen
            var opgeloste = _db.Meldingen.Where(m => m.IsOpgelost).ToList();

            if (opgeloste.Count > 0)
            {
                _db.Meldingen.RemoveRange(opgeloste);
                _db.SaveChanges();
            }

            // Refresh lijst
            _alleMeldingen = _db.Meldingen.ToList();
            MaintenanceListView.ItemsSource = _alleMeldingen;

            var dialog = new ContentDialog
            {
                Title = "Opgeslagen",
                Content = "Opgeloste meldingen zijn verwijderd.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
