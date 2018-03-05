using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Yahtzee.WinClient
{
    public class IntToImageConverter : IValueConverter
    {
        public IntToImageConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string image;
            switch ((int)value)
            {
                case 1:
                    image = "Images/dice1.png";
                    break;
                case 2:
                    image = "Images/dice2.png";
                    break;
                case 3:
                    image = "Images/dice3.png";
                    break;
                case 4:
                    image = "Images/dice4.png";
                    break;
                case 5:
                    image = "Images/dice5.png";
                    break;
                case 6:
                    image = "Images/dice6.png";
                    break;
                default:
                    image = null;
                    break;
            }

            return image == null ? null : $"pack://application:,,,/Yahtzee.WinClient;component/{image}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}