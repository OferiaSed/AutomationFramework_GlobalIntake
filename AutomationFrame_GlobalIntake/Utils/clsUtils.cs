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

        public static IWebElement fnGetParentNodeFromJavascript(this IWebDriver driver, IWebElement element) => (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].parentNode;", element);
        public static IWebElement fnGetParentNode(this IWebElement element) => element.FindElement(By.XPath("./.."));
    }
}
