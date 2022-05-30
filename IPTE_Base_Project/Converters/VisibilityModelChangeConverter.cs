using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IPTE_Base_Project.Converters
{
    class VisibilityModelChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Model = (string)value;
            switch (Model)
            {
                case "WL":
                    return Visibility.Visible;

                case "WS":
                    return Visibility.Hidden;

                default:
                    return Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
