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

namespace Barroc_Intense.Pages 
{
        public sealed partial class LoginPage : Page
        {
            public LoginPage()
            {
                this.InitializeComponent();
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(UsernameBox.Text))
            {
                errors.Add("Name is required");
            }

            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                errors.Add("Password is required");
            }

            if (errors.Count > 0)
            {
                errorsTextBlock.Text = string.Join(Environment.NewLine, errors);
                return;
            }

            errorsTextBlock.Text = "Validation succeded!";

            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;


        } 

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainWindow));
        }
    }

}
