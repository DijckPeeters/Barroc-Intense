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



namespace Barroc_Intense.Pages;


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

   

    private void OpenKalender_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceKalender));
    }
}
