using AutomationFrame_GlobalIntake.POM;
using AutomationFramework;
using MyUtils.Email;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutomationFrame_GlobalIntake.Utils
{
    static class clsUtils
    {
        /// <summary>
        /// ClsMG object to reference clsMegaIntake Classs
        /// </summary>
        private static clsMegaIntake clsMG = new clsMegaIntake();

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

        #region WebDriver Extensions
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
        /// Verifies if element is visible, if not returns false.
        /// </summary>
        /// <param name="by">Provide Locator like xpath, css, id, ect.</param>
        /// <param name="driver">Provide webdriver object.</param>
        /// <returns></returns>
        public static bool fnIsElementVisible(this IWebDriver driver, By by)
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
        #endregion

        /// <summary>
        /// Get the parent of a Web Element by using XPath
        /// </summary>
        /// <param name="driver">The WebDriver</param>
        /// <param name="element">The element to find the parent of</param>
        /// <returns>The parent element</returns>
        public static IWebElement fnGetParentNode(this IWebElement element) => element.FindElement(By.XPath("./.."));

        /// <summary>
        /// Find text between two sections of text
        /// </summary>
        /// <param name="str">string to extract text from</param>
        /// <param name="start">left limit string</param>
        /// <param name="end">right limit string</param>
        /// <returns></returns>
        public static string fnTextBetween(this string str, string start, string end)
        {
            var index = str.IndexOf(start) + start.Length;
            var result = str.Substring(index);
            var lenght = result.IndexOf(end);
            result = result.Substring(0, lenght);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string fnToSingleLineText(this string str)
        {
            return str.Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Returns the string without any non-alphanumeric character
        /// </summary>
        /// <param name="str">string to clean</param>
        /// <returns></returns>
        public static string fnOnlyAlphanumericChars(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            str = rgx.Replace(str, "");
            return str;
        }

        /// <summary>
        /// Function to return true if no exception is throwed when executing
        /// </summary>
        /// <param name="action">Function to execute</param>
        /// <returns>True if no exceptions thrown</returns>
        public static bool TryExecute(this Action action)
        {
            try
            {
                action.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                clsReportResult.fnLog(
                    "Exception was throwed",
                    $"Exception Message: '{ex.Message}'. Stack Trace: {ex.StackTrace}",
                    "Warn",
                    true
                );
                return false;
            }
        }

        public static bool fnIsElementEnabledVisible(By by, IWebDriver driver)
        {
            try
            {
                IWebElement element = driver.FindElement(by);
                return element.Displayed && element.Enabled; 
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static clsEmailV2 fnFindGeneratedEmail(string pstrSetNo, string pstrSubject, string pstrContainsText, bool blNegativescenario = false)
        {
            // Get Email Credentials
            Dictionary<string, string> strCreds = clsEmailV2.fnGetUserAndPasswordEmail(pstrSetNo);

            // Find email
            clsReportResult.fnLog("Info Email", $"Looking for email with subject '{pstrSubject}'", "Info", false);
            var email = new clsEmailV2(strCreds["User"], strCreds["Password"], clsEmailV2.emServer.POPGMAIL, false);
            var found = clsMG.fnGenericWait(() => email.fnReadEmail(pstrSubject, pstrContainsText), TimeSpan.FromSeconds(5), 10);
            if (!blNegativescenario)
            {
                if (found) clsReportResult.fnLog("Info Email", $"The email with subject '{pstrSubject}' was found as expected.", "Pass", false);
                else clsReportResult.fnLog("Info Email", $"The email with subject '{pstrSubject}' was not found as expected.", "Fail", false, false);
            }
            else
            {
                if (!found) clsReportResult.fnLog("Info Email", $"The email with subject '{pstrSubject}' was not found as expected.", "Pass", false);
                else clsReportResult.fnLog("Info Email", $"The email with subject '{pstrSubject}' was found as expected but should not be generated.", "Fail", false, false);
            }
            

            return email;
        }

    }
}
