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
                ssnInEmail.Contains(ssnMask) ? "Pass" : "Fail",
                false
            );

            // Verify SSN Mask in Attached PDF
            clsReportResult.fnLog("Info PDF SSN OCR", "Verifying PDF SSN OCR", "Info", false);
            var ocrText = clsOCR.fnGetOCRText(email.Attachments.Single());
            var strSsnInPdf = ocrText.fnTextBetween(
                "NAME (LAST, FIRST, MIDDLE) DATE OF BIRTH SOCIAL SECURITY NUMBER | DATE HIRED STATE OF HIRE",
                "ADDRESS (INCL ZIP) SEX MARITAL STATUS OCCUPATION/JOB TITLE"
            ).Split(' ').SingleOrDefault(x => x.Contains("-") && x.Length == 11);
            var blSsnFound = !string.IsNullOrEmpty(ssnInEmail);

            clsReportResult.fnLog(
                "Check SSN in pdf", $"SSN in pdf must be masked: {(blSsnFound ? strSsnInPdf : "SSN not found in pdf")}",
                (ssnInEmail != null ? ssnInEmail.Contains(ssnMask) : false) ? "Pass" : "Fail",
                false
            );
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
    }
}
