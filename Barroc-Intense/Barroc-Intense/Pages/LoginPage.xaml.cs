using Barroc_Intense.Data;
using Barroc_Intense.Pages.Dashboards;
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
    public sealed partial class LoginPage : Page
    {
        private readonly AppDbContext _context = new();

        private readonly Dictionary<string, Type> dashboards = new()
        {
            { "Inkoop", typeof(InkoopDashBoard) },
            { "Sales", typeof(SalesDashboard) },
            { "Finance", typeof(FinanceDashboard) },
            { "Maintenance", typeof(MaintenancePagee) },
            { "Klantenservice", typeof(KlantenservicePage) },
            { "Manager", typeof(ManagerDashboard) }
        };

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                errorsTextBlock.Text = "Gebruikersnaam en wachtwoord zijn verplicht";
                return;
            }

            var employee = _context.Employees
                .FirstOrDefault(e =>
                    e.Username == UsernameBox.Text &&
                    e.Password == PasswordBox.Password);

            if (employee == null)
            {
                errorsTextBlock.Text = "Ongeldige login gegevens";
                return;
            }

            if (dashboards.TryGetValue(employee.Role, out Type dashboard))
            {
                Frame.Navigate(dashboard);
            }
            else
            {
                errorsTextBlock.Text = "Geen dashboard voor deze rol";
            }
        }
    }
}
