using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intense.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MaintenanceDashboard : Page
{
    public MaintenanceDashboard()
    {
        this.InitializeComponent();
    }

    private void OpenMeldingen_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceMelding));
    }

    private void OpenMachines_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceMachine));
    }

    private void OpenLogboek_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceLogboek));
    }

    private void OpenKalender_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceKalender));
    }
}
