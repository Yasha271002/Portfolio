using System.Globalization;
using System.Windows.Data;

namespace SberUniversity.Helpers.Converters;

public class ValueInRangeConverter:IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3) return false;
        if (values[0] is not int value) return false;
        if (values[1] is not int startValue) return false;
        if (values[2] is not int endValue) return false;
        return value >= startValue && value < endValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}