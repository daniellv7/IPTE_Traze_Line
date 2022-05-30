using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IPTE_Base_Project.Converters
{
    public class MachineStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int Status = (int)value;
            switch (Status)
            {
                case 1:
                    return Translation.Translate.MachineState_Automatic;
                case 2:
                    return Translation.Translate.MachineState_Manual;
                case 3:
                    return Translation.Translate.MachineState_DRP;
                case 4:
                    return Translation.Translate.MachineState_Bypass;
                case 5:
                    return Translation.Translate.MachineState_Fail;
                default:
                    return Translation.Translate.MachineState_Idle;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
