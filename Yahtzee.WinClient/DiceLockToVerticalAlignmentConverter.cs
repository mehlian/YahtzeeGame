using System;
using System.Globalization;
using System.Windows.Data;

namespace Yahtzee.WinClient
{
    public class DiceLockToVerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Top" : "Bottom";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}