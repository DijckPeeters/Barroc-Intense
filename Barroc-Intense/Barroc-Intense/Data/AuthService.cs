using Barroc_Intense.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intense.Data
{
    public static class AuthService
    {
        public static void Logout(Frame frame)
        {
            Session.IsLoggedIn = false;
            Session.Username = null;

            frame.Navigate(typeof(LoginPage));
        }
    }
}
