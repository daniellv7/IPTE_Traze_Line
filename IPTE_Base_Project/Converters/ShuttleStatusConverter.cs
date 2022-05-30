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
    class ShuttleStatusConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int Status = (int)value;
            switch (Status)
            {
                case 1:
                    return "Status:Active";
                case 2:
                    return "Status:Warning";
                case 3:
                    return "Status:Error";
                case 4:
                    return "Status:Maintenance";
                default:
                    return "Status:Idle";


            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}
