using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IPTE_Base_Project.Converters
{
    public class ShuttleBackgroundChange : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int Status = (int)value;
            switch (Status)
            {
                case 1:
                    return new SolidColorBrush(Colors.MediumSeaGreen);
                case 2:
                    return new SolidColorBrush(Colors.Red);
                case 20:
                    return new SolidColorBrush(Colors.MediumSeaGreen);
                default:
                    return new SolidColorBrush(Colors.Gray);


            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
