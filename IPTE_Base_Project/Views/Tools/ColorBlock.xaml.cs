using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IPTE_Base_Project.Views.Tools
{
    /// <summary>
    /// Interaction logic for ColorBlock.xaml
    /// </summary>
    public partial class ColorBlock : UserControl
    {
        public ColorBlock()
        {
            InitializeComponent();
        }
    }

    public class PeakHelper : DependencyObject
    {
        public static readonly DependencyProperty IsPeakProperty = DependencyProperty.RegisterAttached(
            "IsPeak", typeof(bool), typeof(PeakHelper), new PropertyMetadata(false));


        public static void SetIsPeak(DependencyObject target, Boolean value)
        {
            target.SetValue(IsPeakProperty, value);
        }

        public static bool GetIsPeak(DependencyObject target)
        {
            return (bool)target.GetValue(IsPeakProperty);
        }
    }
}
