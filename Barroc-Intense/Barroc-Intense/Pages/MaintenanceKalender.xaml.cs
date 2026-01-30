using Barroc_Intense.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barroc_Intense.Pages;

public sealed partial class MaintenanceKalender : Page
{
    private AppDbContext _db = new AppDbContext();
    private DateTime _huidigeDatum = DateTime.Today;
    private List<Melding> _alleMeldingen;

    public MaintenanceKalender()
    {
        this.InitializeComponent();
        LaadMeldingen();
        LaadAlleMeldingenVoorKalender();
        UpdateKalenderHeader();
    }

    // ================== DATA ==================
    private void LaadMeldingen()
    {
        DagMeldingenControl.ItemsSource = _db.Meldingen
            .Include(m => m.Machine)
            .Include(m => m.Delivery)
            .OrderByDescending(m => m.Datum)
            .ToList();
    }

    private void LaadAlleMeldingenVoorKalender()
    {
        _alleMeldingen = _db.Meldingen.ToList();
        Kalender.SelectedDates.Clear();
        Kalender.SetDisplayDate(_huidigeDatum);
    }

    private void UpdateKalenderHeader()
    {
        MonthYearText.Text = _huidigeDatum.ToString("MMMM yyyy");
    }

    // ================== KALENDER VISUALS ==================
    private void Kalender_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
    {
        if (args.Item == null) return;

        DateTime day = args.Item.Date.Date;

        bool heeftMeldingen = _alleMeldingen != null &&
            _alleMeldingen.Any(m => m.Datum.HasValue && m.Datum.Value.Date == day);

        if (heeftMeldingen)
        {
            // Rand
            args.Item.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
            args.Item.BorderThickness = new Thickness(2);

            // Achtergrond highlight
            args.Item.Background = new SolidColorBrush(
                Microsoft.UI.ColorHelper.FromArgb(80, 255, 215, 0));

            // Rond effect
            try
            {
                args.Item.CornerRadius = new CornerRadius(20);
            }
            catch { }
        }
        else
        {
            args.Item.BorderBrush = null;
            args.Item.BorderThickness = new Thickness(0);
            args.Item.Background = null;
            try
            {
                args.Item.CornerRadius = new CornerRadius(0);
            }
            catch { }
        }
    }

    // ================== SELECTIE ==================
    private void Kalender_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        if (sender.SelectedDates.Count > 0)
        {
            LaadMeldingenVoorDatum(sender.SelectedDates[0].Date);
        }
    }

    private void LaadMeldingenVoorDatum(DateTime date)
    {
        DagMeldingenControl.ItemsSource = _db.Meldingen
            .Where(m => m.Datum.HasValue && m.Datum.Value.Date == date.Date)
            .ToList();
    }

    // ================== NAVIGATIE ==================
    private void PrevMonth_Click(object sender, RoutedEventArgs e)
    {
        _huidigeDatum = _huidigeDatum.AddMonths(-1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void NextMonth_Click(object sender, RoutedEventArgs e)
    {
        _huidigeDatum = _huidigeDatum.AddMonths(1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void PrevYear_Click(object sender, RoutedEventArgs e)
    {
        _huidigeDatum = _huidigeDatum.AddYears(-1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void NextYear_Click(object sender, RoutedEventArgs e)
    {
        _huidigeDatum = _huidigeDatum.AddYears(1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void BackToMaintenanceDashboard(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceDashboard));
    }
}
