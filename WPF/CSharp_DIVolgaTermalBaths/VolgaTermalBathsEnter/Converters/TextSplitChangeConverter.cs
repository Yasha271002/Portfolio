using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace VolgaTermalBathsEnter.Converters;

public class TextSplitChangeConverter:IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str) return value;

        var match = Regex.Match(str, @"^[^/]+");
        return match.Success ? match.Value : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}