using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationFrame_GlobalIntake.Models
{
    public static class CreateIntakeScreen
    {
        public static By objFloatingListSelector = By.XPath("//div[@id='list-example']/a[span]");
        public static By objAllLabels = By.XPath("//div[contains(@class, 'question-row')]/div/div[@class='row']//div[@class='row']//div[@class='col-md-12']");
        public static By objIsThisTheLossLocation = By.XPath("//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']");

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

    }
}
