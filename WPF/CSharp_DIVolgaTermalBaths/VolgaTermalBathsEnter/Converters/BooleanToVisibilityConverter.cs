using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace VolgaTermalBathsEnter.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return Visibility.Collapsed;

        bool isInverse = parameter != null &&
                         parameter.ToString() == "inverse";

        bool result = isInverse ? !boolValue : boolValue;
        return result ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}