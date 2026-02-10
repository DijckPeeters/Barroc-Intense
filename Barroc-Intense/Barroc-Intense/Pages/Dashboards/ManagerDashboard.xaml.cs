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
    using System.Collections.ObjectModel;
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
        /// NOTE: The XAML's x:Class is "Barroc_Intense.Pages.Dashboards.ManagerDashboard"
        /// so this partial class name must match that exactly.
        /// </summary>
        public sealed partial class ManagerDashboard : Page
        {
            public ObservableCollection<Employee> Employees { get; set; }

            public ManagerDashboard()
            {
                this.InitializeComponent();

                Employees = new ObservableCollection<Employee>
                {
                    new Employee { Id = 1, Username = "admin", Role = "Manager" }
                };

                EmployeeList.ItemsSource = Employees;

                CheckWarnings();
            }

            // ------------------------------
            // Dashboard navigatie
            // ------------------------------
            private void OpenInkoop_Click(object sender, RoutedEventArgs e)
            {
                // InkoopDashboard class is defined as InkoopDashboard (no "Page" suffix)
                Frame.Navigate(typeof(InkoopDashBoard));
            }

            private void OpenSales_Click(object sender, RoutedEventArgs e)
            {
                Frame.Navigate(typeof(SalesDashboard));
            }

            private void OpenFinance_Click(object sender, RoutedEventArgs e)
            {
                Frame.Navigate(typeof(FinanceDashboard));
            }

            private void OpenMaintenance_Click(object sender, RoutedEventArgs e)
            {
                // The maintenance page class is MaintenancePagee in Barroc_Intense.Pages
                Frame.Navigate(typeof(MaintenanceDashboard));
            }

            private void OpenCustomer_Click(object sender, RoutedEventArgs e)
            {
                // Navigate to the klantenservice page
                Frame.Navigate(typeof(KlantenservicePage));
            }

            // ------------------------------
            // Rapportages
            // ------------------------------
            private void ViewReport_Click(object sender, RoutedEventArgs e)
            {
                if (DepartmentReportSelector.SelectedItem is ComboBoxItem item)
                {
                    ReportOutput.Text = $"{item.Content} rapportage:\n" +
                        "- Activiteit: stabiel\n" +
                        "- Voortgang projecten: 70%\n" +
                        "- Medewerkerprestaties: voldoende\n";
                }
            }

            // ------------------------------
            // Medewerkersbeheer
            // ------------------------------
            private void AddEmployee_Click(object sender, RoutedEventArgs e)
            {
                if (string.IsNullOrWhiteSpace(NewUsername.Text) || NewRole.SelectedItem == null)
                    return;

                var selectedRoleItem = NewRole.SelectedItem as ComboBoxItem;
                var roleContent = selectedRoleItem?.Content?.ToString() ?? string.Empty;

                Employees.Add(new Employee
                {
                    Id = Employees.Count + 1,
                    Username = NewUsername.Text,
                    Role = roleContent
                });

                NewUsername.Text = "";
                NewRole.SelectedItem = null;
            }

            private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
            {
                if (EmployeeList.SelectedItem is Employee selected)
                {
                    Employees.Remove(selected);
                }
            }

            // ------------------------------
            // Alerts systeem
            // ------------------------------
            private void CheckWarnings()
            {
                // Dummy warnings
                bool voorraadTekort = true;
                bool bkrAfkeuring = false;

                if (voorraadTekort)
                    AlertsBlock.Text = "Voorraadtekort bij Inkoop";

                if (bkrAfkeuring)
                    AlertsBlock.Text += "\nBKR-afkeuring bij Finance";
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session.IsLoggedIn = false;
            Session.Username = null;

            Frame.Navigate(typeof(LoginPage));
        }
    }
    }