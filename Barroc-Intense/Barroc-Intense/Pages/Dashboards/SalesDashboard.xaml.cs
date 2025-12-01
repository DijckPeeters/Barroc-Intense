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

        private void SaveProspect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProspectCompany.Text) ||
                string.IsNullOrWhiteSpace(ProspectNote.Text))
            {
                ProspectMessage.Text = "Vul alle velden in!";
                ProspectMessage.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            // Later add database logic here
            ProspectMessage.Text = $"Notitie opgeslagen voor {ProspectCompany.Text}.";
            ProspectMessage.Foreground = new SolidColorBrush(Colors.Green);

            ProspectNote.Text = "";
        }
    }
}
