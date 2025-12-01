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
    public sealed partial class FinanceDashboard : Page
    {
        private List<LeaseContract> _contracts = new();

        public FinanceDashboard()
        {
            this.InitializeComponent();
            LoadContracts();
            DisplayContracts();
            UpdateTotalIncome();
        }

        private void LoadContracts()
        {
            // Simpele demo-contracten
            _contracts = new List<LeaseContract>
            {
                new LeaseContract("Bedrijf A", 1200, true, PaymentType.Monthly, false),
                new LeaseContract("Bedrijf B", 3600, true, PaymentType.Quarterly, true),
                new LeaseContract("Bedrijf C", 1500, false, PaymentType.Monthly, false),
                new LeaseContract("Bedrijf D", 4800, true, PaymentType.Quarterly, false)
            };
        }

        private void DisplayContracts()
        {
            ContractListPanel.Children.Clear();

            foreach (var contract in _contracts)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 15 };

                var nameText = new TextBlock
                {
                    Text = contract.CompanyName,
                    Width = 200,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18
                };

                var amountText = new TextBlock
                {
                    Text = $"€{contract.Amount:0.00}",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18
                };

                var paymentText = new TextBlock
                {
                    Text = contract.PaymentType == PaymentType.Monthly ? "Maandelijks" : "Periodiek",
                    Width = 120,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18
                };

                var bkrText = new TextBlock
                {
                    Text = contract.BKRChecked ? "BKR OK" : "BKR NIET GEKONTROLEERD",
                    Width = 180,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(
                        contract.BKRChecked ? Colors.Green : Colors.Red
                    )
                };

                var paidButton = new Button
                {
                    Content = contract.Paid ? "Betaald" : "Markeer als betaald",
                    Width = 180,
                    Height = 40,
                    Tag = contract
                };
                paidButton.Click += PaidButton_Click;

                row.Children.Add(nameText);
                row.Children.Add(amountText);
                row.Children.Add(paymentText);
                row.Children.Add(bkrText);
                row.Children.Add(paidButton);

                ContractListPanel.Children.Add(row);
            }
        }

        private void PaidButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LeaseContract contract)
            {
                contract.Paid = true;
                DisplayContracts();
                UpdateTotalIncome();
            }
        }

        private void UpdateTotalIncome()
        {
            double total = _contracts
                .Where(c => c.Paid)
                .Sum(c => c.Amount);

            TotalIncomeText.Text = $"Totale inkomsten: €{total:0.00}";
        }
    }

    public enum PaymentType
    {
        Monthly,
        Quarterly
    }

    public class LeaseContract
    {
        public string CompanyName { get; }
        public double Amount { get; }
        public bool BKRChecked { get; }
        public PaymentType PaymentType { get; }
        public bool Paid { get; set; }

        public LeaseContract(string companyName, double amount, bool bkrChecked, PaymentType paymentType, bool paid)
        {
            CompanyName = companyName;
            Amount = amount;
            BKRChecked = bkrChecked;
            PaymentType = paymentType;
            Paid = paid;
        }
    }
}
