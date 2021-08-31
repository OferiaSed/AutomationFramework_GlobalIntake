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

        /// <summary>
        /// Generates a Future/Old/Current Date and return as string with format MM/dd/yyyy
        /// </summary>
        /// <param name="pstrDays">Provide a valid formart like Today+Ndays, Today, Today-5days</param>
        /// <returns></returns>
        public static string fnGetCustomDate(string pstrDays) 
        {
            string newDate = "";
            if (pstrDays.Contains("+") || pstrDays.Contains("-"))
            {
                DateTime dtNewDate;
                dtNewDate = DateTime.Today.AddDays(Convert.ToDouble(pstrDays.Replace("TODAY", "").Replace("Today", "").Replace("today", "")));
                newDate = dtNewDate.ToString("MM/dd/yyyy");
            }
            else if (pstrDays == "TODAY" || pstrDays == "Today" || pstrDays == "today")
            {
                newDate = DateTime.Today.ToString("MM/dd/yyyy");
            }
            else if (pstrDays.ToUpper() == "INVALIDDATE") 
            {
                newDate = "41/41/9999";
            }
            return newDate;
        }

    }
}
