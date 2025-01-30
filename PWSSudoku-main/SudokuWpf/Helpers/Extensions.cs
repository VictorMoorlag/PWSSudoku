using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuWpf.Helpers;
public static class Extensions
{
    public static string ToRowLetter(this int number)
    {
        if (number < 0 || number > 8)
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 0 and 8.");

        return ((char)('A' + number)).ToString();
    }

    public static string ToColumnLetter(this int number)
    {
        if (number < 0 || number > 8)
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 0 and 8.");

        return ((char)('a' + number)).ToString();
    }
}
