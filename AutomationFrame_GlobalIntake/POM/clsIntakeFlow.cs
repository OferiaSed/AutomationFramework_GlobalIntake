using AutomationFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.POM
{
    class clsIntakeFlow
    {
        private clsMegaIntake clsMG = new clsMegaIntake();
        private clsWebElements clsWE = new clsWebElements();


        public bool fnSelectIntake(string pstrClientNumber, string pstrClientName)
        {
            bool blResult = true;
            //Go to New Intake and select Intake
            clsMG.fnHamburgerMenu("New Intake");
            //clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Select Intake']"), "Select Intake", false, false);
            if (clsWE.fnElementExist("Select Intake", "//h4[text()='Select Intake']", true))
            {
                //Verify if the client is already selected
                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//button[@id='selectClient_']/span"), "Select Client", "innerText", false) == "SELECT CLIENT")
                {
                    clsWE.fnClick(clsWE.fnGetWe("//button[@id='selectClient_']"), "Open Client Popup", false);
                    if (clsWE.fnElementExist("Select Intake Popup", "//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]", false))
                    {
                        //Apply the filter
                        clsMG.fnCleanAndEnterText("Client Number or Name", "//input[@placeholder='Client Number or Name']", pstrClientNumber, false, false, "", false);
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        //Check if the client exist
                        if (clsMG.IsElementPresent("//tr[td[contains(text(), '"+ pstrClientNumber + "')] and td[contains(text(), '"+ pstrClientName + "')]]"))
                        {
                            clsWE.fnClick(clsWE.fnGetWe("//tr[td[contains(text(), '" + pstrClientNumber + "')] and td[contains(text(), '" + pstrClientName + "')]]//a"), "Select Button", true);
                        }
                        else 
                        {
                            clsReportResult.fnLog("Select Intake", "The client: "+ pstrClientNumber + " was not found in the popup", "Fail", true, true);
                            blResult = false;
                        }
                    }
                    else
                    {
                        clsReportResult.fnLog("Select Intake", "The select intake popup was not displayed", "Fail", true, true);
                        blResult = false;
                    }
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

        public bool fnDuplicateClaimPage(clsData pobjData) 
        {
            bool blResult = true;
            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Intake", false, false);
            if (clsWE.fnElementExist("Duplicate Claim Check", "//span[contains(text(), 'Duplicate Claim')]", true))
            {
                clsMG.fnCleanAndEnterText("Loss Time", "//div[@class='row' and div[span[text()='Loss Time']]]//input[@class='form-control']", pobjData.fnGetValue("LossTime", ""), false, false, "", true);
                clsMG.fnCleanAndEnterText("Loss Date", "//div[@class='row' and div[span[text()='Loss Date']]]//input[@class='form-control']", pobjData.fnGetValue("LossDate", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reporter Type", "//div[@class='row' and div[span[text()='Reporter Type']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReporterType", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Reported By", "//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']", pobjData.fnGetValue("ReportedBy", ""), false, false, "", false);
                //Fill EE Lookup
                if (pobjData.fnGetValue("FillEELookup", "").ToUpper() == "TRUE") { blResult = fnEmployeeLocationLookup(pobjData.fnGetValue("EELookupSet", "")); }
                //FIll Location Lookup
                if (pobjData.fnGetValue("FillLocLookup", "").ToUpper() == "TRUE") { blResult = fnLocationLookup(pobjData.fnGetValue("LocLookupSet", "")); }
                //Verify if error messages exist
                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Next']"), "Next Button", false);
                if (!clsMG.IsElementPresent("//*[@data-bind='text:ValidationMessage']") || !clsMG.IsElementPresent("//div[@class='md-toast md-toast-error']"))
                {
                    clsReportResult.fnLog("Duplicate Claim Check", "The Duplicate Claim Check Page was filled successfully.", "Fail", false, false);
                }
                else 
                {
                    clsReportResult.fnLog("Duplicate Claim Check", "Some errors were found in Duplicate Claim Check Page and test cannot continue", "Fail", true, false);
                    blResult = false;
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
                    if (clsMG.IsElementPresent("//button[span[text()='Employee Lookup']]"))
                    {
                        //Verify EE Lookup Popup is displayed
                        clsWE.fnClick(clsWE.fnGetWe("//button[span[text()='Employee Lookup']]"), "Emplpyee Lookup Button", false);
                        if (clsMG.IsElementPresent("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]"))
                        {
                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisEmployeeSearchModal_EMPLOYEE_LOOKUP' and contains(@style, 'display: block')]"), "Employee Lookup Popup", false, false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisEmployeeResults_EMPLOYEE_LOOKUP_info']"), "Employee Table", false, false);
                            //Select Data Set
                            if (objData.fnGetValue("Set", "") != "0")
                            {
                                clsMG.fnCleanAndEnterText("Employee ID", "//input[@id='search-empId']", objData.fnGetValue("EmpID", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("SSN", "//input[@id='search-ssn']", objData.fnGetValue("SSN", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("FirstName", "//input[@id='search-firstName']", objData.fnGetValue("FirstName", ""), false, false, "", false);
                                clsMG.fnCleanAndEnterText("LastName", "//input[@id='search-lastName']", objData.fnGetValue("LastName", ""), false, false, "", false);
                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search Button", false);
                                //Select Employee
                                if (clsMG.IsElementPresent("//table[@id='jurisEmployeeResults_EMPLOYEE_LOOKUP']//tr[td[text()='"+ objData.fnGetValue("EmpID", "") + "']]//button"))
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
                    if (clsMG.IsElementPresent("//button[span[text()='Location Lookup']]"))
                    {
                        //Verify Location Lookup Popup is displayed
                        clsWE.fnClick(clsWE.fnGetWe("//button[span[text()='Location Lookup']]"), "Location Lookup Button", false);
                        if (clsMG.IsElementPresent("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"))
                        {
                            //Wait to Load Location Lookup
                            clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block')]"), "Location Lookup Popup", false, false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//table[@aria-describedby='jurisLocationResults_LOCATION_LOOKUP_info']"), "Location Table", false, false);
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
                                    clsMG.fnCleanAndEnterText("City", "//input[@id='search-zipcode']", objData.fnGetValue("ZipCode", ""), false, false, "", false);
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
            return blResult;
        }


        public bool fnSearchClaim(clsData pobjData) 
        {
            bool blResult = true;
            //Go to Search Intake and verify that page is loaded
            clsReportResult.fnLog("Search Intake", "The Search Claims Start.", "Info", false, false);
            clsMG.fnHamburgerMenu("Search Intakes");
            if (clsWE.fnElementExist("Search Intake Page", "//h4[text()='Search Intakes']", true))
            {
                //Populate Search Criteria
                clsMG.fnCleanAndEnterText("Confirmation Number", "//input[contains(@data-bind, 'ConfirmationNumber')]", pobjData.fnGetValue("ConfNo", ""), false, false, "", false);
                clsMG.fnCleanAndEnterText("Claim Number", "//input[contains(@data-bind, 'VendorIncidentNumber')]", pobjData.fnGetValue("ClaimNo", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Line of Business", "//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input", pobjData.fnGetValue("LOB", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Status", "//div[select[contains(@data-bind, 'SearchParameters.Status')]]//input", pobjData.fnGetValue("Status", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Search')]"), "Search", false);
            }
            else
            {
                clsReportResult.fnLog("Search Intake", "The Search Page was not loaded successfully.", "Fail", true, false);
                blResult = false;
            }

            return blResult;
        }

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
                                        if (clsWE.fnElementExist("Search Intake Value", "//tr[td[text() = '" + objData.fnGetValue("ClaimNo", "") + "']]", false))
                                        { clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " is displayed as expected.", "Pass", true); }
                                        else
                                        {
                                            clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " should be displayed for this user.", "Fail", true);
                                            blResult = false;
                                        }
                                        break;
                                    case "NEGATIVE":
                                        if (clsWE.fnElementExist("Search Intake Value", "//td[contains(text(), 'No data available in table')]", false))
                                        { clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " is not displayed as expected.", "Pass", true); }
                                        else
                                        {
                                            clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " should not be displayed for this user.", "Fail", true);
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
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "Driver");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake
                        fnStartNewIntake(objData.fnGetValue("IntakeName", ""));
                        clsReportResult.fnLog("Account Unit Security", "--->> The Location Lookup Account/Unit Verification Start.", "Info", false);
                        clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Duplicate Claim Page", false, false);
                        if (clsWE.fnElementExist("Duplicate Claim Check Page", "//span[contains(text(), 'Duplicate Claim')]", true))
                        {

                            //Verify Policy Case
                            switch (objData.fnGetValue("PolicyType", "").ToUpper())
                            {
                                case "DATABASE":

                                    break;
                                case "SPSS":
                                    break;
                            }

                            /*
                            //Verify Location Lookup Popup was opened successfully
                            clsWE.fnClick(clsWE.fnGetWe("//button[@id='btnJurisLocation_LOCATION_LOOKUP']"), "Location Lookup Button", false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            if (clsWE.fnElementExist("Location Lookup Popup", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]", true))
                            {
                                //Verify the account or unit in the table
                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]"), "Location Lookup Page", false, false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']"), "Location Lookup Values", false, false);
                                //Verify Policy Case
                               
                            }
                            else 
                            {
                                clsReportResult.fnLog("Location Lookup Popup", "The Location Lookup Popup was not loaded correctly and test cannot continue.", "Fail", true, false);
                                blResult = false;
                            }
                            */


                        }
                        else 
                        {
                            clsReportResult.fnLog("Duplicate Claim Check Page", "The Duplicate Claim Page was not loaded correctly and test cannot continue.", "Fail", true, false);
                            blResult = false;
                        }
                    }
                    else 
                    { blResult = false; }
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


        public bool fnTrainingMode(string pstrSetNo) 
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Two Factor Authentication", "Two Factor Authentication Function Starts.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "LogInData");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {

                }
            }
            return blResult;
        }


        public bool fnCreateAndSubmitClaim(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Create Standard Claim", "Create Standard Claim.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "ClaimInfo");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Select Client
                    if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                    {
                        //Start Intake and go to Duplicate Claim Check
                        if (fnStartNewIntake(objData.fnGetValue("LOB", "")))
                        {
                            //Populate Duplicate Claim Check
                            if (fnDuplicateClaimPage(objData))
                            {
                                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='page-container']"), "Intake FLow Page", false, false);
                                clsMG.WaitWEUntilAppears("Wait Intake Flow", "//div[@id='list-example']", 10);

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
                                clsMG.fnCleanAndEnterText("Employee First Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='First Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeFN", ""), false, false, "", false);
                                //Employee Last Name
                                clsMG.fnCleanAndEnterText("Employee Last Name", "//div[contains(@question-key, 'EMPLOYEE_INFORMATION')]//div[@class='row' and div[span[text()='Last Name']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployeeLN", ""), false, false, "", false);
                                //Do You Expect The Team Member To Lose Time From Work?
                                clsMG.fnSelectDropDownWElm("Do You Expect The Team Member To Lose Time From Work?", "//div[@class='row' and div[span[contains(text(), 'Do You Expect The Team Member To Lose Time From Work?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("TeamMemberLossTime", ""), false, false);
                                //Employer Notified Date
                                clsMG.fnCleanAndEnterText("Employer Notified Date", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Employer Notified Date']]]//following-sibling::input[starts-with(@class, 'form-control')]", objData.fnGetValue("EmployerNotifiedDate", ""), false, false, "", false);
                                //Loss Description 
                                clsMG.fnCleanAndEnterText("Loss Description", "//div[contains(@question-key, 'INCIDENT_INFORMATION')]//div[@class='row' and div[span[text()='Loss Description']]]//textarea", objData.fnGetValue("LossDescription", ""), false, false, "", false);
                                //Is Contact Same As Caller?
                                clsMG.fnSelectDropDownWElm("Is This The Loss Location", "//div[contains(@question-key, 'CONTACT_INFORMATION')]//div[@class='row' and div[span[contains(text(), 'Is Contact Same As Caller?')]]]//span[@class='select2-selection select2-selection--single']", objData.fnGetValue("IsSameAsCaller", ""), false, false);


                            }
                            else 
                            { }

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


    }
}
