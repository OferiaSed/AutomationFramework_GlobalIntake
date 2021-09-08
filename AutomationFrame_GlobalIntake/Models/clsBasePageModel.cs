﻿using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class clsBasePageModel
    {
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
        public static bool fnUntilSpinnerHidden(clsMegaIntake clsMG, IWebDriver driver)
        {
            var spinnerHidden = clsMG.fnGenericWait(() => clsUtils.fnIsElementHidden(objLoadSpinnerSelector, driver), TimeSpan.Zero, 2);
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
        public static bool fnUntilSpinnerVisible(clsMegaIntake clsMG, IWebDriver driver)
        {
            var spinnerVisible = clsMG.fnGenericWait(() => clsUtils.fnIsElementVisible(objLoadSpinnerSelector, driver), TimeSpan.Zero, 2);
            return spinnerVisible;
        }
    }
}