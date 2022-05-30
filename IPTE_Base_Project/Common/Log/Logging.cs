using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Common
{
    public class Logging
    {
        // Esta línea debe estar en todas las clases de la solución
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Logging()
        {

        }

        #region Debug
        public static void DebugIn(object className, string methodName)
        {
            String logMessage = String.Format("{0} => {1}", className.ToString().Split('.').Last(), methodName);
            log.Debug(logMessage);
        }
        public static void DebugOut(object className, string methodName)
        {
            String logMessage = String.Format("{0} <= {1}", className.ToString().Split('.').Last(), methodName);
            log.Debug(logMessage);
        }
        #endregion
    }
}
