using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
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

        /// <summary>
        /// Check if an element is visible
        /// </summary>
        /// <param name="by">Selector of the element to be checked</param>
        /// <param name="driver">The WebDriver</param>
        /// <returns>True if element is visible</returns>
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
        /// Check if an element is not visible
        /// </summary>
        /// <param name="by">Selector of the element to be checked</param>
        /// <param name="driver">The WebDriver</param>
        /// <returns>True if element is not visible</returns>
        public static bool fnIsElementHidden(By by, IWebDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(by));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //TODO Delete if it is not necessary
        /// <summary>
        /// Created to scroll in pages as needed and make a specific element visible
        /// </summary>
        /// <param name="driver">The WebDriver</param>
        /// <param name="element">The elelemt to scroll to</param>
        public static void fnScrollToElement(this IWebDriver driver, IWebElement element) =>
            new Actions(driver)
                .MoveToElement(element)
                .Build()
                .Perform();

        /// <summary>
        /// Get the parent of a Web Element by using Javascript
        /// </summary>
        /// <param name="driver">The WebDriver</param>
        /// <param name="element">The element to find the parent of</param>
        /// <returns>The parent element</returns>
        public static IWebElement fnGetParentNodeFromJavascript(this IWebDriver driver, IWebElement element) => (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].parentNode;", element);
        /// <summary>
        /// Get the parent of a Web Element by using XPath
        /// </summary>
        /// <param name="driver">The WebDriver</param>
        /// <param name="element">The element to find the parent of</param>
        /// <returns>The parent element</returns>
        public static IWebElement fnGetParentNode(this IWebElement element) => element.FindElement(By.XPath("./.."));
    }
}
