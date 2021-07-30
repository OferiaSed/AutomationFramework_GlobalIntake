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
                    clsWE.fnClick(clsWE.fnGetWe("//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]"), "Select Cliet Popup", false);
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
            //Verify is LOB script is displayed
            if (clsWE.fnElementExist("Select LOB", "//table[@id='intakes']//tr[td[@data-bind='text: Name' and contains(text(), '"+ pstrLOB + "')]]//button", true))
            {
                clsWE.fnClick(clsWE.fnGetWe("//table[@id='intakes']//tr[td[@data-bind='text: Name' and contains(text(), '" + pstrLOB + "')]]//button"), "Select LOB", false);
            }
            else 
            {
                clsReportResult.fnLog("Start New Intake", "The intake cannot start since eas not found.", "Fail", true, true);
                blResult = false;
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




    }
}
