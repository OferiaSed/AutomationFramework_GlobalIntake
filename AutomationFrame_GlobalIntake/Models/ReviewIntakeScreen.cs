using AutomationFrame_GlobalIntake.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class ReviewIntakeScreen : clsBasePageModel
    {
        public ReviewIntakeScreen(IWebDriver driver) : base(driver) { }

        private static string objRowSelector(string field) => $"//div[div[span[text()='{field}']]]";
        public string GetFieldValue(string field)
        {
            var element = driver.FindElement(By.XPath(objRowSelector(field))).FindElement(By.XPath(".//div[2]"));
            driver.fnScrollToElement(element);
            return element.Text;
        }

        public void ClickEditFieldValue(string field)
        {
            var element = driver.FindElement(By.XPath(objRowSelector(field))).FindElement(By.XPath(".//div[3]/a"));
            driver.fnScrollToElement(element);
            element.Click();
        }
    }
}
