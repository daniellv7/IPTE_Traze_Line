using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IPTE_Base_Project.Common.Utils
{
    public static class CollectionUtils
    {
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.Invoke(addMethod, item);
        }

        public static void ClearOnUI<T>(this ICollection<T> collection)
        {
            Action clearMethod = collection.Clear;
            Application.Current.Dispatcher.Invoke(clearMethod);
        }
    }
}
