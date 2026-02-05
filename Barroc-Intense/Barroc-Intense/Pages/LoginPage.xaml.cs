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
        //  Dummy employees lijst voor login (in echte app vervangbaar door DB)
        private readonly List<Employee> _employees = new()
        {
            new Employee { Id = 1, Username = "sarah", Password = "1234", Role = "Sales" },
            new Employee { Id = 2, Username = "john", Password = "1234", Role = "Inkoop" },
            new Employee { Id = 3, Username = "emma", Role = "Finance" }, 
            new Employee { Id = 4, Username = "mike", Role = "Maintenance" },
            new Employee { Id = 5, Username = "anna", Role = "Klantenservice" },
            new Employee { Id = 6, Username = "marc", Password = "1234", Role = "Manager" }
        };

        //  Mapping van rollen naar de juiste dashboard pagina
        private readonly Dictionary<string, Type> dashboards = new Dictionary<string, Type>
        {
            { "Inkoop", typeof(InkoopDashBoard) },
            { "Sales", typeof(SalesDashboard) },
            { "Finance", typeof(FinanceDashboard) },
            { "Maintenance", typeof(MaintenanceDashboard) },
            { "Klantenservice", typeof(KlantenservicePage) },
            { "Manager", typeof(ManagerDashboard) }
        };

        public LoginPage()
        {
            this.InitializeComponent();
        }

        //  Login knop logica
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var errors = new List<string>();

            //  Validatie van lege velden
            if (string.IsNullOrEmpty(UsernameBox.Text))
                errors.Add("Name is required");

            if (string.IsNullOrEmpty(PasswordBox.Password))
                errors.Add("Password is required");

            if (errors.Count > 0)
            {
                errorsTextBlock.Text = string.Join(Environment.NewLine, errors);
                return;
            }

            errorsTextBlock.Text = "Validation succeded!";

            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            //  Controleer of gebruiker bestaat in _employees lijst
            var employee = _employees.FirstOrDefault(e => e.Username.Equals(user, StringComparison.OrdinalIgnoreCase)
                                                       && e.Password == pass);

            if (employee == null)
            {
                errorsTextBlock.Text = "Invalid username or password!";
                return;
            }

            //  Navigatie naar juiste dashboard op basis van rol
            if (dashboards.TryGetValue(employee.Role, out Type dashboardPage))
            {
                Frame.Navigate(dashboardPage); //  Navigeren naar dashboard
            }
            else
            {
                errorsTextBlock.Text = "No dashboard found for your role!";
            }
        }
    }
}