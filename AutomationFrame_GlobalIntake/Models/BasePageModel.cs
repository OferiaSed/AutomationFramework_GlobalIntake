using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class BasePageModel
    {
        protected IWebDriver driver;
        protected clsMegaIntake clsMG;
        protected BasePageModel(IWebDriver driver, clsMegaIntake clsMG)
        {
            this.driver = driver;
            this.clsMG = clsMG;
        }

        /// <summary>
        /// NAvigates to Search Calls Module
        /// </summary>
        /// <returns>True if success</returns>
        public bool NavigateToSearchCalls()
        {
            this.clsMG.fnHamburgerMenu("Search Intakes");
            return this.fnUntilSpinnerHidden();
        }

        /// <summary>
        /// The load spinner showed when any page is loading
        /// </summary>
        internal static By objLoadSpinnerSelector = By.Id("spinner");

        /// <summary>
        /// Wait until spinner is hidden.
        /// </summary>
        /// <param name="clsMG">clsMegaIntake</param>
        /// <param name="driver">IWebDriver</param>
        /// <returns>
        /// True once spinner gets hidden.
        /// False if spinner is visible even after 2 attempts.
        /// </returns>
        public bool fnUntilSpinnerHidden()
        {
            var spinnerHidden = this.clsMG.fnGenericWait(() => clsUtils.fnIsElementHidden(objLoadSpinnerSelector, this.driver), TimeSpan.Zero, 2);
            return spinnerHidden;
        }

        /// <summary>
        /// Wait until spinner is visible
        /// </summary>
        /// <param name="clsMG">clsMegaIntake</param>
        /// <param name="driver">IWebDriver</param>
        /// <returns>
        /// True once spinner gets visible.
        /// False if spinner is not visible even after 2 attempts.
        /// </returns>
        public bool fnUntilSpinnerVisible()
        {
            var spinnerVisible = clsMG.fnGenericWait(() => this.driver.fnWaitUntilElementVisible(objLoadSpinnerSelector), TimeSpan.Zero, 2);
            return spinnerVisible;
        }
    }
}
