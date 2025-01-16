using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SudokuWpf.Converters;
public class ValueToStringConverter : IValueConverter
{
    // Convert int to string and return empty string if value is 0
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            var parameterValue = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return (doubleValue - parameterValue).ToString(CultureInfo.InvariantCulture);
        }
        else if (value is int intValue)
        {
            return intValue == 0 ? string.Empty : intValue.ToString();
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string strValue = value as string;
        return string.IsNullOrEmpty(strValue) ? 0 : int.Parse(strValue);
    }
}
