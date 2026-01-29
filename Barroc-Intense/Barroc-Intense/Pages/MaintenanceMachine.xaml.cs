using Barroc_Intense.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace Barroc_Intense.Pages
{
    public sealed partial class MaintenanceMachine : Page
    {
        private AppDbContext _db = new AppDbContext();

        public MaintenanceMachine()
        {
            InitializeComponent();
            LaadMachines();
        }

        private void LaadMachines()
        {
            MachinesListView.ItemsSource = _db.Machines
                .Include(m => m.Deliveries)
                .ToList();
        }

        // ========== BUTTONS ==========

       
        private void Meldingen_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int id = (int)button.Tag;

            // 👉 navigeer naar meldingen gefilterd op machine
            Frame.Navigate(typeof(MaintenanceMelding), id);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MaintenanceDashboard));
        }
    }
}
