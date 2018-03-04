using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Yahtzee.WinClient
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 210)) : new SolidColorBrush(Color.FromArgb(0, 255, 255, 210));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}