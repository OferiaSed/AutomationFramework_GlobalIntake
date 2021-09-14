using AutomationFrame_GlobalIntake.Models;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using MyUtils;
using MyUtils.Email;
using OpenQA.Selenium;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.POM
{
    public partial class clsIntakeFlow
    {
        /// <summary>
        /// Find WC Dissemination Email by using Claim Number
        /// </summary>
        private clsEmailV2 fnFindWcDisseminationEmailByClaimNumber(clsData objData, string strClaimNo)
        {
            // Get Email Credentials
            Dictionary<string, string> strCreds = clsEmailV2.fnGetUserAndPasswordEmail(objData.fnGetValue("ActionValues", ""));

            // Find email
            clsReportResult.fnLog("Info Email SSN", $"Looking for email with subject 'Sedgwick WC Incident' and Claim Number '{strClaimNo}'", "Info", false);
            var email = new clsEmailV2(strCreds["User"], strCreds["Password"], clsEmailV2.emServer.POPGMAIL, true);
            var found = clsMG.fnGenericWait(() => email.fnReadEmail("Sedgwick WC Incident", strClaimNo), TimeSpan.FromSeconds(5), 10);
            if (found) clsReportResult.fnLog("Info Email SSN", $"Found email with subject 'Sedgwick WC Incident' and Claim Number '{strClaimNo}'", "Info", false);
            else clsReportResult.fnLog("Info Email SSN", $"Not found email with subject 'Sedgwick WC Incident' and Claim Number '{strClaimNo}'", "Fail", false, true, "Email was not found");

            return email;
        }

        /// <summary>
        /// Verify Froi in the attachment of dissemination email 
        /// </summary>
        private void fnCheckFroiAttachmentWcOnly(clsData objData, string strClaimNo)
        {
            var email = fnFindWcDisseminationEmailByClaimNumber(objData, strClaimNo);
            var success = email.Attachments.Any(
                attachmentPath => 
                {
                    var froiKeywords = "FIRSTREPORTOFWORKRELATEDINJURY";
                    var ocrText = clsOCR.fnGetOCRText(attachmentPath).fnToSingleLineText().fnOnlyAlphanumericChars().Replace(" ", "").Replace("-", "").ToUpper();
                    return ocrText.Contains(froiKeywords);
                }
            );
            clsReportResult.fnLog("Verify FROI Attachment was disseminated", $"FROI Attachment was disseminated. Claim #{strClaimNo}.", success ? "Pass" : "Fail", false);
        }

        /// <summary>
        /// Verify Froi in the attachment of dissemination email only in the Event info
        /// </summary>
        private void fnVerifyFroiInTheEmailsForWc()
        {

        }


        /// <summary>
        /// Verify The SSN Masking in intake review Email Dissemination and FROI PDF
        /// </summary>
        /// <param name="objData">obj to read the data sheet</param>
        /// <param name="strClaimNo">The claim number to look for</param>
        private void fnTcVerifySsnMaskingInIntakeReviewEmailDisseminationAndFroiPdf(clsData objData, string strClaimNo)
        {
            var email = fnFindWcDisseminationEmailByClaimNumber(objData, strClaimNo);

            // Verify SSN Mask in Email
            clsReportResult.fnLog("Info Email SSN", "Verifying Email SSN Mask", "Info", false);
            var ssnInEmail = email.fnGetContentAsString(
                "",
                "",
                CreateIntakeScreen.strHTMLEmailSSNDelimiterStart,
                CreateIntakeScreen.strHTMLEmailSSNDelimiterEnd
            );

            clsReportResult.fnLog(
                "Check SSN in email", $"SSN in email must be masked: {ssnInEmail}",
                ssnInEmail.Contains(clsConstants.ssnMask) ? "Pass" : "Fail",
                false
            );

            // Verify SSN Mask in Attached PDF
            clsReportResult.fnLog("Info PDF SSN OCR", "Verifying PDF SSN OCR", "Info", false);
            if (email.Attachments.Count > 0)
            {
                /*var ocrText = clsOCR.fnGetOCRText(email.Attachments.Single());
                var strSsnInPdf = ocrText.fnTextBetween(
                    "SOCIAL SECURITY NUMBER",
                    "ADDRESS (INCL ZIP)"
                ).Split(' ').SingleOrDefault(x => x.Contains("-") && x.Length == 11);*/

                var ocrText = clsOCR.fnGetOCRText(email.Attachments.Single());
                var strSsnInPdf = ocrText.fnTextBetween("SOCIAL SECURITY NUMBER","ADDRESS (INCL ZIP)");
                var blSsnFound = strSsnInPdf.Contains(clsConstants.ssnMask);

                clsReportResult.fnLog(
                    "Check SSN in pdf", $"SSN in pdf must be masked: {(blSsnFound ? strSsnInPdf.Substring(strSsnInPdf.IndexOf("XXX-"), 11) : "SSN not found in pdf")}",
                    (ssnInEmail != null ? ssnInEmail.Contains(clsConstants.ssnMask) : false) ? "Pass" : "Fail",
                    false
                );
            }
            else 
            {
                clsReportResult.fnLog("Info Email SSN", "The email does not have attachments to review.", "Fail", false);
            }
            
        }

        /// <summary>
        /// Verifies that each Force Refresh-Enabled field actually refreshes the page after its value is updated
        /// </summary>
        /// <param name="spreadsheetFileName"></param>
        private void VerifyTabbingOrderInForceRefreshFields(string spreadsheetFileName)
        {
            // Create list of questions required for validation
            var forceRefreshQuestionKeys = new List<string>();
            using (var questionsSheet = new SLDocument(spreadsheetFileName, "Questions"))
            {
                var stats = questionsSheet.GetWorksheetStatistics();
                for (var rowIndex = 2; rowIndex <= stats.EndRowIndex; rowIndex++)
                {
                    // Verify the question is enabled to forcefully refresh on value change
                    if (questionsSheet.GetCellValueAsBoolean(rowIndex, 9))
                    {
                        var section = questionsSheet.GetCellValueAsString(rowIndex, 1);
                        var question = questionsSheet.GetCellValueAsString(rowIndex, 2);
                        forceRefreshQuestionKeys.Add($"{section}.{question}");
                    }
                }
            }

            forceRefreshQuestionKeys.ForEach(
                questionKey =>
                {
                    var selector = CreateIntakeScreen.objQuestionXPathByQuestionKey(questionKey);
                    IWebElement question;
                    try
                    {
                        question = clsWebBrowser.objDriver.FindElement(selector);
                    }
                    catch (NoSuchElementException)
                    {
                        // Element is not present in this page
                        return;
                    }

                    clsWebBrowser.objDriver.fnScrollToElement(question);
                    var fields = question.FindElements(By.XPath(".//button | .//select | .//input")).Where(y => y.Enabled && y.Displayed).ToList();

                    // Skip Question if it contains any button
                    if (fields.Exists(x => x.TagName.ToUpper() == "BUTTON"))
                    {
                        return;
                    }

                    // Test
                    foreach (var field in fields)
                    {
                        switch (field.TagName.ToUpper())
                        {
                            case "INPUT":
                                var text = field.GetAttribute("inputmode") == "numeric" ? "1" : "TEST TEXT";
                                field.SendKeys(text);
                                clsWebBrowser.objDriver.FindElement(By.TagName("body")).SendKeys(Keys.Tab);
                                break;
                            case "SELECT":
                                field.fnGetParentNode().FindElement(By.XPath(".//span[@role='combobox']")).Click();
                                var optionValues = field.FindElements(By.TagName("option"));
                                var valueToSelect = optionValues.First(x => !string.IsNullOrWhiteSpace(x.Text)).Text;
                                var optionElement = clsWebBrowser.objDriver.FindElement(By.XPath($"//ul[@role='tree']/li[contains(text(), '{valueToSelect}')]"));
                                clsWebBrowser.objDriver.fnScrollToElement(optionElement);
                                optionElement.Click();
                                break;
                        }
                        var visible = CreateIntakeScreen.fnUntilSpinnerVisible(clsMG, clsWebBrowser.objDriver);
                        var hidden = CreateIntakeScreen.fnUntilSpinnerHidden(clsMG, clsWebBrowser.objDriver);
                        var result = visible && hidden ? "Pass" : "Fail";
                        clsReportResult.fnLog("Force Refresh", $"Force Refresh: Page is refreshed after changing value of '{questionKey}'.", result, true);
                    }
                }
            );
        }

        private string fnGetBranchOffice(string strClientNo, string strState)
        {
            clsDB objDBOR = new clsDB();
            objDBOR.fnOpenConnection(objDBOR.GetConnectionString("lltcsed1dvq-scan", "1521", "viaonei", "oferia", "P@ssw0rd#02"));
            string strQuery = "select ex_office from viaone.cont_st_off where cont_num = '{CLIENTNO}' and data_set = 'WC' and state = '{STATE}' fetch first 1 row only";
            var strValue = objDBOR.fnGetSingleValue(strQuery.Replace("{CLIENTNO}", strClientNo).Replace("{STATE}", strState));
            return strValue;
        }

        private void fnVerifyOneTeamIntakeFlow(clsData pobjData) 
        {
            var oneTeamDisplayed = clsMG.fnGenericWait(() => clsMG.IsElementPresent("//a[@id='NavOption_ONETEAM_MANDATORY_CHECK']"), TimeSpan.FromSeconds(1), 5);
            if (oneTeamDisplayed)
            {
                clsWE.fnClick(clsWE.fnGetWe("//a[@id='NavOption_ONETEAM_MANDATORY_CHECK']"), "One Team Menu", false);
                clsMG.fnCleanAndEnterText("One Tea, Callback Number", "//div[span[text()='Callback Phone Number']]//input", pobjData.fnGetValue("OneTeamCallback", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//button[span[text()='Send']]"), "Send One Team", false);
                var oneTeamSubmitted = clsMG.fnGenericWait(() => clsMG.IsElementPresent("//span[text()='Demographic data successfully sent to OneTeam']"), TimeSpan.FromSeconds(1), 5);
                if (oneTeamSubmitted)
                {
                    clsReportResult.fnLog("Verify OneTeam Submition", "The OneTeam Submit was done successfully.", "Pass", true);
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(CreateIntakeScreen.strWorkPhoneNumber), TimeSpan.FromSeconds(1), 5);
                    clsMG.fnCleanAndEnterText("Contact Work Phone", CreateIntakeScreen.strWorkPhoneNumber, pobjData.fnGetValue("ContactWorkPhone", ""), false, false, "", true);
                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                    clsMG.fnCleanAndEnterText("Employee Best Contact Number", CreateIntakeScreen.strEmployeeBestContactNumber, pobjData.fnGetValue("EmployeeBestContactNumber", ""), false, false, "", true);
                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                }
                else
                {
                    clsReportResult.fnLog("Verify OneTeam Submition", "The OneTeam was not submitted on intake screen.", "Fail", true);
                }
            }
            else 
            {
                clsReportResult.fnLog("Verify OneTeam flow", "The OneTeam menu should be displayed but was not found", "Fail", true);
            }
        }




    }
}
