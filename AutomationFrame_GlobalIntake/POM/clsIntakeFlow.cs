using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFrame_GlobalIntake.Models;

namespace AutomationFrame_GlobalIntake.POM
{
    class clsIntakeFlow
    {
        private readonly clsMegaIntake clsMG = new clsMegaIntake();
        private readonly clsWebElements clsWE = new clsWebElements();


        public bool fnSelectClientPopup(string pstrClientNumber, string pstrClientName)
        {
            bool blResult = true;
            clsWE.fnClick(clsWE.fnGetWe("//button[@id='selectClient_']"), "Select Client Button", false);
            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]"), "Select Client Popup", false, false);
            if (clsWE.fnElementExist("Select Client Popup", "//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]", false, false))
            {
                //Apply the filter
                clsMG.fnCleanAndEnterText("Client Number or Name", "//input[@placeholder='Client Number or Name']", pstrClientNumber, false, false, "", false);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                //Check if the client exist
                if (clsMG.IsElementPresent("//tr[td[contains(text(), '" + pstrClientNumber + "')] and td[contains(text(), '" + pstrClientName + "')]]"))
                {
                    clsWE.fnClick(clsWE.fnGetWe("//tr[td[contains(text(), '" + pstrClientNumber + "')] and td[contains(text(), '" + pstrClientName + "')]]//a"), "Select Button", true);
                }
                else
                {
                    clsReportResult.fnLog("Select Client Popup", "The client: " + pstrClientNumber + " was not found in the popup", "Fail", true, true);
                    blResult = false;
                }
            }
            else
            {
                clsReportResult.fnLog("Select Client Popup", "The select intake popup was not displayed", "Fail", true, true);
                blResult = false;
            }
            return blResult;
        }

        public bool fnSelectIntake(string pstrClientNumber, string pstrClientName)
        {
            bool blResult = true;
            //Go to New Intake and select Intake
            clsMG.fnHamburgerMenu("New Intake");
            if (clsWE.fnElementExist("Select Intake", "//h4[text()='Select Intake']", true))
            {
                //Verify if the client is already selected
                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//button[@id='selectClient_']/span"), "Select Client", "innerText", false) == "SELECT CLIENT")
                {
                    blResult = fnSelectClientPopup(pstrClientNumber, pstrClientName);
                }
            }
            else
            {
                clsReportResult.fnLog("Select Intake", "The select intake page was not loaded as expected.", "Fail", true, true);
                blResult = false;
            }

            //Close popup
            if (!blResult)
            {
                if (clsMG.IsElementPresent("//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]"))
                { clsWE.fnClick(clsWE.fnGetWe("//a[@id='btn_close_client']"), "Save", false); }
            }

            return blResult;
        }

        public bool fnStartNewIntake(string pstrLOB)
        {
            bool blResult = true;
            //Go to New Intake and select Intake
            clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Select Intake']"), "Select Intake", false, false);
            clsMG.fnCleanAndEnterText("Filter Result", "//input[@placeholder='Filter Results']", pstrLOB, false, false, "", false);

            //Verify LOB Name
            string strLocator;
            if (pstrLOB.Contains("'"))
            {
                string[] arrLOB = pstrLOB.Split('\'');
                string strFirstPart = "\"" + arrLOB[0] + "'\"";
                strLocator = "//table[@id='intakes']//tr[td[@data-bind='text: Name'][contains(text()," + strFirstPart + ") and contains(text(),'" + arrLOB[1] + "')]]//button";
            }
            else
            {
                strLocator = "//table[@id='intakes']//tr[td[@data-bind='text: Name' and contains(text(), '" + pstrLOB + "')]]//button";
            }

            //Verify is LOB script is displayed
            if (clsWE.fnElementExist("Select LOB", strLocator, true))
            {
                clsWE.fnClick(clsWE.fnGetWe(strLocator), "Select Intake Script", false);
                //Verify if Confirmation Need exist
                if (clsMG.IsElementPresent("//div[@id='megaModalDialog' and contains(@style, 'display: block;')]"))
                { clsWE.fnClick(clsWE.fnGetWe("//button[text()='OK']"), "Confirm button", false); }
            }
            else
            {
                clsReportResult.fnLog("Start New Intake", "The intake cannot start since was not found.", "Fail", true, true);
                blResult = false;
            }

            return blResult;
        }

        public bool fnDuplicateClaimPage(clsData pobjData, bool blNext = true)
        {
            bool blResult = true;
            clsReportResult.fnLog("Duplicate Claim Function", "<<<<<<<<<< The Duplicate Claim Functions Starts. >>>>>>>>>>", "Info", false);
            if (clsConstants.blTrainingMode)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                clsMG.fnSwitchToWindow("Intake");
            }
            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Intake", false, false);
            if (clsMG.IsElementPresent("//span[contains(text(), 'Duplicate Claim')]"))
            {
                //Verify Preview Mode
                if (pobjData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "TRUE" || pobjData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "YES")
                {
                    clsMG.fnGoTopPage();
                    if (clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                    {
                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label was displayed in the Duplicated Claim Check Page as expected.", "Pass", true, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should be displayed in the Duplicated Claim Check Page but was not found.", "Fail", true, false);
                        blResult = false;
                    }
                }
                else if (pobjData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "FALSE" || pobjData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "NO")
                {
                    clsMG.fnGoTopPage();
                    if (!clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                    {
                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label is not displayed as expected in the Duplicated Claim Check Page.", "Pass", true, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should not be displayed in the Duplicated Claim Check Page for this user role.", "Fail", true, false);
                        blResult = false;
                    }
                }
                clsWE.fnPageLoad(clsWE.fnGetWe("//span[@data-bind='text:Value.Label']"), "Header Intake", false, false);
                clsMG.fnCleanAndEnterText("Loss Time", "//div[@class='row' and div[span[text()='Loss Time']]]//input[@class='form-control']", pobjData.fnGetValue("LossTime", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//span[@data-bind='text:Value.Label']"), "Header Intake", false);
                clsMG.fnCleanAndEnterText("Loss Date", "//div[@class='row' and div[span[text()='Loss Date']]]//input[@class='form-control']", pobjData.fnGetValue("LossDate", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reporter Type", "//div[@class='row' and div[span[text()='Reporter Type']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReporterType", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reported By", "//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReportedBy", ""), false, false, "", false);
                clsWE.fnPageLoad(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Move Focus to Header", false);
                //Fill EE Lookup
                if (pobjData.fnGetValue("FillEELookup", "").ToUpper() == "TRUE") { blResult = fnEmployeeLocationLookup(pobjData.fnGetValue("EELookupSet", "")); }
                //FIll Location Lookup
                if (pobjData.fnGetValue("FillLocLookup", "").ToUpper() == "TRUE") { blResult = fnLocationLookup(pobjData.fnGetValue("LocLookupSet", "")); }
                //Verify if error messages exist
                if (blNext)
                {
                    clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next Button", false);
                    if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']") || !clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                    {
                        clsReportResult.fnLog("Duplicate Claim Check", "The Duplicate Claim Check Page was filled successfully.", "Pass", false, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Duplicate Claim Check", "Some errors were found in Duplicate Claim Check Page and test cannot continue", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            else
            {
                clsReportResult.fnLog("Duplicate Claim Check", "The Duplicate Claim Check Page was not loaded as expected.", "Fail", true, false);
                blResult = false;
            }

            return blResult;
        }

        public bool fnDuplicateClaimPageIntakeScreen(clsData pobjData, bool blNext = true)
        {
            bool blResult = true;
            clsReportResult.fnLog("Duplicate Claim Function", "<<<<<<<<<< The Duplicate Claim Functions Starts. >>>>>>>>>>", "Info", false);
            if (clsConstants.blTrainingMode)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                clsMG.fnSwitchToWindow("Intake");
            }
            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Intake", false, false);
            if (clsMG.IsElementPresent("//span[contains(text(), 'Duplicate Claim')]"))
            {
                //Verify Action
                switch (pobjData.fnGetValue("Action", "").ToUpper())
                {
                    case "VERIFYDOL":
                        clsReportResult.fnLog("Preview Mode Label", "The DOL Verification Starts on Duplicated Claim Check.", "Info", false, false);
                        blResult = VerifyDOLElement("");
                        break;
                    case "VERIFYPREVIEWMODE":
                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label verification starts on Duplicated Claim check.", "Info", false, false);
                        if (pobjData.fnGetValue("ActionValues", "").ToUpper() == "TRUE" || pobjData.fnGetValue("ActionValues", "").ToUpper() == "YES")
                        {
                            blResult = clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                            string strMessage = blResult ? "was displayed in the Duplicated Claim Check Page as expected." : "should be displayed in the Duplicated Claim Check Page but was not found.";
                            clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                        }
                        else if (pobjData.fnGetValue("ActionValues", "").ToUpper() == "FALSE" || pobjData.fnGetValue("ActionValues", "").ToUpper() == "NO")
                        {
                            clsMG.fnGoTopPage();
                            blResult = !clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                            string strMessage = blResult ? "is not displayed as expected in the Duplicated Claim Check Page." : "should not be displayed in the Duplicated Claim Check Page for this user role.";
                            clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);

                        }
                        break;
                    case "VERIFYTRAININGMODE":
                        break;
                }

                clsWE.fnPageLoad(clsWE.fnGetWe("//span[@data-bind='text:Value.Label']"), "Header Intake", false, false);
                clsMG.fnCleanAndEnterText("Loss Time", "//div[@class='row' and div[span[text()='Loss Time']]]//input[@class='form-control']", pobjData.fnGetValue("LossTime", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//span[@data-bind='text:Value.Label']"), "Header Intake", false);
                clsMG.fnCleanAndEnterText("Loss Date", "//div[@class='row' and div[span[text()='Loss Date']]]//input[@class='form-control']", pobjData.fnGetValue("LossDate", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reporter Type", "//div[@class='row' and div[span[text()='Reporter Type']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReporterType", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reported By", "//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReportedBy", ""), false, false, "", false);
                clsWE.fnPageLoad(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Move Focus to Header", false);
                //Fill EE Lookup
                if (pobjData.fnGetValue("FillEELookup", "").ToUpper() == "TRUE") { blResult = fnEmployeeLocationLookup(pobjData.fnGetValue("EELookupSet", "")); }
                //FIll Location Lookup
                if (pobjData.fnGetValue("FillLocLookup", "").ToUpper() == "TRUE") { blResult = fnLocationLookup(pobjData.fnGetValue("LocLookupSet", "")); }
                //Verify if error messages exist
                if (blNext)
                {
                    clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next Button", false);
                    if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']") || !clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                    {
                        clsReportResult.fnLog("Duplicate Claim Check", "The Duplicate Claim Check Page was filled successfully.", "Pass", false, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Duplicate Claim Check", "Some errors were found in Duplicate Claim Check Page and test cannot continue", "Fail", true, false);
                        blResult = false;
                    }
                }

                //Verify Action
                switch (pobjData.fnGetValue("Action", "").ToUpper())
                {
                    case "VERIFYDOL":
                        clsReportResult.fnLog("Preview Mode Label", "The DOL Verification After Fill Data Starts on Duplicated Claim Check.", "Info", false, false);
                        blResult = VerifyDOLElement(pobjData.fnGetValue("LossDate", ""));
                        break;
                }

            }
            else
            {
                clsReportResult.fnLog("Duplicate Claim Check", "The Duplicate Claim Check Page was not loaded as expected.", "Fail", true, false);
                blResult = false;
            }

            return blResult;
        }
        public bool fnEmployeeLocationLookup(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Employee Lookup", "Employee Lookup Function Starts.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "EELookup");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Verify if EE Lookup button exist
                    clsReportResult.fnLog("Employee Lookup Function", "<<<<<<<<<< The Employee Functions Starts. >>>>>>>>>>", "Info", false);
                    if (clsMG.IsElementPresent("//button[span[text()='Employee Lookup']]"))
                    {
                        //Verify EE Lookup Popup is displayed
                        clsWE.fnClick(clsWE.fnGetWe("//button[span[text()='Employee Lookup']]"), "Employee Lookup Button", false);
                        if (clsMG.IsElementPresent("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]"))
                        {
                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]"), "Employee Lookup Popup", false, false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisEmployeeResults_EMPLOYEE_LOOKUP_info']"), "Employee Table", true, false);
                            //Select Data Set
                            if (objData.fnGetValue("Set", "") != "0")
                            {
                                clsMG.fnCleanAndEnterText("Employee ID", "//input[@id='search-empId']", objData.fnGetValue("EmpID", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("SSN", "//input[@id='search-ssn']", objData.fnGetValue("SSN", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("FirstName", "//input[@id='search-firstName']", objData.fnGetValue("FirstName", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("LastName", "//input[@id='search-lastName']", objData.fnGetValue("LastName", ""), false, false, "", false);
                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search Button", false);
                                //Select Employee
                                if (clsMG.IsElementPresent("//table[@id='jurisEmployeeResults_EMPLOYEE_LOOKUP']//tr[td[text()='" + objData.fnGetValue("EmpID", "") + "']]//button"))
                                {
                                    clsReportResult.fnLog("Employee Lookup", "The Employee Record was found as expected.", "Pass", true, false);
                                    clsWE.fnClick(clsWE.fnGetWe("//table[@id='jurisEmployeeResults_EMPLOYEE_LOOKUP']//tr[td[text()='" + objData.fnGetValue("EmpID", "") + "']]//button"), "Select Employee Record", false);
                                }
                                else
                                {
                                    clsReportResult.fnLog("Employee Lookup", "The Employee Record was not found.", "Fail", true, false);
                                    blResult = false;
                                }
                            }
                            //Close Popup
                            if (clsMG.IsElementPresent("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]"))
                            { clsWE.fnClick(clsWE.fnGetWe("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]//button[text()='Close']"), "Close Button", false); }
                        }
                        else
                        {
                            clsReportResult.fnLog("Employee Lookup", "The Employee Lookup Page was not loaded successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Employee Lookup", "The Employee Lookup button was not found and cannot be filled.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            clsWE.fnPageLoad(clsWE.fnGetWe("//*[@data-bind='text: IntakeName']"), "Wait to load previous screen.", false, false);
            return blResult;
        }

        public bool fnLocationLookup(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Location Lookup", "Location Lookup Function Starts.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "LocLookup");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Verify if Location Lookup button exist
                    clsReportResult.fnLog("Location Lookup Function", "<<<<<<<<<< The Location Lookup Functions Starts. >>>>>>>>>>", "Info", false);
                    clsMG.WaitWEUntilAppears("Waiting Location Lookup button", "//button[span[text()='Location Lookup']]", 10);
                    if (clsMG.IsElementPresent("//button[span[text()='Location Lookup']]"))
                    {
                        //Verify Location Lookup Popup is displayed
                        clsWE.fnClick(clsWE.fnGetWe("//button[span[text()='Location Lookup']]"), "Location Lookup Button", false);
                        if (clsMG.IsElementPresent("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"))
                        {
                            //Wait to Load Location Lookup
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"), "Location Lookup Popup", false, false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"), "Location Table", true, false);
                            //Verify is table is empty
                            IList<IWebElement> lsRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr"));
                            if (lsRows.Count() > 2)
                            {
                                //Select especific location
                                if (objData.fnGetValue("Set", "") != "0")
                                {
                                    clsMG.fnCleanAndEnterText("Account Name", "//input[@id='search-accountName']", objData.fnGetValue("AccountName", ""), false, false, "", false);
                                    clsMG.fnCleanAndEnterText("Account Number", "//input[@id='search-accountNumber']", objData.fnGetValue("AccountNumber", ""), false, false, "", false);
                                    clsMG.fnCleanAndEnterText("Unit Name", "//input[@id='search-unitName']", objData.fnGetValue("UnitName", ""), false, false, "", false);
                                    clsMG.fnCleanAndEnterText("Unit Number", "//input[@id='search-unitNumber']", objData.fnGetValue("UnitNumber", ""), false, false, "", false);
                                    clsMG.fnCleanAndEnterText("Address", "//input[@id='search-address']", objData.fnGetValue("Address", ""), false, false, "", false);
                                    clsMG.fnCleanAndEnterText("City", "//input[@id='search-city']", objData.fnGetValue("City", ""), false, false, "", false);
                                    clsMG.fnSelectDropDownWElm("State", "//select[contains(@data-bind, 'SearchParameters.State')]", objData.fnGetValue("State", ""), false, false);
                                    clsMG.fnCleanAndEnterText("City", "//input[@id='search-zipcode']", objData.fnGetValue("ZipCode", ""), true, false, "", false);
                                    clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search Button", false);
                                    //Select Location
                                    if (clsMG.IsElementPresent("(//table[@id='jurisLocationResults_LOCATION_LOOKUP']//button)[1]"))
                                    {
                                        clsReportResult.fnLog("Location Lookup", "The Location Record was found as expected.", "Pass", true, false);
                                        clsWE.fnClick(clsWE.fnGetWe("(//table[@id='jurisLocationResults_LOCATION_LOOKUP']//button)[1]"), "Select Location Record", false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Location Lookup", "The Location Record was not found.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    //Select first existing location
                                    if (clsMG.IsElementPresent("(//table[@id='jurisLocationResults_LOCATION_LOOKUP']//button)[1]"))
                                    {
                                        clsReportResult.fnLog("Location Lookup", "The Location Record was found as expected.", "Pass", true, false);
                                        clsWE.fnClick(clsWE.fnGetWe("(//table[@id='jurisLocationResults_LOCATION_LOOKUP']//button)[1]"), "Select Location Record", false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Location Lookup", "The Location Record was not found.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                if (clsMG.IsElementPresent("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"))
                                { clsWE.fnClick(clsWE.fnGetWe("//button[@id='btn_close_juris'"), "Close Location Lookup", false); }
                            }
                            else
                            {
                                clsReportResult.fnLog("Location Lookup", "The Location Lookup Table does not have records.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Location Lookup", "The Location Lookup Page was not loaded successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Location Lookup", "The Location Lookup button was not found and cannot be filled.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            clsWE.fnPageLoad(clsWE.fnGetWe("//*[@data-bind='text: IntakeName']"), "Wait to load previous screen.", false, false);
            return blResult;
        }

        /*
        public bool fnSearchClaim(clsData pobjData) 
        {
            bool blResult = true;
            //Go to Search Intake and verify that page is loaded
            clsReportResult.fnLog("Search Intake", "The Search Claims Start.", "Info", false, false);
            clsMG.fnHamburgerMenu("Search Intakes");
            if (clsWE.fnElementExist("Search Intake Page", "//h4[text()='Search Intakes']", true))
            {
                //Clear filters
                clsWE.fnClick(clsWE.fnGetWe("//button[@id='primaryClear']"), "Clear Button", false);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                clsMG.fnGoTopPage();
                //Select Client
                if (pobjData.fnGetValue("ClientNo", "") != "" && pobjData.fnGetValue("ClientName", "") != "") 
                {
                    //Select Client
                    blResult = fnSelectClientPopup(pobjData.fnGetValue("ClientNo", ""), pobjData.fnGetValue("ClientName", ""));
                }
                //Populate Search Criteria
                clsMG.fnCleanAndEnterText("Confirmation Number", "//input[contains(@data-bind, 'ConfirmationNumber')]", pobjData.fnGetValue("ConfNo", ""), false, false, "", false);
                clsMG.fnCleanAndEnterText("Claim Number", "//input[contains(@data-bind, 'VendorIncidentNumber')]", pobjData.fnGetValue("ClaimNumber", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Line of Business", "//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input", pobjData.fnGetValue("LOB", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Status", "//div[select[contains(@data-bind, 'SearchParameters.Status')]]//input", pobjData.fnGetValue("Status", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Search')]"), "Search", false);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            else
            {
                clsReportResult.fnLog("Search Intake", "The Search Page was not loaded successfully.", "Fail", true, false);
                blResult = false;
            }

            return blResult;
        }
        */


        /*
        public bool fnSearchClaim(string pstrConfNo, string pstrClaimNumber, string pstrLOB, string pstrStatus)
        {
            bool blResult = true;
            //Go to Search Intake and verify that page is loaded
            clsReportResult.fnLog("Search Intake", "The Search Claims Start.", "Info", false, false);
            clsMG.fnHamburgerMenu("Search Intakes");
            if (clsWE.fnElementExist("Search Intake Page", "//h4[text()='Search Intakes']", true))
            {
                //Populate Search Criteria
                clsMG.fnCleanAndEnterText("Confirmation Number", "//input[contains(@data-bind, 'ConfirmationNumber')]", pstrConfNo, false, false, "", false);
                clsMG.fnCleanAndEnterText("Claim Number", "//input[contains(@data-bind, 'VendorIncidentNumber')]", pstrClaimNumber, false, false, "", false);
                clsMG.fnSelectDropDownWElm("Line of Business", "//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input", pstrLOB, false, false, "", false);
                clsMG.fnSelectDropDownWElm("Status", "//div[select[contains(@data-bind, 'SearchParameters.Status')]]//input", pstrStatus, false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Search')]"), "Search", false);
            }
            else
            {
                clsReportResult.fnLog("Search Intake", "The Search Page was not loaded successfully.", "Fail", true, false);
                blResult = false;
            }

            return blResult;
        }

        */

        public void fnCloseLocationLookupPopup()
        {
            if (clsMG.IsElementPresent("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]"))
            { clsWE.fnClick(clsWE.fnGetWe("//button[@id='btn_close_juris']"), "Close Location Lookup", false); }
        }

        public bool fnAccountUnitSecurityVerification(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Account Unit Security", "<<<<<<<<<< Account Unit Security Function Starts. >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "AccUnitSec");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Select Location Lookup or Search Validation
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake
                        fnStartNewIntake(objData.fnGetValue("IntakeName", ""));
                        clsReportResult.fnLog("Account Unit Security", "--->> The Location Lookup Account/Unit Verification Start.", "Info", false);
                        clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Duplicate Claim Page", false, false);
                        if (clsWE.fnElementExist("Duplicate Claim Check Page", "//span[contains(text(), 'Duplicate Claim')]", true))
                        {
                            //Verify Location Lookup Popup was opened successfully
                            clsWE.fnClick(clsWE.fnGetWe("//button[@id='btnJurisLocation_LOCATION_LOOKUP']"), "Location Lookup Button", false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            if (clsWE.fnElementExist("Location Lookup Popup", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]", true))
                            {
                                //Verify the account or unit in the table
                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]"), "Location Lookup Page", false, false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']"), "Location Lookup Values", false, false);
                                //verify that all records in the first page has the correct values
                                if (fnAccountUnitTableRows(objData.fnGetValue("ScenarioType", ""), objData.fnGetValue("RestrictionType", ""), objData.fnGetValue("AccUnitVal", "")))
                                {
                                    //Apply the filter and verify that the table display the correct values
                                    if (objData.fnGetValue("RestrictionType", "").Contains("Unit"))
                                    { clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Unit Name')])[1]"), "Unit Name Sorting", false); }
                                    else
                                    { clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Account Name')])[1]"), "Account Name Sorting", false); }
                                    Thread.Sleep(TimeSpan.FromSeconds(3));
                                    if (fnAccountUnitTableRows(objData.fnGetValue("ScenarioType", ""), objData.fnGetValue("RestrictionType", ""), objData.fnGetValue("AccUnitVal", "")))
                                    {
                                        clsReportResult.fnLog("Location Lookup Popup", "The Account/Unit records in Location Lookup was verified successfully.", "Pass", false, false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Location Lookup Popup", "An Invalid Account/Unit was identified after apply a sorting in the table.", "Fail", false, false);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    clsReportResult.fnLog("Location Lookup Popup", "An Invalid Account/Unit was identified in the record of the table.", "Fail", false, false);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("Location Lookup Popup", "The Location Lookup Popup was not loaded correctly and test cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Duplicate Claim Check Page", "The Duplicate Claim Page was not loaded correctly and test cannot continue.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else { blResult = false; }
                    fnCloseLocationLookupPopup();



                    /*
                    switch (objData.fnGetValue("Action", "").ToUpper())
                    {
                        case "LOCATIONLOOKUP":
                            //Verify that it goes to Select Intake Page
                            if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                            {
                                //Start Intake
                                fnStartNewIntake(objData.fnGetValue("IntakeName", ""));
                                clsReportResult.fnLog("Account Unit Security", "--->> The Location Lookup Account/Unit Verification Start.", "Info", false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Duplicate Claim Page", false, false);
                                if (clsWE.fnElementExist("Duplicate Claim Check Page", "//span[contains(text(), 'Duplicate Claim')]", true))
                                {
                                    //Verify Location Lookup Popup was opened successfully
                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='btnJurisLocation_LOCATION_LOOKUP']"), "Location Lookup Button", false);
                                    Thread.Sleep(TimeSpan.FromSeconds(3));
                                    if (clsWE.fnElementExist("Location Lookup Popup", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]", true))
                                    {
                                        //Verify the account or unit in the table
                                        clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]"), "Location Lookup Page", false, false);
                                        clsWE.fnPageLoad(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']"), "Location Lookup Values", false, false);
                                        //verify that all records in the first page has the correct values
                                        if (fnAccountUnitTableRows(objData.fnGetValue("ScenarioType", ""), objData.fnGetValue("RestrictionType", ""), objData.fnGetValue("AccUnitVal", "")))
                                        {
                                            //Apply the filter and verify that the table display the correct values
                                            if (objData.fnGetValue("RestrictionType", "").Contains("Unit")) 
                                            { clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Unit Name')])[1]"), "Unit Name Sorting", false); }
                                            else 
                                            { clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Account Name')])[1]"), "Account Name Sorting", false); }
                                            Thread.Sleep(TimeSpan.FromSeconds(3));
                                            if (fnAccountUnitTableRows(objData.fnGetValue("ScenarioType", ""), objData.fnGetValue("RestrictionType", ""), objData.fnGetValue("AccUnitVal", "")))
                                            {
                                                clsReportResult.fnLog("Location Lookup Popup", "The Account/Unit records in Location Lookup was verified successfully.", "Pass", false, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Location Lookup Popup", "An Invalid Account/Unit was identified after apply a sorting in the table.", "Fail", false, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Location Lookup Popup", "An Invalid Account/Unit was identified in the record of the table.", "Fail", false, false);
                                            blResult = false;
                                        }
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Location Lookup Popup", "The Location Lookup Popup was not loaded correctly and test cannot continue.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    clsReportResult.fnLog("Duplicate Claim Check Page", "The Duplicate Claim Page was not loaded correctly and test cannot continue.", "Fail", true, false);
                                    blResult = false;
                                }
                            }
                            else { blResult = false; }
                            fnCloseLocationLookupPopup();
                            break;
                        case "SEARCH":
                            clsReportResult.fnLog("Account Unit Security", "--->> The Search Claim Account/Unit Verification Start.", "Info", false);
                            if (fnSearchClaim(objData))
                            {
                                //verify Results
                                switch (objData.fnGetValue("ScenarioType", "").ToUpper()) 
                                {
                                    case "POSITIVE":
                                        if (clsWE.fnElementExist("Search Intake Value", "//tr[td[text() = '" + objData.fnGetValue("ClaimNumber", "") + "']]", false))
                                        { clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNumber", "").ToString() + " is displayed as expected.", "Pass", true); }
                                        else
                                        {
                                            clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNumber", "").ToString() + " should be displayed for this user.", "Fail", true);
                                            blResult = false;
                                        }
                                        break;
                                    case "NEGATIVE":
                                        if (clsWE.fnElementExist("Search Intake Value", "//td[contains(text(), 'No data available in table')]", false))
                                        { clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNumber", "").ToString() + " is not displayed as expected.", "Pass", true); }
                                        else
                                        {
                                            clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNumber", "").ToString() + " should not be displayed for this user.", "Fail", true);
                                            blResult = false;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("Account Unit Security", "The Search Intake Page was not loaded successfully and test cannot continue.", "Fail", false);
                                blResult = false;
                            }
                            break;
                    }
                    */
                }
            }
            if (blResult)
            { clsReportResult.fnLog("Account Unit Security", "The Account Unit Security function was executed successfully.", "Pass", false); }
            else
            { clsReportResult.fnLog("Account Unit Security", "The Account Unit Security function was not executed successfully.", "Fail", false); }

            return blResult;
        }

        public bool fnPolicyLookupVerification(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("", "", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "PolicyLookup");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Select Client
                    clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup Verification Function Starts.", "Info", false);
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake and go to Duplicate Claim Check
                        if (fnStartNewIntake(objData.fnGetValue("LOB", "")))
                        {
                            if (fnDuplicateClaimPage(objData, false))
                            {
                                //Verify Policy Case
                                switch (objData.fnGetValue("PolicyType", "").ToUpper())
                                {
                                    case "SPSS":
                                        if (clsWE.fnElementExist("Policy Button", "//span[contains(text(), 'Policy Search')]", true))
                                        {
                                            clsWE.fnClick(clsWE.fnGetWe("//span[contains(text(), 'Policy Search')]"), "Click Policy Search", false);
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"), "Policy Popup", false, false);
                                            if (clsWE.fnElementExist("Loading element", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]", true, false))
                                            {
                                                //Provide Policy Information
                                                while (!clsMG.IsElementPresent("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"))
                                                { Thread.Sleep(TimeSpan.FromSeconds(2)); }
                                                //Thread.Sleep(TimeSpan.FromSeconds(5));
                                                //clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"), "Policy Table", false, false);
                                                //clsMG.WaitWEUntilAppears("Wait Table", "//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']", 10);

                                                IWebElement objElement;
                                                if (objData.fnGetValue("ScenarioType", "").ToUpper() == "POSITIVE")
                                                { objElement = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[2]")); }
                                                else
                                                { objElement = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//td[@class='dataTables_empty']")); }

                                                Actions action = new Actions(clsWebBrowser.objDriver);
                                                objElement.Click();
                                                action.SendKeys(Keys.Home).Perform();
                                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                                //Enter Policy
                                                clsMG.fnCleanAndEnterText("Policy Number", "//input[@id='search-policyNumber']", objData.fnGetValue("PolicyNo", ""), false, false, "", false);
                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search button", true);
                                                Thread.Sleep(TimeSpan.FromSeconds(3));
                                                switch (objData.fnGetValue("ScenarioType", "").ToUpper())
                                                {
                                                    case "POSITIVE":

                                                        if (clsWE.fnElementExist("Policy Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[td[text()='" + objData.fnGetValue("PolicyNo", "") + "']]", false, false))
                                                        {
                                                            //Select Policy
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The SPSS Policy: " + objData.fnGetValue("PolicyNo", "").ToString() + " was found as expected.", "Pass", true);
                                                            clsWE.fnClick(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[td[text()='" + objData.fnGetValue("PolicyNo", "") + "']]//button"), "Select Policy", false);
                                                            //Verify Blue Toast Message
                                                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[contains(@data-bind, 'Answer.ContractNumber()')]"), "Blue Toast Message", false, false);
                                                            if (clsWE.fnGetAttribute(clsWE.fnGetWe("//span[contains(@data-bind,'Answer.PolicyNumber')]"), "Get policy", "innerText", false, false) == objData.fnGetValue("PolicyNo", ""))
                                                            {
                                                                clsReportResult.fnLog("Policy Lookup Verification", "The SPSS Policy " + objData.fnGetValue("PolicyNo", "").ToString() + " was displayed on Duplicate Claim Check Page as expected.", "Pass", true);
                                                                //Go to Intake Flow
                                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next button", false);
                                                                if (clsMG.IsElementPresent("//button[@id='start-intake']"))
                                                                {
                                                                    IWebElement objWeBar = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
                                                                    action = new Actions(clsWebBrowser.objDriver);
                                                                    objWeBar.Click();
                                                                    action.SendKeys(Keys.Home).Perform();
                                                                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='start-intake']"), "Start Intake Button", false, false);
                                                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                                                }
                                                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='page-container']"), "Intake FLow Page", false, false);
                                                                //Click on Client Location Information
                                                                clsWE.fnClick(clsWE.fnGetWe("//div[@id='list-example']//span[contains(text(), 'Client/Location Information')]"), "Client Location Information", false);
                                                                Thread.Sleep(TimeSpan.FromSeconds(5));
                                                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//span[contains(@data-bind,'Answer.PolicyNumber')]"), "Get policy", "innerText", false, false) == objData.fnGetValue("PolicyNo", ""))
                                                                {
                                                                    clsWE.fnScrollTo(clsWE.fnGetWe("//span[contains(@data-bind,'Answer.PolicyNumber')]"), "Scroll to Policy", false, false);
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The Policy " + objData.fnGetValue("PolicyNo", "").ToString() + " was displayed on Intake Flow Page as expected", "Pass", true);
                                                                }
                                                                else
                                                                {
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The Policy " + objData.fnGetValue("PolicyNo", "").ToString() + " was not displayed on Intake Flow Page as expected", "Fail", false);
                                                                    blResult = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                clsReportResult.fnLog("Policy Lookup Verification", "--->> The policy displayed on screen does not match.", "Fail", false);
                                                                blResult = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "No SPSS Policy records were retrieved in the table.", "Fail", false);
                                                        }
                                                        break;
                                                    case "NEGATIVE":
                                                        IList<IWebElement> lsRow = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr"));
                                                        if (lsRow.Count > 2)
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The SPSS Policy should not be returned when DOL is not provided or is invalid.", "Fail", true, false);
                                                            blResult = false;
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The SPSS Policies are not returned when DOL is not provied or out of date range as expected.", "Pass", true, false);
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup Popup was not displayed and test cannot continue.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup button is not displayed on the screen and test cannot continue.", "Fail", true, false);
                                            blResult = false;
                                        }

                                        break;
                                    case "DATABASE":
                                        //Enter Exta Data
                                        clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false);
                                        clsMG.fnCleanAndEnterText("What Is The Policy Number", "//div[@class='row' and div[span[text()='What Is The Policy Number? (Provided By Reporter)']]]//input", objData.fnGetValue("PolicyProvided", ""), false, false, "", true);
                                        clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false);
                                        Thread.Sleep(TimeSpan.FromSeconds(3));
                                        clsMG.fnCleanAndEnterText("DBA Name Given By Caller", "//div[@class='row' and div[span[text()='DBA Name Given By Caller']]]//input", objData.fnGetValue("DBAName", ""), false, false, "", true);
                                        if (clsWE.fnElementExist("Policy Button", "//button[@id='btnJurisLocation_LOCATION_LOOKUP']", true))
                                        {
                                            clsWE.fnClick(clsWE.fnGetWe("//button[@id='btnJurisLocation_LOCATION_LOOKUP']"), "Click Policy Search", false);
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]//span[text()='Policy Lookup']"), "Policy Popup", false, false);
                                            if (clsWE.fnElementExist("Loading element", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]//span[text()='Policy Lookup']", true, false))
                                            {
                                                //Provide Policy Information
                                                while (!clsMG.IsElementPresent("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"))
                                                { Thread.Sleep(TimeSpan.FromSeconds(2)); }
                                                //Thread.Sleep(TimeSpan.FromSeconds(5));
                                                //clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"), "Policy Table", false, false);
                                                //clsMG.WaitWEUntilAppears("Wait Table", "//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']", 10);

                                                IWebElement objElement;
                                                if (objData.fnGetValue("ScenarioType", "").ToUpper() == "POSITIVE")
                                                { objElement = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[2]")); }
                                                else
                                                { objElement = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//td[@class='dataTables_empty']")); }

                                                Actions action = new Actions(clsWebBrowser.objDriver);
                                                objElement.Click();
                                                action.SendKeys(Keys.Home).Perform();
                                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                                //Enter Policy
                                                clsMG.fnCleanAndEnterText("Policy Number", "//input[@id='search-policyNumber']", objData.fnGetValue("PolicyNo", ""), false, false, "", false);
                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search button", true);
                                                Thread.Sleep(TimeSpan.FromSeconds(3));
                                                switch (objData.fnGetValue("ScenarioType", "").ToUpper())
                                                {
                                                    case "POSITIVE":

                                                        if (clsWE.fnElementExist("Policy Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[td[text()='" + objData.fnGetValue("PolicyNo", "") + "']]", false, false))
                                                        {
                                                            //Select Policy
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The DataBase Policy: " + objData.fnGetValue("PolicyNo", "").ToString() + " was found as expected.", "Pass", true);
                                                            clsWE.fnClick(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[td[text()='" + objData.fnGetValue("PolicyNo", "") + "']]//button"), "Select Policy", false);
                                                            //Verify Blue Toast Message
                                                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[contains(@data-bind, 'Answer.ContractNumber()')]"), "Blue Toas Message", false, false);
                                                            if (clsWE.fnGetAttribute(clsWE.fnGetWe("//span[contains(@data-bind,'Answer.PolicyNumber')]"), "Get policy", "innerText", false, false) == objData.fnGetValue("PolicyNo", ""))
                                                            {
                                                                clsReportResult.fnLog("Policy Lookup Verification", "The DataBase Policy " + objData.fnGetValue("PolicyNo", "").ToString() + " was displayed on Duplicate Claim Check Page as expected.", "Pass", true);
                                                                //Go to Intake Flow
                                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next button", false);
                                                                if (clsMG.IsElementPresent("//button[@id='start-intake']"))
                                                                {
                                                                    IWebElement objWeBar = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
                                                                    action = new Actions(clsWebBrowser.objDriver);
                                                                    objWeBar.Click();
                                                                    action.SendKeys(Keys.Home).Perform();
                                                                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='start-intake']"), "Start Intake Button", false, false);
                                                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                                                }
                                                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='page-container']"), "Intake FLow Page", false, false);
                                                                //Click on Client Location Information
                                                                clsWE.fnClick(clsWE.fnGetWe("//div[@id='list-example']//span[contains(text(), 'Client/Location Information')]"), "Client Location Information", false);
                                                                Thread.Sleep(TimeSpan.FromSeconds(5));
                                                                //Verify What Is The Policy Number? (Provided By Reporter)
                                                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//div[@class='row' and div[span[text()='What Is The Policy Number? (Provided By Reporter)']]]//input"), "Get policy Reporter", "value", false, false) == objData.fnGetValue("PolicyProvided", ""))
                                                                {
                                                                    clsWE.fnScrollTo(clsWE.fnGetWe("//div[@class='row' and div[span[text()='What Is The Policy Number? (Provided By Reporter)']]]//input"), "Scroll to Policy", false, false);
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The What Is The Policy Number? (Provided By Reporter): " + objData.fnGetValue("PolicyProvided", "").ToString() + " was displayed on Intake Flow Page as expected.", "Pass", true);
                                                                }
                                                                else
                                                                {
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The What Is The Policy Number? (Provided By Reporter): " + objData.fnGetValue("PolicyProvided", "").ToString() + " was not displayed on Intake Flow Page as expected.", "Fail", true);
                                                                    blResult = false;
                                                                }
                                                                //DBA Name Given By Caller
                                                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//div[@class='row' and div[span[text()='DBA Name Given By Caller']]]//input"), "Get DBA Name Given By Caller", "value", false, false) == objData.fnGetValue("DBAName", ""))
                                                                {
                                                                    clsWE.fnScrollTo(clsWE.fnGetWe("//div[@class='row' and div[span[text()='DBA Name Given By Caller']]]//input"), "Scroll to DBA Name Given By Caller", false, false);
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The DBA Name Given By Caller: " + objData.fnGetValue("DBAName", "").ToString() + " was displayed on Intake Flow Page as expected.", "Pass", true);
                                                                }
                                                                else
                                                                {
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The DBA Name Given By Caller: " + objData.fnGetValue("DBAName", "").ToString() + " was not displayed on Intake Flow Page as expected.", "Fail", true);
                                                                    blResult = false;
                                                                }
                                                                //Policy Number (Provided By Search)
                                                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//div[@class='row' and div[span[text()='Policy Number (Provided By Search)']]]//input"), "Get Policy Number (Provided By Search)", "value", false, false) == objData.fnGetValue("PolicyNo", ""))
                                                                {
                                                                    clsWE.fnScrollTo(clsWE.fnGetWe("//div[@class='row' and div[span[text()='Policy Number (Provided By Search)']]]//input"), "Scroll to Policy Number (Provided By Search)", false, false);
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The Policy Number (Provided By Search): " + objData.fnGetValue("PolicyNo", "").ToString() + " was displayed on Intake Flow Page as expected.", "Pass", true);
                                                                }
                                                                else
                                                                {
                                                                    clsReportResult.fnLog("Policy Lookup Verification", "The Policy Number (Provided By Search): " + objData.fnGetValue("PolicyNo", "").ToString() + " was not displayed on Intake Flow Page as expected.", "Fail", true);
                                                                    blResult = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                clsReportResult.fnLog("Policy Lookup Verification", "The DataBase policy displayed on screen does not match.", "Fail", false);
                                                                blResult = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "No DataBase Policy records were retrieved in the table.", "Fail", false);
                                                        }
                                                        break;
                                                    case "NEGATIVE":
                                                        IList<IWebElement> lsRow = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr"));
                                                        if (lsRow.Count > 2)
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The DataBase Policy should not be returned when DOL is not provided or is invalid.", "Fail", true, false);
                                                            blResult = false;
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Policy Lookup Verification", "The DataBase Policies are not returned when DOL is not provied or out of date range as expected.", "Pass", true, false);
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup Popup was not displayed and test cannot continue.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup button is not displayed on the screen and test cannot continue.", "Fail", true, false);
                                            blResult = false;
                                        }
                                        break;
                                }
                                //Close Popup
                                if (clsMG.IsElementPresent("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"))
                                {
                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='btn_close_juris']"), "Close Policy Popup", false);
                                    Thread.Sleep(TimeSpan.FromSeconds(2));
                                }

                            }
                            else
                            {
                                clsReportResult.fnLog("Policy Lookup Verification", "The Duplicated Claim Check was not successfully and claim creation cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup cannot continue since the claim cannot start successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Policy Lookup Verification", "The Policy Lookup cannot was not able to select the client and cannot continue.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            return blResult;

        }

        public bool fnAccountUnitTableRows(string pstrType, string pstrAccUnit, string pstrExpectedVal)
        {
            //Get WebElements and Review the expected value
            bool blResult = true;
            string strLocator;

            switch (pstrAccUnit.ToUpper())
            {
                case "ACCOUNT":
                    strLocator = "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr//td[9]";
                    blResult = fnReadLocationLookRows(pstrType, pstrAccUnit, pstrExpectedVal, strLocator);
                    break;
                case "UNIT":
                    strLocator = "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr//td[3]";
                    blResult = fnReadLocationLookRows(pstrType, pstrAccUnit, pstrExpectedVal, strLocator);
                    break;
                case "ACCOUNTUNIT":
                    strLocator = "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr//td[9]";
                    string strUit = "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr//td[3]";
                    string[] arrVal = pstrExpectedVal.Split(';');
                    foreach (string strRestriction in arrVal)
                    {
                        if (strRestriction.ToUpper().Contains("ACCOUNT"))
                        { blResult = fnReadLocationLookRows(pstrType, "Account", strRestriction.Replace("Account:=", ""), strLocator); }
                        else
                        { blResult = fnReadLocationLookRows(pstrType, "Unit", strRestriction.Replace("Unit:=", ""), strUit); }
                    }
                    break;
            }

            return blResult;
        }

        public bool fnReadLocationLookRows(string pstrType, string pstrAccUnit, string pstrExpectedVal, string pstrLocator)
        {
            bool blResult = true;
            IList<IWebElement> lsRow = clsWebBrowser.objDriver.FindElements(By.XPath(pstrLocator));
            int intCounter = 0;
            switch (pstrType.ToUpper())
            {
                case "POSITIVE":
                    foreach (IWebElement elmRow in lsRow)
                    {
                        intCounter++;
                        if (elmRow.GetAttribute("innerText") == pstrExpectedVal)
                        { clsReportResult.fnLog("Location Lookup Table", "The " + pstrAccUnit + ": " + pstrExpectedVal + " was found correctly in the row[" + intCounter.ToString() + "].", "Info", false); }
                        else
                        {
                            clsReportResult.fnLog("Location Lookup Table", "The " + pstrAccUnit + ": " + elmRow.GetAttribute("innerText") + " was found but should be: " + pstrExpectedVal + " in row[" + intCounter.ToString() + "]", "Fail", true);
                            blResult = false;
                        }
                    }
                    break;
                case "NEGATIVE":
                    foreach (IWebElement elmRow in lsRow)
                    {
                        intCounter++;
                        if (elmRow.GetAttribute("innerText") != pstrExpectedVal)
                        { clsReportResult.fnLog("Location Lookup Table", "The " + pstrAccUnit + ": " + pstrExpectedVal + " was not found in the row[" + intCounter.ToString() + "] as expected.", "Info", false); }
                        else
                        {
                            clsReportResult.fnLog("Location Lookup Table", "The " + pstrAccUnit + ": " + elmRow.GetAttribute("innerText") + " was found but should not be displayed for a restricted account/unit.", "Fail", true);
                            blResult = false;
                        }
                    }
                    break;
            }
            return blResult;
        }

        public bool fnCreateAndSubmitClaim(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Create Standard Claim", "Create Standard Claim.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "EventInfo");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Select Client
                    clsReportResult.fnLog("Create Claim", "The Create Claim Function Starts.", "Info", false);
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake and go to Duplicate Claim Check
                        if (fnStartNewIntake(objData.fnGetValue("LOB", "")))
                        {
                            //Populate Duplicate Claim Check
                            if (fnDuplicateClaimPage(objData))
                            {
                                if (clsMG.IsElementPresent("//button[@id='start-intake']"))
                                {
                                    IWebElement objWeBar = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
                                    Actions action = new Actions(clsWebBrowser.objDriver);
                                    objWeBar.Click();
                                    action.SendKeys(Keys.Home).Perform();
                                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='start-intake']"), "Start Intake Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                }
                                //Verify Preview Mode
                                if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "TRUE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "YES")
                                {
                                    clsMG.fnGoTopPage();
                                    if (clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                    {
                                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label was displayed in the Intake Flow Page as expected.", "Pass", true, false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should be displayed in the  Intake Flow Page but was not found.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "FALSE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "NO")
                                {
                                    clsMG.fnGoTopPage();
                                    if (!clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                    {
                                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label is not displayed as expected in the Intake Flow Page.", "Pass", true, false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should not be displayed in the Intake Flow Page for this user role.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='page-container']"), "Intake FLow Page", false, false);

                                //Reporter First Name
                                clsMG.fnCleanAndEnterText("First Name", "//div[contains(@question-key, 'CALLER_INFORMATION')]//div[@class='row' and div[span[text()='First Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ReporterFN", ""), false, false, "", false);
                                //Reporter Last Name
                                clsMG.fnCleanAndEnterText("Last Name", "//div[contains(@question-key, 'CALLER_INFORMATION')]//div[@class='row' and div[span[text()='Last Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ReporterLN", ""), false, false, "", false);
                                //Is This The Loss Location? 
                                clsMG.fnSelectDropDownWElm("Is This The Loss Location", "//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("IsTheSameLoc", ""), false, false);

                                //Date Reported To Sedgwick
                                clsMG.fnCleanAndEnterText("Date Reported To Sedgwick", "//div[@class='row' and div[span[text()='Date Reported To Sedgwick']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("DateReportedToSedgwick", ""), false, false, "", false);
                                //Time Reported To Sedgwick
                                clsMG.fnCleanAndEnterText("Time Reported To Sedgwick", "//div[@class='row' and div[span[text()='Time Reported To Sedgwick']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("TimeReportedToSedgwick", ""), false, false, "", false);
                                //Employee Firt Name
                                clsMG.fnCleanAndEnterText("Employee First Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='First Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeFN", ""), false, false, "", true);
                                //Employee Last Name
                                clsMG.fnCleanAndEnterText("Employee Last Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='Last Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeLN", ""), false, false, "", true);
                                //Do You Expect The Team Member To Lose Time From Work?
                                clsMG.fnSelectDropDownWElm("Do You Expect The Team Member To Lose Time From Work?", "//div[@class='row' and div[span[contains(text(), 'Do You Expect The Team Member To Lose Time From Work?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("TeamMemberLossTime", ""), false, false);
                                //Employer Notified Date
                                clsMG.fnCleanAndEnterText("Employer Notified Date", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Employer Notified Date']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployerNotifiedDate", ""), false, false, "", false);
                                //Loss Description 
                                clsMG.fnCleanAndEnterText("Loss Description", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Loss Description']]]//textarea", objData.fnGetValue("LossDescription", ""), false, false, "", false);
                                //Is Contact Same As Caller?
                                clsMG.fnSelectDropDownWElm("Is This The Loss Location", "//div[contains(@question-key, 'CONTACT_INFORMATION')]//div[@class='row' and div[span[contains(text(), 'Is Contact Same As Caller?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("IsSameAsCaller", ""), false, false);
                                //Work Phone Number
                                clsMG.fnCleanAndEnterText("Contact Work Phone", "//div[contains(@question-key, 'CONTACT_INFORMATION')]//div[@class='row' and div[span[text()='Work Phone Number']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ContactWorkPhone", ""), false, false, "", false);

                                //Go to Closing Script Statement
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Next')]"), "Next Button", false, false);
                                if (!clsMG.IsElementPresent("//*[@class='col-md-8 secondary-red']"))
                                {
                                    if (objData.fnGetValue("SubmitClaim", "").ToUpper() == "TRUE" || objData.fnGetValue("SubmitClaim", "").ToUpper() == "YES")
                                    {
                                        //Verify Preview Mode
                                        if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "TRUE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "YES")
                                        {
                                            clsMG.fnGoTopPage();
                                            if (clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label was displayed in the Closing Statement Page as expected.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should be displayed in the Closing Statement Page but was not found.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "FALSE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "NO")
                                        {
                                            clsMG.fnGoTopPage();
                                            if (!clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label is not displayed as expected in the Closing Statement Page.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should not be displayed in the Closing Statement Page for this user role.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        //Submit Claim
                                        clsReportResult.fnLog("Submit Claim", "Submiting Claim Created.", "Info", true, false);
                                        clsWE.fnClick(clsWE.fnGetWe("//button[@id='top-submit']"), "Submit Button", false, false);
                                        if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']"))
                                        {
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//*[text()='Thank you']"), "Thank You Page", false, false);
                                            string strClaimNo = "";
                                            strClaimNo = clsWE.fnGetAttribute(clsWE.fnGetWe("//span[contains(@data-bind, 'VendorIncidentNumber')]"), "Confirmation Number", "innerText");
                                            //Save Confirmation Number
                                            clsData objSaveData = new clsData();
                                            objSaveData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "EventInfo", "ClaimNumber", intRow, strClaimNo);
                                            clsConstants.strSubmitClaimTrainingMode = strClaimNo;
                                            clsReportResult.fnLog("Create Claim", "The claim: " + strClaimNo + " was created successfully.", "Pass", false, false);
                                            //Verify Preview Mode
                                            if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "TRUE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "YES")
                                            {
                                                clsMG.fnGoTopPage();
                                                if (clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                                {
                                                    clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label was displayed in the Submit Statement Page as expected.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should be displayed in the Submit Statement Page but was not found.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                            else if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "FALSE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "NO")
                                            {
                                                clsMG.fnGoTopPage();
                                                if (!clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                                {
                                                    clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label is not displayed as expected in the Submit Statement Page.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should not be displayed in the Submit Statement Page for this user role.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                            //Switch to Defaul Tab
                                            if (clsConstants.blTrainingMode)
                                            {
                                                clsMG.fnSwitchToWindowAndClose(1);
                                                clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles[0]);
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Create Claim", "Some errors were found after submit the claim and the creation cannot continue.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    else
                                    {
                                        string strClaimNo = "";
                                        strClaimNo = clsWE.fnGetAttribute(clsWE.fnGetWe("//span[contains(@data-bind, 'VendorIncidentNumber')]"), "Confirmation Number", "innerText");
                                        //Save Confirmation Number
                                        clsData objSaveData = new clsData();
                                        objSaveData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "EventInfo", "ClaimNumber", intRow, strClaimNo);
                                        clsConstants.strResumeClaimTrainingMode = strClaimNo;
                                        clsReportResult.fnLog("Create Claim", "The resume claim: " + strClaimNo + " was created but not submited.", "Pass", false, false);
                                        //Verify Preview Mode
                                        if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "TRUE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "YES")
                                        {
                                            clsMG.fnGoTopPage();
                                            if (clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label was displayed in the Closing Statement Page as expected.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should be displayed in the Closing Statement Page but was not found.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else if (objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "FALSE" || objData.fnGetValue("VerifyPreviewMode", "").ToUpper() == "NO")
                                        {
                                            clsMG.fnGoTopPage();
                                            if (!clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]"))
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label is not displayed as expected in the Submit Statement Page.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label should not be displayed in the Submit Statement Page for this user role.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        //Switch to Defaul Tab
                                        if (clsConstants.blTrainingMode)
                                        {
                                            clsMG.fnSwitchToWindowAndClose(1);
                                            clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles[0]);
                                        }

                                    }
                                }
                                else
                                {
                                    clsReportResult.fnLog("Create Claim", "Some errors were found after try to go to Closing Script Page and claim creation cannot continue.", "Fail", true, false);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("Create Claim", "The Duplicated Claim Check was not successfully and claim creation cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Create Claim", "The Create Claim cannot continue since the claim cannot start successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Create Claim", "The Create Claim cannot continue since the client was not selected as expected.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            return blResult;
        }


        public bool fnIntakeScreen(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Create Standard Claim", "Create Standard Claim.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "EventInfo");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Select Client
                    clsReportResult.fnLog("Create Claim", "The Create Claim Function Starts.", "Info", false);
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake and go to Duplicate Claim Check
                        if (fnStartNewIntake(objData.fnGetValue("LOB", "")))
                        {
                            //Populate Duplicate Claim Check
                            if (fnDuplicateClaimPageIntakeScreen(objData))
                            {
                                //Go to Start Intake
                                if (clsMG.IsElementPresent("//button[@id='start-intake']"))
                                {
                                    IWebElement objWeBar = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
                                    Actions action = new Actions(clsWebBrowser.objDriver);
                                    objWeBar.Click();
                                    action.SendKeys(Keys.Home).Perform();
                                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='start-intake']"), "Start Intake Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(10));
                                }
                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='page-container']"), "Intake FLow Page", false, false);
                                var actionDriver = objData.fnGetValue("Action");
                                var actions = actionDriver.Split(';').ToList();
                                actions.ForEach(action =>
                                    {
                                        //Select validation
                                        switch (action.ToUpper())
                                        {
                                            case "VERIFYTABBINGORDER":
                                                var labels = clsWebBrowser.objDriver.FindElements(CreateIntakeScreen.objAllLabels)
                                                    .Take(13) //After 13th element it will fail, client 9066
                                                    .ToList();
                                                var firstLabel = labels.First();
                                                new Actions(clsWebBrowser.objDriver).MoveToElement(firstLabel).Build().Perform();
                                                labels.Remove(firstLabel);
                                                labels.ForEach(
                                                    x =>
                                                    {
                                                        var parent = x.fnGetParentNode();
                                                        var question = new
                                                        {
                                                            inputCount = parent.FindElements(By.XPath(".//button | .//select | .//input")).Count(y => y.Enabled && y.Displayed),
                                                            labelText = x.Text
                                                        };

                                                        var result = "Fail";
                                                        for (var i = 0; i < question.inputCount; i++)
                                                        {
                                                            clsWebBrowser.objDriver.FindElement(By.TagName("body")).SendKeys(Keys.Tab);
                                                            var activeElementLabel = this.fnGetActiveElementLabel();
                                                            if (question.labelText.Equals(activeElementLabel))
                                                            {
                                                                result = "Pass";
                                                            }
                                                        }
                                                        clsReportResult.fnLog("Tabbing: Element is active", $"Tabbing: Element '{question.labelText}' is active", result, true);
                                                    }
                                                );
                                                break;
                                            case "VERIFYDOL":
                                                blResult = VerifyDOLElement(objData.fnGetValue("LossDate", ""));
                                                break;
                                            case "VERIFYPREVIEWMODE":
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label verification starts on Intake Flow Screen.", "Info", false, false);
                                                clsMG.fnGoTopPage();
                                                if (objData.fnGetValue("ActionValues", "").ToUpper() == "TRUE" || objData.fnGetValue("ActionValues", "").ToUpper() == "YES")
                                                {
                                                    blResult = clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                    string strMessage = blResult ? "was displayed in the Intake Flow Page as expected." : "should be displayed in the Intake Flow Page but was not found.";
                                                    clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                }
                                                else if (objData.fnGetValue("ActionValues", "").ToUpper() == "FALSE" || objData.fnGetValue("ActionValues", "").ToUpper() == "NO")
                                                {
                                                    clsMG.fnGoTopPage();
                                                    blResult = !clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                    string strMessage = blResult ? "is not displayed as expected in the Intake Flow Page." : "should not be displayed in the Intake Flow Page for this user role.";
                                                    clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                }
                                                break;
                                            case "VERIFYFLOATINGMENUBAR":
                                                IList<IWebElement> lsitemsInMenuBar = clsWebBrowser.objDriver.FindElements(CreateIntakeScreen.objFloatingListSelector);
                                                clsUtils.fnExecuteIf(lsitemsInMenuBar.Count > 0,
                                                    () =>
                                                    {
                                                        foreach (var element in lsitemsInMenuBar)
                                                        {
                                                            element.Click();
                                                            var elementIsActive = clsMG.fnGenericWait(() => element.GetAttribute("class").Contains("active"), TimeSpan.FromSeconds(1), 10);
                                                            if (!elementIsActive)
                                                            {
                                                                clsReportResult.fnLog("Element in floating menu shuld be in active status", element.Text, "Fail", true);
                                                            }
                                                            else
                                                            {
                                                                clsReportResult.fnLog("Element in floating menu is in active status", element.Text, "Pass", true);
                                                            }
                                                        }
                                                    }
                                                );
                                                break;
                                            case "VERIFYCAUSECODES":
                                                blResult = VerifyCodesDropDown(objData.fnGetValue("ActionValues", ""));
                                                break;
                                            default:
                                                //Reporter First Name
                                                clsMG.fnCleanAndEnterText("First Name", "//div[contains(@question-key, 'CALLER_INFORMATION')]//div[@class='row' and div[span[text()='First Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ReporterFN", ""), false, false, "", false);
                                                //Reporter Last Name
                                                clsMG.fnCleanAndEnterText("Last Name", "//div[contains(@question-key, 'CALLER_INFORMATION')]//div[@class='row' and div[span[text()='Last Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ReporterLN", ""), false, false, "", false);
                                                //Is This The Loss Location? 
                                                clsMG.fnSelectDropDownWElm("Is This The Loss Location", "//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("IsTheSameLoc", ""), false, false);
                                                //Date Reported To Sedgwick
                                                clsMG.fnCleanAndEnterText("Date Reported To Sedgwick", "//div[@class='row' and div[span[text()='Date Reported To Sedgwick']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("DateReportedToSedgwick", ""), false, false, "", false);
                                                //Time Reported To Sedgwick
                                                clsMG.fnCleanAndEnterText("Time Reported To Sedgwick", "//div[@class='row' and div[span[text()='Time Reported To Sedgwick']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("TimeReportedToSedgwick", ""), false, false, "", false);
                                                //Employee Firt Name
                                                clsMG.fnCleanAndEnterText("Employee First Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='First Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeFN", ""), false, false, "", true);
                                                //Employee Last Name
                                                clsMG.fnCleanAndEnterText("Employee Last Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='Last Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeLN", ""), false, false, "", true);
                                                //Do You Expect The Team Member To Lose Time From Work?
                                                clsMG.fnSelectDropDownWElm("Do You Expect The Team Member To Lose Time From Work?", "//div[@class='row' and div[span[contains(text(), 'Do You Expect The Team Member To Lose Time From Work?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("TeamMemberLossTime", ""), false, false);
                                                //Employer Notified Date
                                                clsMG.fnCleanAndEnterText("Employer Notified Date", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Employer Notified Date']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployerNotifiedDate", ""), false, false, "", false);
                                                //Loss Description 
                                                clsMG.fnCleanAndEnterText("Loss Description", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Loss Description']]]//textarea", objData.fnGetValue("LossDescription", ""), false, false, "", false);
                                                //Is Contact Same As Caller?
                                                clsMG.fnSelectDropDownWElm("Is This The Loss Location", "//div[contains(@question-key, 'CONTACT_INFORMATION')]//div[@class='row' and div[span[contains(text(), 'Is Contact Same As Caller?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("IsSameAsCaller", ""), false, false);
                                                //Work Phone Number
                                                clsMG.fnCleanAndEnterText("Contact Work Phone", "//div[contains(@question-key, 'CONTACT_INFORMATION')]//div[@class='row' and div[span[text()='Work Phone Number']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("ContactWorkPhone", ""), false, false, "", false);
                                                break;
                                        }
                                    }
                                );

                                //Submit Claim
                                if (objData.fnGetValue("SubmitClaim", "").ToUpper() == "YES" || objData.fnGetValue("SubmitClaim", "").ToUpper() == "TRUE")
                                {
                                    clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Next')]"), "Next Button", false, false);
                                    if (!clsMG.IsElementPresent("//*[@class='col-md-8 secondary-red']"))
                                    {
                                        //Verify Actions
                                        switch (objData.fnGetValue("Action", "").ToUpper())
                                        {
                                            case "VERIFYPREVIEWMODE":
                                                clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label verification starts on Closing Script Screen.", "Info", false, false);
                                                if (objData.fnGetValue("ActionValues", "").ToUpper() == "TRUE" || objData.fnGetValue("ActionValues", "").ToUpper() == "YES")
                                                {
                                                    blResult = clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                    string strMessage = blResult ? "was displayed in the Closing Script Page as expected." : "should be displayed in the Closing Script Page but was not found.";
                                                    clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                }
                                                else if (objData.fnGetValue("ActionValues", "").ToUpper() == "FALSE" || objData.fnGetValue("ActionValues", "").ToUpper() == "NO")
                                                {
                                                    clsMG.fnGoTopPage();
                                                    blResult = !clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                    string strMessage = blResult ? "is not displayed as expected in the Closing Script Page." : "should not be displayed in the Closing Script Page for this user role.";
                                                    clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                }
                                                break;
                                        }

                                        clsReportResult.fnLog("Submit Claim", "Submiting Claim Created.", "Info", true, false);
                                        clsWE.fnClick(clsWE.fnGetWe("//button[@id='top-submit']"), "Submit Button", false, false);
                                        if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']"))
                                        {
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//*[text()='Thank you']"), "Thank You Page", false, false);
                                            string strClaimNo = "";
                                            strClaimNo = fnGetConfirmationNumber();
                                            //Save Confirmation Number
                                            clsData objSaveData = new clsData();
                                            objSaveData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "EventInfo", "ClaimNumber", intRow, strClaimNo);
                                            clsConstants.strSubmitClaimTrainingMode = strClaimNo;
                                            clsReportResult.fnLog("Create Claim", "The claim: " + strClaimNo + " was created successfully.", "Pass", false, false);

                                            switch (objData.fnGetValue("Action", "").ToUpper())
                                            {
                                                case "VERIFYPREVIEWMODE":
                                                    clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label verification starts on Submit Screen.", "Info", false, false);
                                                    if (objData.fnGetValue("ActionValues", "").ToUpper() == "TRUE" || objData.fnGetValue("ActionValues", "").ToUpper() == "YES")
                                                    {
                                                        blResult = clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                        string strMessage = blResult ? "was displayed in the Submit Page as expected." : "should be displayed in the Submit Page but was not found.";
                                                        clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                    }
                                                    else if (objData.fnGetValue("ActionValues", "").ToUpper() == "FALSE" || objData.fnGetValue("ActionValues", "").ToUpper() == "NO")
                                                    {
                                                        clsMG.fnGoTopPage();
                                                        blResult = !clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                        string strMessage = blResult ? "is not displayed as expected in the Submit Page." : "should not be displayed in the Submit Page for this user role.";
                                                        clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Create Claim", "Some errors were found after submit the claim and the creation cannot continue.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Create Claim", "Some errors were found after try to go to Closing Script Page and claim creation cannot continue.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    //Resume Claim
                                    clsReportResult.fnLog("Create Claim", "The claim was left on resume status", "Info", true, false);
                                    string strClaimNo = "";
                                    strClaimNo = fnGetConfirmationNumber();
                                    clsData objSaveData = new clsData();
                                    objSaveData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "EventInfo", "ClaimNumber", intRow, strClaimNo);
                                    clsConstants.strResumeClaimTrainingMode = strClaimNo;
                                    clsReportResult.fnLog("Create Claim", "The resume claim: " + strClaimNo + " was created.", "Pass", true, false);

                                    //Select validation
                                    switch (objData.fnGetValue("Action", "").ToUpper())
                                    {
                                        case "VERIFYDOL":
                                            blResult = VerifyDOLElement(objData.fnGetValue("LossDate", ""));
                                            break;
                                        case "VERIFYUSPS":
                                            string strCity = clsWE.fnGetAttribute(clsWE.fnGetWe("//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[contains(@data-bind, 'City')]"), "", "value", false);
                                            string strState = clsWE.fnGetAttribute(clsWE.fnGetWe("(//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//span[@class='select2-selection__rendered'])[2]"), "", "title", false);
                                            //Verify Initial Value
                                            if (strCity == "" && strState == "")
                                            {
                                                clsReportResult.fnLog("Create Claim", "The Loss Location City and State are empty", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Create Claim", "The Loss Location City and State has the following initial values. City: " + strCity + " ,State: " + strState + ".", "Fail", true, false);
                                                blResult = false;
                                            }
                                            //Enter ZipCode
                                            string[] arrValues = objData.fnGetValue("ActionValues", "").Split(';');
                                            IWebElement objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[contains(@data-bind, 'ZipCode')]"));
                                            Actions action = new Actions(clsWebBrowser.objDriver);
                                            objWebEdit.Click();
                                            action.KeyDown(Keys.Control).SendKeys(Keys.Home).Perform();
                                            objWebEdit.SendKeys(Keys.Delete);
                                            objWebEdit.SendKeys(arrValues[0]);
                                            Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                            objWebEdit.SendKeys(Keys.Enter);
                                            Thread.Sleep(TimeSpan.FromMilliseconds(500));
                                            //Get City/State values
                                            strCity = clsWE.fnGetAttribute(clsWE.fnGetWe("//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//input[contains(@data-bind, 'City')]"), "", "value", false);
                                            strState = clsWE.fnGetAttribute(clsWE.fnGetWe("(//div[@id='address_CLAIM_LOSS_LOCATION_ADDRESS']//span[@class='select2-selection__rendered'])[2]"), "", "title", false);
                                            //Verify Initial Value
                                            if (strCity != "" && strState != "")
                                            {
                                                clsReportResult.fnLog("Create Claim", "The USPS function retrive the City = " + strCity + " and State = " + strState + " after provide the ZipCode as expected.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Create Claim", "The USPS values was not returned as expected, City expected (" + arrValues[1] + ") but it returns (" + strCity + "), State expected (" + arrValues[2] + ") but returns (" + strState + "). ", "Fail", true, false);
                                                blResult = false;
                                            }
                                            break;
                                        case "VERIFYPREVIEWMODE":
                                            clsReportResult.fnLog("Preview Mode Label", "The Preview Mode Label verification starts on Intake Flow Screen.", "Info", false, false);
                                            if (objData.fnGetValue("ActionValues", "").ToUpper() == "TRUE" || objData.fnGetValue("ActionValues", "").ToUpper() == "YES")
                                            {
                                                blResult = clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                string strMessage = blResult ? "was displayed in the Intake Flow Page as expected." : "should be displayed in the Intake Flow Page but was not found.";
                                                clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                            }
                                            else if (objData.fnGetValue("ActionValues", "").ToUpper() == "FALSE" || objData.fnGetValue("ActionValues", "").ToUpper() == "NO")
                                            {
                                                clsMG.fnGoTopPage();
                                                blResult = !clsMG.IsElementPresent("//span[contains(@data-bind, 'PreviewModeSubmitting')]");
                                                string strMessage = blResult ? "is not displayed as expected in the Intake Flow Page." : "should not be displayed in the Intake Flow Page for this user role.";
                                                clsReportResult.fnLog("Preview Mode Label", $"The Preview Mode Label {strMessage}.", blResult ? "Pass" : "Fail", true, false);
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("Create Claim", "The Duplicated Claim Check was not successfully and claim creation cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Create Claim", "The Create Claim cannot continue since the claim cannot start successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Create Claim", "The Create Claim cannot continue since the client was not selected as expected.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            return blResult;
        }

        private string fnGetActiveElementLabel()
        {
            string strLabelElement = "";
            Thread.Sleep(TimeSpan.FromSeconds(2));
            var activeElement = clsWebBrowser.objDriver.SwitchTo().ActiveElement();
            IWebElement tempElement = null;
            do
            {
                IWebElement getParentElement;
                if (tempElement == null)
                { getParentElement = clsWebBrowser.objDriver.fnGetParentNodeFromJavascript(activeElement); }
                else
                { getParentElement = clsWebBrowser.objDriver.fnGetParentNodeFromJavascript(tempElement); }
                tempElement = getParentElement;
            }
            while (tempElement.GetAttribute("class") != "row");
            //Get Current Label
            if (tempElement != null)
            {
                By byXPath = By.XPath(".//div[1]//span");
                IWebElement getCurrentLabel;
                try
                {
                    getCurrentLabel = tempElement.FindElement(byXPath);
                }
                catch(NoSuchElementException)
                {
                    getCurrentLabel = tempElement.fnGetParentNode().fnGetParentNode().FindElements(byXPath).First(x => !string.IsNullOrEmpty(x.Text));
                }
                strLabelElement = getCurrentLabel.Text;
            }

            return strLabelElement;
        }

        public bool fnReportedByVerification(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Restricted Reported By", "Create Standard Claim.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "EventInfo");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Select Client
                    clsReportResult.fnLog("Restricted Reported By", "The Create Claim Function Starts.", "Info", false);
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake and go to Duplicate Claim Check
                        if (fnStartNewIntake(objData.fnGetValue("LOB", "")))
                        {
                            //Populate Duplicate Claim Check
                            if (fnDuplicateClaimPage(objData, false))
                            {
                                //Verify that Reported By is not displayed on Duplicated Claim Check
                                if (!clsMG.IsElementPresent("//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']"))
                                {
                                    clsReportResult.fnLog("Restricted Reported By", "The Reported By dropdown is not displayed for Intake Only Users on Duplicate Claim Check Page as expected.", "Pass", true, false);
                                }
                                else
                                {
                                    clsReportResult.fnLog("Restricted Reported By", "The Reported By dropdown should not be displayed for Intake Only Users on Duplicate Claim Check Page", "Fail", true, false);
                                    blResult = false;
                                }

                                //Next Button
                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next Button", false);
                                if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']") || !clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                                {
                                    clsReportResult.fnLog("Restricted Reported By", "The Duplicate Claim Check Page was filled successfully.", "Pass", false, false);
                                    //Verify is exist Start Intake Button
                                    if (clsMG.IsElementPresent("//button[@id='start-intake']"))
                                    {
                                        clsMG.fnGoTopPage();
                                        clsWE.fnClick(clsWE.fnGetWe("//button[@id='start-intake']"), "Start Intake Button", false, false);
                                        Thread.Sleep(TimeSpan.FromSeconds(10));
                                    }

                                    //Verify that Reported By is not displayed on Intake Flow
                                    if (!clsMG.IsElementPresent("//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']"))
                                    {
                                        clsReportResult.fnLog("Restricted Reported By", "The Reported By dropdown is not displayed for Intake Only Users on Intake Flow Page as expected.", "Pass", true, false);
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Restricted Reported By", "The Reported By dropdown should not be displayed for Intake Only Users on Intake Flow Page", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    clsReportResult.fnLog("Restricted Reported By", "Some errors were found in Duplicate Claim Check Page and test cannot continue", "Fail", true, false);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("Restricted Reported By", "The Duplicated Claim Check was not successfully and claim creation cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Restricted Reported By", "The Create Claim cannot continue since the claim cannot start successfully.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Restricted Reported By", "The Create Claim cannot continue since the client was not selected as expected.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            return blResult;
        }

        /*
        public bool fnIntakeOnlyResumeVerification(string pstrSetNo)
        {
            bool blResult = true;
            
            clsData objData = new clsData();
            clsReportResult.fnLog("Verify Intake Only Resume Claim", "<<<<<<<<<< Resume Claim Function Starts. >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "ResumeIntOnly");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Search Claim
                    if (fnSearchClaim(objData))
                    {
                        clsReportResult.fnLog("Verify Intake Only Resume Claim", "Search Page Results", "Info", true);
                        //Get Claim List
                        IList<IWebElement> lsPagButton = clsWebBrowser.objDriver.FindElements(By.XPath("//li[contains(@class, 'paginate_button page-item')]"));
                        IList<IWebElement> lsRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//tr//td[8]"));
                        string strCurrentUser = clsWebBrowser.objDriver.FindElement(By.XPath("//li[@class='nav-item dropdown m-menuitem-show']//span[contains(@data-bind, 'DisplayName')]")).GetAttribute("innerText");
                        bool blFound = false;
                        if (lsRows.Count > 0)
                        {
                            //Iterate for each Page
                            for (int intPage = 1; intPage <= lsPagButton.Count - 2; intPage++) 
                            {
                                int intCount = 0;
                                lsRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//tr//td[8]"));
                                switch (objData.fnGetValue("UserVerification", "").ToUpper()) 
                                {
                                    case "SAMEUSER":
                                        //Search all the records with the same user logged in
                                        foreach (var row in lsRows)
                                        {
                                            intCount++;
                                            if (row.GetAttribute("innerText") == strCurrentUser)
                                            {
                                                //Select Rown and Click on submit
                                                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                                Thread.Sleep(TimeSpan.FromSeconds(1));
                                                clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)["+ intCount +"]"), "Click Edit Button", false, false);
                                                Thread.Sleep(TimeSpan.FromSeconds(4));
                                                clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                                blFound = true;
                                                if (!clsMG.IsElementPresent("//button[text()='Resume']"))
                                                {
                                                    clsReportResult.fnLog("Verify Intake Only Resume Claim", "The RESUME button is not displayed for Intake Only Users as expected.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Verify Intake Only Resume Claim", "The RESUME button should not be displayed for Intake Only Users.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                    case "OTHERUSER":
                                        //Search all the records with a different user
                                        clsWE.fnScrollTo(clsWE.fnGetWe("//h4[text()='Search Results']"), "", false, false);
                                        Thread.Sleep(TimeSpan.FromSeconds(3));
                                        foreach (var row in lsRows)
                                        {
                                            intCount++;
                                            if (row.GetAttribute("innerText") != strCurrentUser)
                                            {
                                                //Select
                                                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                                Thread.Sleep(TimeSpan.FromSeconds(1));
                                                clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                                clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                                blFound = true;
                                                if (!clsMG.IsElementPresent("//button[text()='Resume']"))
                                                {
                                                    clsReportResult.fnLog("Verify Intake Only Resume Claim", "The RESUME button is not displayed for Intake Only Users as expected.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Verify Intake Only Resume Claim", "The RESUME button should not be displayed for Intake Only Users.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                                break;
                                            }
                                        }
                                        break;
                                }
                                //Click Next Button
                                if (!clsMG.IsElementPresent("//li[@id='results_next']/a")) 
                                {
                                    clsWebBrowser.objDriver.Navigate().Back();
                                }
                                clsWE.fnPageLoad(clsWE.fnGetWe("//li[@id='results_next']/a"), "Wait Next Button", false, false);
                                if (intPage + 1 > 1) 
                                { 
                                    clsWE.fnClick(clsWE.fnGetWe("//li[@id='results_next']/a"), "Next Button", false, false);
                                    //clsMG.fnGoTopPage();
                                }
                                if (blFound) { break; }
                            }
                            //Verify that Resume button is not displayed
                            if (!blFound) 
                            {
                                clsReportResult.fnLog("Verify Intake Only Resume Claim", "No claims were found with the criteria provided and test cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("Verify Intake Only Resume Claim", "The search criteria did not return any data and test cannot continue.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Verify Intake Only Resume Claim", "The Intake Only Resume Claims cannot continue since some error appears in the search page.", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            
            return blResult;
        }
        */

        public bool fnIntakeOnlyDashboardResumeAPI()
        {
            bool blResult = true;
            bool blFound = false;
            clsData objData = new clsData();
            clsReportResult.fnLog("Resume API Claim", "<<<<<<<<<< Resume API Claim Function Starts. >>>>>>>>>>", "Info", false);

            //Get Claim Created via API
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "API");
            objData.CurrentRow = 2;
            string strClaim = objData.fnGetValue("IncidentNumber", "");

            //Search claim created in Dashboard
            IList<IWebElement> lsPagButton = clsWebBrowser.objDriver.FindElements(By.XPath("//li[contains(@class, 'paginate_button page-item')]"));
            IList<IWebElement> lsRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='resumeCalls']//tr//a"));
            //Verify if Dashboard has records
            if (lsRows.Count > 0)
            {
                //Iterate for each Page
                for (int intPage = 1; intPage <= lsPagButton.Count - 2; intPage++)
                {
                    //Iterator
                    int intElement = 0;
                    lsRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='resumeCalls']//tr//a"));
                    //Iterate for each row
                    for (int intBtn = 1; intBtn <= lsRows.Count; intBtn++)
                    {
                        intElement++;
                        IWebElement elButton = clsWebBrowser.objDriver.FindElement(By.XPath("(//table[@id='resumeCalls']//tr//a)[" + intElement + "]"));
                        elButton.Click();
                        if (!clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                        {
                            if (!clsMG.IsElementPresent("//*[text()='Report a Claim']"))
                            {
                                clsMG.WaitWEUntilAppears("Wait Intake Page", "//div[@id='list-example']", 3);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//h1[@data-bind='text: IntakeName']"), "Intake Page", false, false);
                                //Get Confirmation number
                                IWebElement elConfNumber = clsWebBrowser.objDriver.FindElement(By.XPath("//span[contains(@data-bind, 'text: VendorIncidentNumber')]"));
                                if (elConfNumber.GetAttribute("innerText") == strClaim)
                                {
                                    blFound = true;
                                    break;
                                }
                                clsWebBrowser.objDriver.Navigate().Back();
                                Thread.Sleep(TimeSpan.FromSeconds(3));
                                clsMG.WaitWEUntilAppears("Wait Intake Dashboard", "//a[@id='resumeCalls-tab']", 2);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//section[@id='calls-section']"), "Intake Dashboard Page", false, false);
                            }
                            else
                            {
                                clsWebBrowser.objDriver.Navigate().Back();
                                Thread.Sleep(TimeSpan.FromSeconds(3));
                                clsMG.WaitWEUntilAppears("Wait Intake Dashboard", "//a[@id='resumeCalls-tab']", 2);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//section[@id='calls-section']"), "Intake Dashboard Page", false, false);
                            }

                        }
                        else
                        {
                            while (clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(3));
                            }
                        }
                    }
                    //Click Next Button
                    if (intPage > 1) { clsWE.fnClick(clsWE.fnGetWe("//li[@id='results_next']/a"), "Next Button", false, false); }
                    if (blFound) { break; }
                }

            }

            //Check Status
            if (blFound)
            {
                clsReportResult.fnLog("Resume API Claim", "The claim: " + strClaim + " created via API was found in the dashboard as expected.", "Pass", true);
            }
            else
            {
                clsReportResult.fnLog("Resume API Claim", "The claim created via API was not found in the dashboard.", "Fail", true);
                blResult = false;
            }
            clsLogin login = new clsLogin();
            login.fnLogOffSession();
            return blResult;
        }

        public bool fnImagesBlockedAnnonymousAccess()
        {
            bool blResult = true;
            clsReportResult.fnLog("Verify blocked for anonymous access", "The Verify client Images blocked for anonymous access function starts.", "Info", false);
            clsWebBrowser.objDriver.Navigate().GoToUrl("https://intake-uat.sedgwick.com/u/2807/home1");
            if (!clsMG.IsElementPresent("//h3[contains(text(),'Access Denied')]"))
            {
                clsWE.fnPageLoad(clsWE.fnGetWe("//h1[contains(text(),'Lowes Intake Center')]"), "Lowes header", false, false);
                if (clsWE.fnElementExist("Options in screen", "//div[@class='action-group']", false, false))
                {
                    if (clsMG.IsElementPresent("//button[@id='cookie-accept']")) { clsWE.fnClick(clsWE.fnGetWe("//button[@id='cookie-accept']"), "Accept Cookies Button", false); }
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    blResult = clickOnActionButton("Positive");
                }
                else
                {
                    clsReportResult.fnLog("Verify blocked for anonymous access", "The landing page does not have LOBs link to review.", "Fail", true, false);
                    blResult = false;
                }
            }
            else
            {
                clsReportResult.fnLog("Verify blocked for anonymous access", "The Self Service page is not accessible and display access denied.", "Fail", false);
                blResult = false;
            }

            return blResult;
        }

        public bool fnImagesBlockedAnnonymousAccessLogin()
        {
            bool blResult = true;
            clsReportResult.fnLog("Verify blocked for anonymous access", "The anonymous access with an existing login session function starts.", "Info", false);
            if (clsWE.fnElementExist("Login Label", "//span[contains(text(), 'You are currently logged into')]", true, false))
            {
                ((IJavaScriptExecutor)clsWebBrowser.objDriver).ExecuteScript("window.open('https://intake-uat.sedgwick.com/u/2807/home1');");
                clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles.Last());
                Thread.Sleep(TimeSpan.FromSeconds(3));
                blResult = clickOnActionButton("Negative", "https://intake-uat.sedgwick.com/u/2807/home1");
                clsMG.fnSwitchToWindowAndClose(1);
                clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles[0]);
            }
            else
            {
                clsReportResult.fnLog("Verify blocked for anonymous access", "The login session was not compleated successfully.", "Fail", false);
                blResult = false;
            }

            return blResult;
        }

        private bool clickOnActionButton(string strCase, string pstrUrl = "")
        {
            bool blResult = true;
            Thread.Sleep(TimeSpan.FromSeconds(5));
            IList<IWebElement> lsbutton = clsWebBrowser.objDriver.FindElements(By.XPath("//button[@class='action']"));
            for (int elm = 1; elm <= lsbutton.Count; elm++)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                IWebElement elLOB = clsWebBrowser.objDriver.FindElement(By.XPath("(//button[@class='action'])[" + elm + "]"));
                IWebElement elName = clsWebBrowser.objDriver.FindElement(By.XPath("(//button[@class='action']//span)[" + elm + "]"));
                clsMG.fnHighlight(elLOB);
                clsReportResult.fnLog("Element clickable", "Click on element: " + elName.GetAttribute("innerText").ToString() + ".", "Info", true, false);
                elLOB.Click();

                switch (strCase.ToUpper())
                {
                    case "POSITIVE":
                        clsWE.fnPageLoad(clsWE.fnGetWe("//h3[contains(text(),'Access Denied')]"), "Access Denied", false, false);
                        if (clsMG.IsElementPresent("//h3[contains(text(),'Access Denied')]"))
                        {
                            clsReportResult.fnLog("Access Denied Message", "The Access Denied message is displayed for anonymous access.", "Pass", true, false);
                        }
                        else
                        {
                            clsReportResult.fnLog("Access Denied Message", "The Access Denied message should be displayed for anonymous access.", "Fail", true, false);
                            blResult = false;
                        }
                        clsWebBrowser.objDriver.Navigate().Back();
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        break;
                    case "NEGATIVE":
                        if (clsMG.IsElementPresent("//span[contains(text(), 'You are currently logged into')]"))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                            clsReportResult.fnLog("Access Granted", "The Self Service goes to intake flow when an active session exist.", "Pass", true, false);
                        }
                        else
                        {
                            clsReportResult.fnLog("Access Granted", "The Self Service should go to intake flow when an active session exist but it display access denied.", "Fail", true, false);
                            blResult = false;
                        }
                        clsWebBrowser.objDriver.Navigate().GoToUrl(pstrUrl);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        break;
                }
            }
            return blResult;
        }

        public bool fnUserWithLOBRestriction(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("LOB Restriction By User", "The User with LOB Restriction Function Starts.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "EventInfo");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        while (!clsMG.IsElementPresent("//table[@id='intakes']")) { Thread.Sleep(TimeSpan.FromSeconds(2)); }
                        clsWE.fnScrollTo(clsWE.fnGetWe("//table[@id='intakes']"), "Scrolling to Intakes table", true, false);
                        //Get Rows from table
                        IList<IWebElement> lsWebRows = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='intakes']//tr//td[contains(@data-bind, 'LineOfBusiness')]"));
                        if (lsWebRows.Count > 0)
                        {
                            int intRows = 1;
                            foreach (IWebElement objLOB in lsWebRows)
                            {
                                if (objLOB.GetAttribute("innerText").ToUpper() == objData.fnGetValue("RestrictedLOB", "").ToUpper())
                                {
                                    clsReportResult.fnLog("LOB Restriction by User", "The restricted LOB[" + intRows + "]: " + objLOB.GetAttribute("innerText") + " is equal to the LOB provided: " + objData.fnGetValue("RestrictedLOB", "") + " in data driver.", "Pass", false, false);
                                }
                                else
                                {
                                    clsReportResult.fnLog("LOB Restriction by User", "The restricted LOB[" + intRows + "]: " + objLOB.GetAttribute("innerText") + " is not equal to the LOB provided: " + objData.fnGetValue("RestrictedLOB", "") + " in data driver.", "Fail", true, false);
                                    blResult = false;
                                }
                                intRow++;
                            }
                        }
                        else
                        {
                            clsReportResult.fnLog("LOB Restriction by User", "The table does not have any active script and is empty.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("LOB Restriction by User", "The Client was not selecte as expected and test cannot continue.", "Fail", false, false);
                        blResult = false;
                    }
                }
            }
            return blResult;

        }
        
        public string fnGetConfirmationNumber()
        {
            string strConfNumber = "";
            if (clsMG.IsElementPresent("//span[@data-bind = 'text: VendorIncidentNumber']"))
            { strConfNumber = clsWE.fnGetAttribute(clsWE.fnGetWe("//span[@data-bind = 'text: VendorIncidentNumber']"), "Confirmation Number", "innerText"); }
            else if (clsMG.IsElementPresent("//span[@data-bind = 'text: ConfirmationNumber']"))
            { strConfNumber = clsWE.fnGetAttribute(clsWE.fnGetWe("//span[@data-bind = 'text: ConfirmationNumber']"), "Confirmation Number", "innerText"); }
            return strConfNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedLossData"></param>
        /// <returns></returns>
        private bool VerifyFloatingMenuBar()
        {

            return false;
            //string dolElement;
            //var dolSelector = "//span[@data-bind='date: headerLossDate']";
            //clsMG.WaitWEUntilAppears("Wait for Date of loss to appear", dolSelector, 10);
            //var count = 0;
            //bool equalsExpectedValue;
            //do
            //{
            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //    dolElement = clsWE.fnGetAttribute(clsWE.fnGetWe(dolSelector), "Date of Loss", "innerText", false);
            //    equalsExpectedValue = dolElement.Equals(expectedLossData);
            //    count++;
            //}
            //while ((!equalsExpectedValue) && count < 10);

            //if (equalsExpectedValue)
            //{
            //    clsReportResult.fnLog("Date Of Loss Check", $"The DOL was found as expected. Expected Value <{expectedLossData}> vs Actual Value <{dolElement}>", "Pass", false);
            //}
            //else
            //{
            //    clsReportResult.fnLog("Date Of Loss Check", $"The DOL was found does not match as expected. Expected Value <{expectedLossData}> vs Actual Value <{dolElement}>", "Fail", true);
            //}
            //return equalsExpectedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedLossData"></param>
        /// <returns></returns>
        private bool VerifyDOLElement(string expectedLossData)
        {
            string dolElement;
            var dolSelector = "//span[@data-bind='date: headerLossDate']";
            clsMG.WaitWEUntilAppears("Wait for Date of loss to appear", dolSelector, 10);
            var count = 0;
            bool equalsExpectedValue;
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                dolElement = clsWE.fnGetAttribute(clsWE.fnGetWe(dolSelector), "Date of Loss", "innerText", false);
                equalsExpectedValue = dolElement.Equals(expectedLossData);
                count++;
            }
            while ((!equalsExpectedValue) && count < 10);

            if (equalsExpectedValue) 
            {
                clsReportResult.fnLog("Date Of Loss Check", $"The DOL was found as expected. Expected Value <{expectedLossData}> vs Actual Value <{dolElement}>", "Pass", false);
            }
            else 
            {
                clsReportResult.fnLog("Date Of Loss Check", $"The DOL was found as expected. Expected Value <{expectedLossData}> vs Actual Value <{dolElement}>", equalsExpectedValue ? "Pass" : "Fail", false);
            }
            return equalsExpectedValue;
        }

        private bool VerifyCodesDropDown(string pstrLOB) 
        {
            bool blResult = false;
            switch (pstrLOB.ToUpper()) 
            {
                case "WORKERSCOMPENSATION":
                    //Go to Injury Information
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='Injury Information']]"), "Injury Information", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Cause Code 1", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Cause Code 2", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Nature Code 1", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Nature Code 2", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Body Part 1", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Body Part 2", "(//div[contains(@question-key, 'CLAIM_EMPLOYEE_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    break;
                case "GENERALLIABILITY":
                    //Go to Injury Information
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='Property/Injured Parties Involved']]"), "Property/Injured Parties Involved", true, false);
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_PROPERTY_INJURED.CLAIM_PROPERTY')]//button"), "Add Property Summary", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Cause Code", "//div[contains(@question-key, 'CLAIM_PROPERTY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single'])", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Nature Code", "//div[contains(@question-key, 'CLAIM_PROPERTY_NATURE_CODE')]//span[@class='select2-selection select2-selection--single'])", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Body Part", "//div[contains(@question-key, 'CLAIM_PROPERTY_TARGET_CODE')]//span[@class='select2-selection select2-selection--single'])", false, false)) { blResult = false; }

                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_PROPERTY_INJURED.CLAIM_INJURED')]//button"), "Add Injured Party Summary", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Cause Code", "//div[contains(@question-key, 'CLAIM_INJURED_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single'])", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Nature Code", "//div[contains(@question-key, 'CLAIM_INJURED_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single'])", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Body Part 1", "(//div[contains(@question-key, 'CLAIM_INJURED_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Body Part 2", "(//div[contains(@question-key, 'CLAIM_INJURED_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }
                    break;
                case "AUTOLIABILITY":
                    //Go to First Party Vehicle
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='First Party Vehicle']]"), "First Party Vehicle", true, false);
                    clsMG.fnSelectDropDownWElm("Was Vehicle Damaged?", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DAMAGE_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle - Cause Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle - Nature Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_NATURE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle - Target Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_TARGET_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    
                    //Go to First Party Vehicle Driver
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='First Party Vehicle Driver']]"), "First Party Vehicle Driver", true, false);
                    clsMG.fnSelectDropDownWElm("Was The Driver Injured?", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver - Cause Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver - Nature Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver- Body Part 1", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    clsMG.fnSelectDropDownWElm("First Party Vehicle Driver- Body Part 1", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", "Head", false, false, "", true);
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver- Body Part 2", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    //Go to First Party Vehicle Passenger
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='First Party Vehicle Driver']]"), "First Party Vehicle Passenger", true, false);
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER')]//button[text()='Add']"), "Add First Party Vehicle Passenger", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    clsMG.fnSelectDropDownWElm("First Party - Was Passenger Injured?", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Passenger - Cause Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Passenger - Nature Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Passenger - Body Part 1", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    clsMG.fnSelectDropDownWElm("First Party Vehicle Passenger - Body Part 1", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", "Head", false, false, "", true);
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Passenger - Body Part 2", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    //Go to Third Party Vehicle
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='First Party Vehicle Driver']]"), "Third Party Vehicle", true, false);
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_VEHICLE')]//button[text()='Add']"), "Add Third Party Vehicle", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    clsMG.fnSelectDropDownWElm("Third Party - Was Vehicle Damaged?", "//div[contains(@question-key, 'CLAIM_VEHICLE_DAMAGE_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle - Cause Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle - Nature Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_NATURE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle - Target Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_TARGET_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }

                    //Go to Third Party Driver
                    clsMG.fnSelectDropDownWElm("Third Party Vehicle Driver - Was The Driver Injured?", "//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver - Cause Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver - Nature Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver- Body Part 1", "(//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    clsMG.fnSelectDropDownWElm("Was The Driver Injured?", "(//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", "Head", false, false, "", true);
                    if (!clsMG.fnDropDownGetElements("First Party Vehicle Driver- Body Part 2", "(//div[contains(@question-key, 'CLAIM_VEHICLE_DRIVER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    //Go to Third Party Vehicle Passenger
                    clsWE.fnClick(clsWE.fnGetWe("//span[text()='Third Party Vehicle Passenger']"), "Third Party Vehicle Passenger", true, false);
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER')]//button[text()='Add']"), "Add Third Party Vehicle Passenger", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    clsMG.fnSelectDropDownWElm("Third Party - Was The Passenger Injured?", "(//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_FLG')]//span[@class='select2-selection select2-selection--single'])[2]", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle Passenger - Cause Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle Passenger - Nature Code", "//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle Passenger - Body Part 1", "(//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    clsMG.fnSelectDropDownWElm("Was The Driver Injured?", "(//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", "Head", false, false, "", true);
                    if (!clsMG.fnDropDownGetElements("Third Party Vehicle Passenger - Body Part 2", "(//div[contains(@question-key, 'CLAIM_VEHICLE_PASSENGER_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    // Go to Other Party/Property Damage
                    clsWE.fnClick(clsWE.fnGetWe("//a[span[text()='Other Party/Property Damage']]"), "Other Party/Property Damage", true, false);
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY')]//button"), "Other Parties", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    clsMG.fnSelectDropDownWElm("Was The Other Party Injured?", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Other Party - Cause Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Other Party - Nature Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_INJURY_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Other Party - Body Part 1", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", false, false)) { blResult = false; }
                    clsMG.fnSelectDropDownWElm("Was The Driver Injured?", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[1]", "Head", false, false, "", true);
                    if (!clsMG.fnDropDownGetElements("Other Party - Body Part 2", "(//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_THIRDPARTY_INJURY_BODY_PART')]//span[@class='select2-selection select2-selection--single'])[2]", false, false)) { blResult = false; }

                    // Go to Other Properties
                    clsWE.fnClick(clsWE.fnGetWe("//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PROPERTY')]//button"), "Other Properties", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    clsMG.fnSelectDropDownWElm("Was The Property Damaged?", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PROPERTY_DAMAGE_FLG')]//span[@class='select2-selection select2-selection--single']", "Yes", false, false, "", true);
                    Thread.Sleep(TimeSpan.FromSeconds(4));
                    if (!clsMG.fnDropDownGetElements("Other Property - Cause Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PROPERTY_CAUSE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Other Property - Nature Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PROPERTY_NATURE_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }
                    if (!clsMG.fnDropDownGetElements("Other Property - Target Code", "//div[contains(@question-key, 'CLAIM_INSURED_VEHICLE_PROPERTY_TARGET_CODE')]//span[@class='select2-selection select2-selection--single']", false, false)) { blResult = false; }


                    break;
                

        }
            
            return blResult;
        }

    }
}
