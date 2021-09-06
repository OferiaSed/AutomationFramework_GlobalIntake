using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationFrame_GlobalIntake.Models
{
    public class CreateIntakeScreen : clsBasePageModel
    {
        /// <summary>
        /// The selector for the floating list in Create Intake Screen
        /// </summary>
        public static By objFloatingListSelector = By.XPath("//div[@id='list-example']/a[span]");
        
        /// <summary>
        /// Xpath Selector for all the labels present in Create Intake Screen
        /// </summary>
        public static By objAllLabels = By.XPath($"//div[contains(@class, 'question-row')]/div/div[@class='row']//div[@class='row']//div[@class='col-md-12']");
        
        /// <summary>
        /// Question selector by specific Question Key wich can be found in script test file.
        /// </summary>
        /// <param name="questionKey">The question key. Example: EMPLOYEE_INFO.ADDRESS</param>
        /// <returns>Returns XPath selector</returns>
        public static By objQuestionXPathByQuestionKey(string questionKey) => By.XPath($"//div[contains(@class, 'question-row') and @question-key='{questionKey}']");
    }   
}
