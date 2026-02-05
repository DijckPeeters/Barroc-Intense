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

    private void LaadMeldingen()
    {
        //  Haal alle meldingen op uit de DB inclusief gerelateerde Machine & Delivery
        DagMeldingenControl.ItemsSource = _db.Meldingen
            .Include(m => m.Machine)
            .Include(m => m.Delivery)
            .OrderByDescending(m => m.Datum)
            .ToList();
    }

    private void LaadAlleMeldingenVoorKalender()
    {
        //  Alle meldingen opslaan in een lijst om kalender te highlighten
        _alleMeldingen = _db.Meldingen.ToList();
        Kalender.SelectedDates.Clear();
        Kalender.SetDisplayDate(_huidigeDatum);
    }

    private void UpdateKalenderHeader()
    {
        //  Maand en jaar tonen boven de kalender
        MonthYearText.Text = _huidigeDatum.ToString("MMMM yyyy");
    }

    private void Kalender_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
    {
        if (args.Item == null) return;

        DateTime day = args.Item.Date.Date;

        //  Controleer of er meldingen zijn op deze dag
        bool heeftMeldingen = _alleMeldingen != null &&
            _alleMeldingen.Any(m => m.Datum.HasValue && m.Datum.Value.Date == day);

        if (heeftMeldingen)
        {
            //  Highlight dag met een rode rand en gouden achtergrond
            args.Item.BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
            args.Item.BorderThickness = new Thickness(2);
            args.Item.Background = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(80, 255, 215, 0));
            try { args.Item.CornerRadius = new CornerRadius(20); } catch { }
        }
        else
        {
            //  Reset styling als er geen meldingen zijn
            args.Item.BorderBrush = null;
            args.Item.BorderThickness = new Thickness(0);
            args.Item.Background = null;
            try { args.Item.CornerRadius = new CornerRadius(0); } catch { }
        }
    }

    private void Kalender_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        //  Als een datum geselecteerd is, laad meldingen voor die dag
        if (sender.SelectedDates.Count > 0)
        {
            LaadMeldingenVoorDatum(sender.SelectedDates[0].Date);
        }
    }

    private void LaadMeldingenVoorDatum(DateTime date)
    {
        //  Filter meldingen op exacte geselecteerde datum
        DagMeldingenControl.ItemsSource = _db.Meldingen
            .Where(m => m.Datum.HasValue && m.Datum.Value.Date == date.Date)
            .ToList();
    }

    private void PrevMonth_Click(object sender, RoutedEventArgs e)
    {
        //  Ga naar vorige maand
        _huidigeDatum = _huidigeDatum.AddMonths(-1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void NextMonth_Click(object sender, RoutedEventArgs e)
    {
        //  Ga naar volgende maand
        _huidigeDatum = _huidigeDatum.AddMonths(1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void PrevYear_Click(object sender, RoutedEventArgs e)
    {
        //  Ga naar vorig jaar
        _huidigeDatum = _huidigeDatum.AddYears(-1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void NextYear_Click(object sender, RoutedEventArgs e)
    {
        //  Ga naar volgend jaar
        _huidigeDatum = _huidigeDatum.AddYears(1);
        Kalender.SetDisplayDate(_huidigeDatum);
        UpdateKalenderHeader();
    }

    private void BackToMaintenanceDashboard(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MaintenanceDashboard));
    }
}