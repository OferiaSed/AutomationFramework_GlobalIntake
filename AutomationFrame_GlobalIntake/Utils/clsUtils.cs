using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Utils
{
    static class clsUtils
    {
        /// <summary>
        /// Executes a method if the specified codition is true
        /// </summary>
        /// <param name="condition">If true 'action' will be executed</param>
        /// <param name="action">Action to execute</param>
        public static void fnExecuteIf(bool condition, Action action)
        {
            if (condition)
            {
                action.Invoke();
            }
        }
    }
}
