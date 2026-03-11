using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VolgaTermalBathsEnter.Converters;

public class StringIsNullOrEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && string.IsNullOrEmpty(str))
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}