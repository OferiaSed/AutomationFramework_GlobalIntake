using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Utils
{
    static class clsUtils
    {
        public static IWebElement fnGetParentNodeFromJavascript(this IWebDriver driver, IWebElement element) => (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].parentNode;", element);
        public static IWebElement fnGetParentNode(this IWebElement element) => element.FindElement(By.XPath("./.."));
      
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
        /// Verifies if element is visible, if not returns false.
        /// </summary>
        /// <param name="By">Provide Locator like xpath, css, id, ect.</param>
        /// <param name="By">Provide webdriver object.</param>
        /// <returns></returns>
        public static bool fnIsElementVisible(By by, IWebDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
                return true;
            }
            catch (Exception)
            {
                return false;
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
