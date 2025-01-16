using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SudokuWpf.Converters;
internal class TurnNumberToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int turnNumber && parameter is string reverseString &&  bool.TryParse(reverseString, out var reverse))
        {
            var isEven = turnNumber % 2 == 0;
            if (isEven)
                return reverse ? Visibility.Hidden : Visibility.Visible;
            else
                return reverse ? Visibility.Visible : Visibility.Hidden;
        }
        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
