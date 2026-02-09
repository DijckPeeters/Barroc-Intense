using Barroc_Intense.Data;
using Microsoft.UI;
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
using Windows.Storage;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SalesDashboard : Page
    {
        public SalesDashboard()
        {
            InitializeComponent();
        }

        private void CalculateQuote_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuoteQuantity.Text, out int quantity) &&
                double.TryParse(QuotePricePerUnit.Text, out double price))
            {
                double total = quantity * price;

                QuoteResult.Text =
                    $"Offerte voor {QuoteClient.Text}:\n" +
                    $"{QuoteProduct.Text} x {quantity}\n\n" +
                    $"Totaalprijs: €{total:0.00}";
            }
            else
            {
                QuoteResult.Text = "Voer geldige getallen in!";
            }
        }
        // Using a simple text file instead of database for quick setup and simplicity with small data.
        private async void SaveProspect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProspectCompany.Text) ||
                string.IsNullOrWhiteSpace(ProspectNote.Text))
            {
                ProspectMessage.Text = "Vul alle velden in!";
                ProspectMessage.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            string company = ProspectCompany.Text;
            string note = ProspectNote.Text;
            string fileName = $"{company}.txt";


            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(
                fileName,
                CreationCollisionOption.OpenIfExists
                );

            string line = $"{DateTime.Now} | {company} | {note} \n";

            await FileIO.AppendTextAsync(file, line);

            
            string folderPath = ApplicationData.Current.LocalFolder.Path;
            ProspectMessage.Text = $"Notitie opgeslagen in:\n{folderPath}";
            ProspectMessage.Foreground = new SolidColorBrush(Colors.Green);

            ProspectNote.Text = "";
            ProspectCompany.Text = "";

            // Opens the file after saving
            await Launcher.LaunchFileAsync(file);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Session.IsLoggedIn = false;
            Session.Username = null;

            Frame.Navigate(typeof(LoginPage));
        }
    }
}
