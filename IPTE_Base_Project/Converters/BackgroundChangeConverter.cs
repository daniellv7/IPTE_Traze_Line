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
    public class BackgroundChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int Status = (int)value;
            switch (Status)
            {
                case 1:
                    return new SolidColorBrush(Colors.Green);
                case 2:
                    return new SolidColorBrush(Colors.Yellow);
                case 3:
                    return new SolidColorBrush(Colors.Orange);
                case 4:
                    return new SolidColorBrush(Colors.Blue);
                case 5:
                    return new SolidColorBrush(Colors.Red);
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
