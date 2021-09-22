using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using System;

namespace AutomationFrame_GlobalIntake.Models
{
    public class SearchIntakeModel : BasePageModel
    {
        /// <summary>
        /// Intake Page Model
        /// </summary>
        public SearchIntakeModel(IWebDriver driver, clsMegaIntake clsMG) : base(driver, clsMG) { }

        // string Selector
        public static string strClaimNumberInputSelector = "//input[@data-bind='value: SubmittedIntakeDTO().SearchParameters.VendorIncidentNumber']";
        
        // By Selector
        public static By objEventSectionSelector = By.XPath("//div[@class='row']//h4[contains(text(),'Event')]/../../..");
        public static By objSearchButton = By.XPath("//button[@data-bind='click: SearchIntakes']");

        /// <summary>
        /// Open details of an specific Intake by using its Claim Number to identify it
        /// </summary>
        /// <param name="strClaimNo">The Claim Number</param>
        /// <returns>True if success</returns>
        public bool OpenIntakeDetailsByClaimNumber(string strClaimNo)
        {
            return clsUtils.TryExecute(
                () =>
                {
                    var openDetailsButtonSelector = By.XPath($"//td[text()='{strClaimNo}']/../td/a");
                    clsMG.fnGenericWait(() => this.driver.fnWaitUntilElementVisible(openDetailsButtonSelector), TimeSpan.Zero, 5);
                    var openDetailsButton = driver.FindElement(openDetailsButtonSelector);
                    this.driver.fnScrollToElement(openDetailsButton);
                    openDetailsButton.Click();
                }
            );
        }

        /// <summary>
        /// Search an intake by using its Claim Number
        /// </summary>
        /// <param name="strClaimNo">The Claim Number</param>
        /// <returns>True if Success</returns>
        public bool SearchIntakeByClaimNumber(string strClaimNo)
        {
            return clsUtils.TryExecute(
                () =>
                {
                    clsMG.fnCleanAndEnterText("Claim Number Input", strClaimNumberInputSelector, strClaimNo, true);
                    var searchButton = clsWebBrowser.objDriver.FindElement(objSearchButton);
                    clsWebBrowser.objDriver.fnScrollToElement(searchButton);
                    searchButton.Click();
                }
            );
        }
    }
}
