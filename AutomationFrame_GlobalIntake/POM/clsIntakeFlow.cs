using AutomationFramework;
using NUnit.Framework;
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
                    switch (objData.fnGetValue("Action", "").ToUpper()) 
                    {
                        case "LOCATIONLOOKUP":
                            //Go to New Intake
                            if (fnSelectIntake(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")))
                            {
                                if (fnStartNewIntake(objData.fnGetValue("IntakeName", "")))
                                {
                                    //Start Intake
                                    clsReportResult.fnLog("Account Unit Security", "--->> The Location Lookup Account/Unit Verification Start.", "Info", false);
                                    clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'Duplicate Claim')]"), "Duplicate Claim Page", false, false);
                                    if (clsWE.fnElementExist("Duplicate Claim Page", "//span[contains(text(), 'Duplicate Claim')]", true))
                                    {
                                        //Verify Location Lookup was opened
                                        clsWE.fnClick(clsWE.fnGetWe("//button[@id='btnJurisLocation_LOCATION_LOOKUP']"), "Location Lookup", false);
                                        Thread.Sleep(TimeSpan.FromSeconds(3));
                                        if (clsWE.fnElementExist("Location Lookup Page", "//div[@id='jurisLocationSearchModal_LOCATION_LOOKUP' and contains(@style, 'display: block;')]", true))
                                        {
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//table[@id='jurisLocationResults_LOCATION_LOOKUP']"), "Location Lookup Values", false, false);
                                            //Verify Account/Unit Restriction
                                            switch (objData.fnGetValue("Restriction", "").ToUpper()) 
                                            {
                                                case "ACCOUNT":
                                                    if (clsWE.fnElementExist("Location Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[9][text()='" + objData.fnGetValue("AccUnRestricted", "") + "']", true))
                                                    {
                                                        //Update sorting with account column
                                                        clsReportResult.fnLog("Location Lookup Table", "The restricted account " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was found in the table.", "info", true, false);
                                                        clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Account Number')])[1]"), "Account Column", false);
                                                        Thread.Sleep(TimeSpan.FromSeconds(3));
                                                        if (clsWE.fnElementExist("Location Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[9][text()='" + objData.fnGetValue("AccUnRestricted", "") + "']", true))
                                                        {
                                                            clsReportResult.fnLog("Location Lookup Table", "The restricted account " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was found in the table after apply the sorting.", "info", true, false);
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Location Lookup Table", "The restricted account " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was not found in the table after apply the sorting but should be displayed.", "Fail", true, false);
                                                            blResult = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        clsReportResult.fnLog("Location Lookup Table", "The restricted account " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was not found in the table but should be displayed.", "Fail", true, false);
                                                        blResult = false;
                                                    }
                                                    fnCloseLocationLookupPopup();
                                                    break;
                                                case "UNIT":
                                                    if (clsWE.fnElementExist("Location Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[3][text()='" + objData.fnGetValue("AccUnRestricted", "") + "']", true))
                                                    {
                                                        //Update sorting with account column
                                                        clsReportResult.fnLog("Location Lookup Table", "The restricted unit " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was found in the table.", "info", true, false);
                                                        clsWE.fnClick(clsWE.fnGetWe("(//th[contains(@aria-label, 'Unit Number')])[1]"), "Unit Column", false);
                                                        Thread.Sleep(TimeSpan.FromSeconds(3));
                                                        if (clsWE.fnElementExist("Location Lookup Table", "//table[@id='jurisLocationResults_LOCATION_LOOKUP']//tr[1]//td[3][text()='" + objData.fnGetValue("AccUnRestricted", "") + "']", true))
                                                        {
                                                            clsReportResult.fnLog("Location Lookup Table", "The restricted unit " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was found in the table after apply the sorting.", "info", true, false);
                                                            clsWE.fnClick(clsWE.fnGetWe("//button[@id='btn_close_juris']"), "Close Location Lookup", false);
                                                        }
                                                        else
                                                        {
                                                            clsReportResult.fnLog("Location Lookup Table", "The restricted unit " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was not found in the table after apply the sorting but should be displayed.", "Fail", true, false);
                                                            blResult = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        clsReportResult.fnLog("Location Lookup Table", "The restricted unit " + objData.fnGetValue("AccUnRestricted", "").ToString() + " was not found in the table but should be displayed.", "Fail", true, false);
                                                        blResult = false;
                                                    }
                                                    fnCloseLocationLookupPopup();
                                                    break;
                                                case "ACCOUNTUNIT":
                                                    break;
                                            }
                                        }
                                        else 
                                        {
                                            clsReportResult.fnLog("Location Lookup Page", "The Location Lookup Page was not loaded correctly.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    else 
                                    {
                                        clsReportResult.fnLog("Duplicate Claim Page", "The Duplicate Claim Page was not loaded correctly.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                else
                                { blResult = false; }
                            }
                            else 
                            { blResult = false; }

                            break;
                        case "SEARCH":
                            clsReportResult.fnLog("Account Unit Security", "--->> The Search Claim Account/Unit Verification Start.", "Info", false);
                            if (fnSearchClaim(objData))
                            {
                                //verify Results
                                if (objData.fnGetValue("BeDisplayed", "").ToUpper() == "YES")
                                {
                                    if (clsWE.fnElementExist("Search Intake Value", "//tr[td[text() = '"+ objData.fnGetValue("ClaimNo", "") + "']]", false))
                                    { clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " is displayed as expected.", "Pass", true); }
                                    else
                                    {
                                        clsReportResult.fnLog("Account Unit Security", "The claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " should be displayed for this user.", "Fail", true);
                                        blResult = false;
                                    }
                                }
                                else
                                {
                                    if (clsWE.fnElementExist("Search Intake Value", "//td[contains(text(), 'No data available in table')]", false))
                                    { clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " is not displayed as expected.", "Pass", true); }
                                    else
                                    {
                                        clsReportResult.fnLog("Account Unit Security", "The restricted claim: " + objData.fnGetValue("ClaimNo", "").ToString() + " should not be displayed for this user.", "Fail", true);
                                        blResult = false;
                                    }
                                }
                            }
                            else 
                            {
                                clsReportResult.fnLog("Account Unit Security", "The Search Intake was not successfully.", "Fail", false);
                                blResult = false;
                            }

                            break;
                        default:
                            clsReportResult.fnLog("Account Unit Security", "--->> The action: " + objData.fnGetValue("Action", "").ToString() + " was not found.", "Fail", false);
                            blResult = false;
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


        

    }
}
