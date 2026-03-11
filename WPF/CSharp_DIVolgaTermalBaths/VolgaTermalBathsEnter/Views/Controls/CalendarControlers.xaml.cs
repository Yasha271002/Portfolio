using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using VolgaTermalBathsEnter.Models.Client.PriceList;

namespace VolgaTermalBathsEnter.Views.Controls;

/// <summary>
/// Логика взаимодействия для CalendarControlers.xaml
/// </summary>
public partial class CalendarControlers : UserControl
{
    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        "SelectedDate", typeof(DateTime?), typeof(CalendarControlers),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly DependencyProperty SelectedParameterModelProperty = DependencyProperty.Register(
        nameof(SelectedParameterModel), typeof(ApiParameters<ApiPriceListModel>), typeof(CalendarControlers),
        new PropertyMetadata(default(ApiParameters<ApiPriceListModel>)));

    public ApiParameters<ApiPriceListModel> SelectedParameterModel
    {
        get { return (ApiParameters<ApiPriceListModel>)GetValue(SelectedParameterModelProperty); }
        set { SetValue(SelectedParameterModelProperty, value); }
    }

    public static readonly DependencyProperty UnavailableDatesProperty = DependencyProperty.Register(
        nameof(UnavailableDates), typeof(List<DateTime>), typeof(CalendarControlers),
        new PropertyMetadata(default(List<DateTime>)));

    public List<DateTime> UnavailableDates
    {
        get { return (List<DateTime>)GetValue(UnavailableDatesProperty); }
        set { SetValue(UnavailableDatesProperty, value); }
    }

    public CalendarControlers()
    {
        InitializeComponent();
    }

    private void Calendar_OnDisplayDateChanged(object? sender, CalendarDateChangedEventArgs e)
    {
        var today = DateTime.Today;
        var firstDayOfMonth = e.AddedDate!.Value;
        Calendar_ValidateDate(ref firstDayOfMonth, ref today);
    }

    private void Calendar_ValidateDate(ref readonly DateTime firstDay, ref readonly DateTime today)
    {
        if (firstDay >= today) return;
        Calendar.DisplayDate = today;
        SelectedDate = today;
    }

    private void Calendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
    {
        if (Calendar.DisplayMode != CalendarMode.Month)
        {
            Calendar.DisplayMode = CalendarMode.Month;
        }
    }
}