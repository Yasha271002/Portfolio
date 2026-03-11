using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VolgaTermalBathsEnter.Converters;

public class CityToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string city) return null;
        if (city == "Город" || string.IsNullOrEmpty(city))
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17B8A8"));
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#171717"));

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}