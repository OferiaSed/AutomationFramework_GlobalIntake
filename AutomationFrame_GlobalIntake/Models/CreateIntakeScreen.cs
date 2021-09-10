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
        public CreateIntakeScreen(IWebDriver driver) : base(driver)
        {

        }

        /// <summary>
        /// The selector for the floating list in Create Intake Screen
        /// </summary>
        public static By objFloatingListSelector = By.XPath("//div[@id='list-example']/a[span]");

        /// <summary>
        /// Xpath Selector for all the labels present in Create Intake Screen
        /// </summary>
        public static By objAllLabels = By.XPath("//div[contains(@class, 'question-row')]/div/div[@class='row']//div[@class='row']//div[@class='col-md-12']");

        /// <summary>
        /// Question selector string by specific Question Key which can be found in script test file.
        /// </summary>
        /// <param name="questionKey">The question key. Example: EMPLOYEE_INFO.ADDRESS</param>
        /// <returns>Returns XPath selector in string format</returns>
        public static string strQuestionXPathByQuestionKey(string questionKey) => $"//div[contains(@class, 'question-row') and @question-key='{questionKey}']";

        /// <summary>
        /// Question selector by specific Question Key which can be found in script test file.
        /// </summary>
        /// <param name="questionKey">The question key. Example: EMPLOYEE_INFO.ADDRESS</param>
        /// <returns>Returns XPath selector</returns>
        public static By objQuestionXPathByQuestionKey(string questionKey) => By.XPath(strQuestionXPathByQuestionKey(questionKey));

        public static By objIsThisTheLossLocation = By.XPath("//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']");

        //Intake Flow Page
        public static string strIntakeFlowPage = "//h1[@data-bind='text: IntakeName']";

        //Intake Floating Menu
        public static string strMenuEmployeeInformation = "//a[@id='NavOption_EMPLOYEE_INFORMATION']";
        public static string strMenuLossLocationInformation = "//a[@id='NavOption_LOSS_LOCATION']";
        public static string strMenuClientLocationInformation = "//a[@id='NavOption_LOCAL_BUSINESS_EMPLOYER_INFORMATION']";



        //Intake Dashboard
        public static string strDashboard = "//section[@id='calls-section']";
        public static string strIntakeDetails = "//h4[text()='Intake Details']";
        public static string strCanceledRow = "//table[@id='myCalls']//tr[td[text()='{CLIENT}'] and td[text()='{REASON}']]//i";
        public static string strFilterResults = "//input[@placeholder='Filter Results']";

        //Cancel Intake
        public static string strCancelButtonIntake = "//div[@class='list-group-item text-center']//button[text()='Cancel']";
        public static string strCancelPopup = "//div[@id='abandonModal' and contains(@style, 'display: block')]";
        public static string strCancelReasonDropdown = "(//div[@id='abandonModal' and contains(@style, 'display: block')]//span[@role='combobox'])[1]";
        public static string strCancelDescriptionInput = "//div[@id='abandonModal' and contains(@style, 'display: block')]//textarea";
        public static string strConfirmCancelButton = "//div[@id='abandonModal' and contains(@style, 'display: block')]//button[text()='Confirm']";

        public static string strDetailsStatus = "//span[@data-bind='text: InstanceStatusDescription']";
        public static string strDetailsReason = "//span[@data-bind='text: abandonReasonDescription']";
        public static string strDetailsDescription = "//span[@data-bind='text: abandonDescription']";
        public static string strDeleteClaimLink = "//*[text()='Mark this intake as deleted']";
        public static string strDeletePopup = "//div[@id='modalChangeFlagStatus']";
        public static string strConfirmDelete = "//div[@id='modalChangeFlagStatus']//button[contains(text(), 'Confirm')]";

        //Resume Intake
        public static string strResumeIntakeButton = "//div[@class='list-group-item text-center']//button[text()='Save']";
        public static string strResumeReasonDropdown = "(//div[@id='abandonModal' and contains(@style, 'display: block')]//span[@role='combobox'])[2]";
        public static string strResumeRow = "//table[@id='myCalls']//tr[td[text()='{CLIENT}'] and td[text()='{REASON}'] and td[contains(text(), '{DATE}')]]//i";

        //Branch Office
        public static string strBONumber = "//span[contains(@data-bind, 'Answer.OfficeNumber()')]";
        public static string strBenefitLabel = "//h2//*[text()='Benefit State']";
        public static string strDefaultBenefitState = "//div[contains(@class, 'row') and div[span[text()='Default Benefit State']]]//div[2]/span";
        public static string strReviewLabel = "(//span[@data-bind='html:HelpText'])[1]";

        //Review Screen
        public static string strReviewScreen = "//span[text()='Review']";
        public static string strReviewEditField = "//div[@class='row py-2' and div[span[contains(text(), '{NAMEFIELD}')]]]//a";

        //Employee Information
        public static string strSearchEEAddress = "//input[@id='CLAIM_EMPLOYEE_ADDRESS']";
        public static string strSearchableAddress = "(//div[@class='tt-suggestion tt-selectable'])[1]";
        public static string strEEAddress1 = "//div[@id='address_CLAIM_EMPLOYEE_ADDRESS']//input[@placeholder='Address Line 1']";
        public static string strEEZipCode = "//div[@id='address_CLAIM_EMPLOYEE_ADDRESS']//input[@placeholder='Zip Code']";
        public static string strEECity = "//div[@id='address_CLAIM_EMPLOYEE_ADDRESS']//input[@placeholder='City']";

        //Loss Location Information
        public static string strLossLocName = $"{strQuestionXPathByQuestionKey("LOSS_LOCATION.CLAIM_LOSS_LOCATION_NAME")}//input";
        public static string strLossLocNonStandardAddress = $"{strQuestionXPathByQuestionKey("LOSS_LOCATION.CLAIM_LOSS_LOCATION_ADDRESS")}//input[@type='checkbox']";
        public static string strLossLocddress1 = "//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[@placeholder='Address Line 1']";
        public static string stsLossLocZipCode = "//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[@placeholder='Zip Code']";
        public static string strLossLocCity = "//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[@placeholder='City']";
        public static string strLossLocStateAndCountrySelector = $"{strQuestionXPathByQuestionKey("LOSS_LOCATION.CLAIM_LOSS_LOCATION_ADDRESS")}//span[@class='select2-selection select2-selection--single']";

        //Employement Information
        public static string strClaimEmployeeMissWorkBeyondShifFlag = $"{strQuestionXPathByQuestionKey("EMPLOYMENT_INFORMATION.CLAIM_EMPLOYEE_MISS_WORK_BEYOND_SHIFT_FLG")}//span[@class='select2-selection select2-selection--single']";

        //Contact Information
        public static string strWorkPhoneNumber = $"{strQuestionXPathByQuestionKey("CONTACT_INFORMATION.CONTACT_PHONE_WORK")}//input";

        //Lost Time Information
        public static string strEmployeeReturnedToWork = $"{strQuestionXPathByQuestionKey("LOST_TIME_INFORMATION.CLAIM_EMPLOYEE_RTW_FLG")}//span[@class='select2-selection select2-selection--single']";

        //Created Claim Email
        public static string strHTMLEmailSSNDelimiterStart = "<tr class='m-1'> <td class='question-label'>SSN</td> <td class='pl-2'><div class='text-spacing-1'>";
        public static string strHTMLEmailSSNDelimiterEnd = "<br/></div></td> </tr>";
    }
}