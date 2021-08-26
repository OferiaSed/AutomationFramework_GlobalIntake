using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
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
    public class clsSearch
    {
        private clsWebElements clsWE = new clsWebElements();
        private clsMegaIntake clsMG = new clsMegaIntake();
        private clsIntakeFlow clsIF = new clsIntakeFlow();

        public bool fnSearchResults(string pstrAction, string pstrClientName, string pstrClientNo, string pstrConfNo, string pstrClaimNumber, string pstrLOB, string pstrStatus)
        {
            bool blResult = true;
            clsReportResult.fnLog("Search Results", "<<<<<<<<<< The Search Results Function starts. >>>>>>>>>>", "Info", false);

            //Go to Search Menu
            clsMG.fnHamburgerMenu("Search Intakes");
            if (clsWE.fnElementExist("Search Intake Page", "//h4[text()='Search Intakes']", true))
            {
                //Clear filters
                clsWE.fnClick(clsWE.fnGetWe("//button[@id='primaryClear']"), "Clear Button", false);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                clsMG.fnGoTopPage();

                //Select Intake Client in Popup
                if (pstrClientName != "" && pstrClientNo != "")
                { blResult = clsIF.fnSelectClientPopup(pstrClientNo, pstrClientName); }

                //Populate Standrad Search Data
                clsMG.fnCleanAndEnterText("Confirmation Number", "//input[contains(@data-bind, 'ConfirmationNumber')]", pstrConfNo, false, false, "", false);
                clsMG.fnCleanAndEnterText("Claim Number", "//input[contains(@data-bind, 'VendorIncidentNumber')]", pstrClaimNumber, false, false, "", false);
                clsMG.fnSelectDropDownWElm("Line of Business", "//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input", pstrLOB, false, false, "", false);
                clsMG.fnSelectDropDownWElm("Status", "//div[select[contains(@data-bind, 'SearchParameters.Status')]]//input", pstrStatus, false, false, "", false);
                
                //Click on Search
                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Search')]"), "Search", false);
                Thread.Sleep(TimeSpan.FromSeconds(5));

                //Select Actions
                blResult = clsPagination(pstrAction, null);



            }
            else
            {
                clsReportResult.fnLog("Search Results", "The Search Page was not loaded and test cannot continue.", "Info", false);
                blResult = false;
            }


            return blResult;
        }

        public bool fnSearchResults(string pstrSetNo) 
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Search Results", "<<<<<<<<<< The Search Results Function starts. >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "SearchIntake");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to Search Menu
                    clsMG.fnHamburgerMenu("Search Intakes");
                    if (clsWE.fnElementExist("Search Intake Page", "//h4[text()='Search Intakes']", true))
                    {
                        //Clear filters
                        clsWE.fnClick(clsWE.fnGetWe("//button[@id='primaryClear']"), "Clear Button", false);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        clsMG.fnGoTopPage();

                        //Select Intake Client in Popup
                        if (objData.fnGetValue("ClientNo", "") != "" && objData.fnGetValue("ClientName", "") != "")
                        { blResult = clsIF.fnSelectClientPopup(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", "")); }

                        //Populate Standrad Search Data
                        clsMG.fnCleanAndEnterText("Confirmation Number", "//input[contains(@data-bind, 'ConfirmationNumber')]", objData.fnGetValue("ConfNo", ""), false, false, "", false);
                        clsMG.fnCleanAndEnterText("Claim Number", "//input[contains(@data-bind, 'VendorIncidentNumber')]", objData.fnGetValue("ClaimNumber", ""), false, false, "", false);
                        clsMG.fnSelectDropDownWElm("Line of Business", "//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input", objData.fnGetValue("LOB", ""), false, false, "", false);
                        clsMG.fnSelectDropDownWElm("Status", "//div[select[contains(@data-bind, 'SearchParameters.Status')]]//input", objData.fnGetValue("Status", ""), false, false, "", false);

                        //Verify Advance Search
                        if (objData.fnGetValue("AdvanceSearch", "").ToUpper() == "TRUE" || objData.fnGetValue("AdvanceSearch", "").ToUpper() == "YES") 
                        {
                            clsWE.fnClick(clsWE.fnGetWe("//*[input[@id='checkAdvanced']]//span[@class='lever']"), "Advance Search Toogle", false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//*[@id='advancedLayout' and not(contains(@style, 'display: none'))]"), "Advance Search Page", true, false);
                            //Advance Search Fields
                            clsMG.fnSelectDropDownWElm("Client Data Location", "//div[select[contains(@id, 'dataLocations')]]//input", objData.fnGetValue("ClientDataLocation", ""), false, false, "", false);
                            clsMG.fnCleanAndEnterText("User", "//input[contains(@data-bind, 'SearchParameters.User')]", objData.fnGetValue("User", ""), false, false, "", false);
                            //Start Date
                            clsMG.fnEnterDatePicker("Start Date", "//input[@id='date-picker-Start-Date']", objData.fnGetValue("StartDate", ""), false, false);
                            //End Date
                            clsMG.fnEnterDatePicker("End Date", "//input[@id='date-picker-End-Date']", objData.fnGetValue("EndDate", ""), false, false);
                            //Claimant Data
                            clsMG.fnCleanAndEnterText("Claimant First Name", "//input[contains(@data-bind, 'SearchParameters.ClaimantFirstName')]", objData.fnGetValue("ClaimantFirstName", ""), false, false, "", false);
                            clsMG.fnCleanAndEnterText("Claimant Last Name", "//input[contains(@data-bind, 'SearchParameters.ClaimantLastName')]", objData.fnGetValue("ClaimantLastName", ""), false, false, "", false);
                            clsMG.fnCleanAndEnterText("Claimant Emp ID", "//input[contains(@data-bind, 'SearchParameters.ClaimantEmpId')]", objData.fnGetValue("ClaimantEmpId", ""), false, false, "", false);
                            //Scroll Page
                            clsWE.fnScrollTo(clsWE.fnGetWe("//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input"), "Scroll to Advance Options", true, false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            clsMG.fnSelectDropDownWElm("Run Mode", "//div[select[contains(@data-bind, 'SearchParameters.RunMode')]]//input", objData.fnGetValue("RunMode", ""), false, false, "", false);
                            if (objData.fnGetValue("EscalatedStatus", "").ToUpper() == "TRUE" || objData.fnGetValue("EscalatedStatus", "").ToUpper() == "YES")
                            { clsWE.fnClick(clsWE.fnGetWe("//*[input[contains(@data-bind, 'SearchParameters.Escalated')]]//span[@class='lever']"), "Escalated Status Toogle", false); }
                            clsMG.fnSelectDropDownWElm("Escalation Status", "//div[select[contains(@id, 'escalationStatus')]]//input", objData.fnGetValue("EscalationStatus", ""), false, false, "", false);
                        }

                        //Click on Search
                        clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Search')]"), "Search", false);
                        Thread.Sleep(TimeSpan.FromSeconds(5));

                        //Select Actions
                        blResult = clsPagination(objData.fnGetValue("Action", ""), objData);



                    }
                    else
                    {
                        clsReportResult.fnLog("Search Results", "The Search Page was not loaded and test cannot continue.", "Info", false);
                        blResult = false;
                    }
                }
            }
            return blResult;
        }

        public bool clsPagination(string pstrSearchType, clsData pobjData) 
        {
            bool blResult = true;
            //Get Web Elements
            IList<IWebElement> lsPagButton = clsWebBrowser.objDriver.FindElements(By.XPath("//li[contains(@class, 'paginate_button page-item')]"));
            IList<IWebElement> lsRecords = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//tbody//tr[@role='row']"));
            string strCurrentUser = clsWebBrowser.objDriver.FindElement(By.XPath("//li[@class='nav-item dropdown m-menuitem-show']//span[contains(@data-bind, 'DisplayName')]")).GetAttribute("innerText");
            bool blFound = false;
            //Verify and interate by Page/Row
            if (lsRecords.Count > 0)
            {
                //Iterate for each Page
                for (int intPage = 1; intPage <= lsPagButton.Count - 2; intPage++) 
                {
                    //Move focus to Table Grid
                    clsWE.fnScrollTo(clsWE.fnGetWe("//h4[text()='Search Results']"), "Search Results Label", true, false);
                    //Iterate by Row Numbers
                    int intCount = 0;
                    lsRecords = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//tr//td[8]"));
                    foreach (var row in lsRecords)
                    {
                        intCount++;
                        switch (pstrSearchType.ToUpper()) 
                        {
                            case "SEARCHVALIDCLAIM":
                                blFound = true;
                                if (clsWE.fnElementExist("Search Results", "//tr[td[text() = '" + pobjData.fnGetValue("ClaimNumber", "") + "']]", false))
                                { clsReportResult.fnLog("Search Results", "The claim: " + pobjData.fnGetValue("ClaimNumber", "").ToString() + " was found as expected.", "Pass", true); }
                                else
                                {
                                    clsReportResult.fnLog("Search Results", "The claim: " + pobjData.fnGetValue("ClaimNumber", "").ToString() + " was not found as expected but should be displayed.", "Fail", true);
                                    blResult = false;
                                }
                                break;
                            case "SINGLECLIENTSEARCH":
                                clsReportResult.fnLog("Search Results", ">>>>> The Search with Single Client Criteria Starts ", "Info", false, false);
                                //clsWE.fnScrollTo(clsWE.fnGetWe("//button[@id='selectClient_']"), "Select Client Button", false, false);
                                //Thread.Sleep(TimeSpan.FromSeconds(3));
                                blFound = true;
                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("//button[@id='selectClient_']"), "Select Client Button", "innerText", false, false) == pobjData.fnGetValue("SingleClient", ""))
                                {
                                    clsReportResult.fnLog("Search Results", "The Search with Single Client was done successfully", "Pass", true, false);
                                }
                                else 
                                {
                                    clsReportResult.fnLog("Search Results", "The Search with Single Client was not completed successfully, the client is not set by default on serach", "Fail", true, false);
                                    blResult = false;
                                }
                                break;
                            case "CLIENTNAMESEARCH":
                                clsReportResult.fnLog("Search Results", ">>>>> The Search with Client Name Criteria Starts ", "Info", false, false);
                                clsMG.fnHighlight(clsWE.fnGetWe("(//table[@id='results']//tbody//tr[@role='row'])[" + intCount + "]"));
                                clsReportResult.fnLog("Search Results", "The Search with Client Name Criteria was done successfully", "Pass", true, false);
                                blFound = true;
                                break;
                            case "CLIENTDATALOCATIONSEARCH":
                                clsReportResult.fnLog("Search Results", ">>>>> The Search with Client Data Location Criteria Starts ", "Info", false, false);
                                clsMG.fnHighlight(clsWE.fnGetWe("(//table[@id='results']//tbody//tr[@role='row'])[" + intCount + "]"));
                                clsReportResult.fnLog("Search Results", "The Search with Client Data Location Criteria was done successfully", "Pass", true, false);
                                blFound = true;
                                break;
                            case "CLAIMNUMBERSEARCH":
                                clsReportResult.fnLog("Search Results", ">>>>> The Claim Number Search Starts ", "Info", false, false);
                                string strClaimRetrived = clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[2])[" + intCount + "]"), "Get Row property", "innerText", false, false);
                                if (strClaimRetrived == pobjData.fnGetValue("ClaimNumber", ""))
                                {
                                    clsMG.fnHighlight(clsWE.fnGetWe("(//table[@id='results']//tr//td[2])[" + intCount + "]"));
                                    clsReportResult.fnLog("Search Results", "The Search with Claim Number Criteria was done successfully", "Pass", true, false);
                                    blFound = true;
                                }
                                else 
                                {
                                    clsReportResult.fnLog("Search Results", "The Search with Claim Number has failed. it expected claim #: "+ pobjData.fnGetValue("ClaimNumber", "") + " but was found"+ strClaimRetrived + ".", "Fail", true, false);
                                    blFound = true;
                                    blResult = false;
                                }
                                break;
                            case "CONFIRMATIONNUMBERSEARCH":
                                clsReportResult.fnLog("Search Results", ">>>>> The Confirmation Number Search Starts ", "Info", false, false);
                                string strConfNoRetrived = clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[10])[" + intCount + "]"), "Get Row property", "innerText", false, false);
                                if (strConfNoRetrived == pobjData.fnGetValue("ConfNo", ""))
                                {
                                    clsMG.fnHighlight(clsWE.fnGetWe("(//table[@id='results']//tr//td[10])[" + intCount + "]"));
                                    clsReportResult.fnLog("Search Results", "The Search with Confirmation Number Criteria was done successfully", "Pass", true, false);
                                    blFound = true;
                                }
                                else
                                {
                                    clsReportResult.fnLog("Search Results", "The Search with Confirmation Number has failed. it expected Confirmation #: " + pobjData.fnGetValue("ConfNo", "") + " but was found: " + strConfNoRetrived + ".", "Fail", true, false);
                                    blFound = true;
                                    blResult = false;
                                }
                                break;
                            case "NODISSEMINATIONDETAILS":
                                //View Row
                                clsReportResult.fnLog("Search Results", ">>>>> The Dissemination Detail Verification Starts ", "Info", false, false);
                                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                Thread.Sleep(TimeSpan.FromSeconds(1));
                                clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                //Verify that no dissemination details are displayed
                                if (!clsMG.IsElementPresent("//table[@id='results']"))
                                {
                                    clsWE.fnScrollTo(clsWE.fnGetWe("//div[@id='bottomMenu']"),"Scroll to bottom page", false, false);
                                    clsReportResult.fnLog("Search Results", "The Dissemination Details section is not displayed in the claim details as expected", "Pass", true, false);
                                    blFound = true;
                                }
                                else 
                                {
                                    clsWE.fnScrollTo(clsWE.fnGetWe("//div[@id='bottomMenu']"),"Scroll to bottom page", false, false);
                                    clsReportResult.fnLog("Search Results", "The Dissemination Details section should not be displayed in the claim details.", "Fail", true, false);
                                    blResult = false;
                                    blFound = true;
                                }
                                break;
                            case "RESUMESTATUSOTHERUSER":
                                //Verify it select a different user as logged
                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[8])["+intCount+"]"), "Get Row property", "innerText", false, false) != strCurrentUser) 
                                {
                                    clsReportResult.fnLog("Search Results", ">>>>> The Resume Status Verification for Other Users Starts ", "Info", false, false);
                                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(4));
                                    clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                    clsWE.fnScrollTo(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Scroll to Intake Details Label", false, false);
                                    blFound = true;
                                    if (!clsMG.IsElementPresent("//button[text()='Resume']"))
                                    { clsReportResult.fnLog("Search Results", "The Resume button is not displayed for as expected in claim created for other users.", "Pass", true, false); }
                                    else
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume button should not be displayed in claims created for other users.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                break;
                            case "RESUMESTATUSSAMEUSER":
                                //Verify it select a different user as logged
                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[8])[" + intCount + "]"), "Get Row property", "innerText", false, false) == strCurrentUser)
                                {
                                    clsReportResult.fnLog("Search Results", ">>>>> The Resume Status Verification for Current User Logged In Starts ", "Info", false, false);
                                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(4));
                                    clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                    clsWE.fnScrollTo(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Scroll to Intake Details Label", false, false);
                                    blFound = true;
                                    if (!clsMG.IsElementPresent("//button[text()='Resume']"))
                                    { clsReportResult.fnLog("Search Results", "The Resume button is not displayed for as expected for intake only users.", "Pass", true, false); }
                                    else
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume button should not be displayed for intake only users.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                break;
                            case "ADMINRESUMESTATUSOTHERUSER":
                                //Verify it select a different user as logged
                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[8])[" + intCount + "]"), "Get Row property", "innerText", false, false) != strCurrentUser)
                                {
                                    clsReportResult.fnLog("Search Results", ">>>>> The Resume Status with Admin UserVerification for Claims Created By Other Users Starts ", "Info", false, false);
                                    //Click on Edit Button
                                    blFound = true;
                                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(4));
                                    clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                    //Verify Resume and Cancel Buttom
                                    if (clsMG.IsElementPresent("//div[@class='card-body']//button[text()='Resume']") && clsMG.IsElementPresent("//div[@class='card-body']//button[text()='Cancel']"))
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume/Cancel button are displaye on Intake Details Page.", "Pass", true, false);
                                        //Verify click on Resume Button
                                        clsWE.fnClick(clsWE.fnGetWe("//div[@class='card-body']//button[text()='Resume']"), "Click Edit Button", false, false);
                                        Thread.Sleep(TimeSpan.FromSeconds(4));
                                        //clsWE.fnPageLoad(clsWE.fnGetWe("//*[@data-bind='text: IntakeName']"), "Intake Name Page", false, false);
                                        if (clsWE.fnElementExist("Intake Name", "//*[@data-bind='text: IntakeName']", false, false))
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resume button opens the Intake Flow Page successfully.", "Pass", true, false);
                                            //Go back to previous screen
                                            clsWebBrowser.objDriver.Navigate().Back();
                                            Thread.Sleep(TimeSpan.FromSeconds(4));
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                            //Verify Cancel Button
                                            clsReportResult.fnLog("Search Results", "Verifying Cancel Button", "Info", true, false);
                                            clsWE.fnClick(clsWE.fnGetWe("//div[@class='card-body']//button[text()='Cancel']"), "Click Edit Button", false, false);
                                            if (clsWE.fnElementExist("Cancel Intake Popup", "//div[@id='abandonModal' and contains(@style, 'display: block')]", false, false))
                                            {
                                                clsMG.fnSelectDropDownWElm("Abandon Reason", "//div[select[contains(@data-bind, 'abandonReason')]]//input", "Other reasons", false, false, "", false);
                                                clsMG.fnCleanAndEnterText("Description", "//textarea[@id='abandonDescription']", "Claim Canceled by Automation Script.", true, false, "", false);
                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Confirm']"), "Click Confirm Button", false, false);
                                                //Verify it Redirects to home page
                                                clsWE.fnPageLoad(clsWE.fnGetWe("//*[@id='recentCalls']"), "Home Page", false, false);
                                                if (clsWE.fnElementExist("Home Page", "//*[@id='recentCalls']", false, false))
                                                {
                                                    clsReportResult.fnLog("Search Results", "After confirm and abandon the claim, the page was redirected to Home Page as expected.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Search Results", "After confirm and abandon the claim, the page was not redirected to Home Page.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                            else 
                                            {
                                                clsReportResult.fnLog("Search Results", "The Cancel Intake Popup is not displayed after click on Cancel Button.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else 
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resume button did not open the Intake Flow Page as expected.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    else 
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume/Cancel button is not displaye on Intake Details and should be displayed both.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                break;
                            case "ADMINRESUMESTATUSSAMEUSER":
                                //Verify it select a different user as logged
                                if (clsWE.fnGetAttribute(clsWE.fnGetWe("(//table[@id='results']//tr//td[8])[" + intCount + "]"), "Get Row property", "innerText", false, false) == strCurrentUser)
                                {
                                    blFound = true;
                                    clsReportResult.fnLog("Search Results", ">>>>> The Resume Status with Admin UserVerification for Claims Created By Same Users Starts ", "Info", false, false);
                                    //Click on Edit Button
                                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    clsWE.fnClick(clsWE.fnGetWe("(//table[@id='results']//tr//a)[" + intCount + "]"), "Click Edit Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(4));
                                    clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                    //Verify Resume and Cancel Buttom
                                    if (clsMG.IsElementPresent("//div[@class='card-body']//button[text()='Resume']") && clsMG.IsElementPresent("//div[@class='card-body']//button[text()='Cancel']"))
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume/Cancel button are displaye on Intake Details Page.", "Pass", true, false);
                                        //Verify click on Resume Button
                                        clsWE.fnClick(clsWE.fnGetWe("//div[@class='card-body']//button[text()='Resume']"), "Click Edit Button", false, false);
                                        Thread.Sleep(TimeSpan.FromSeconds(4));
                                        //clsWE.fnPageLoad(clsWE.fnGetWe("//*[@data-bind='text: IntakeName']"), "Intake Name Page", false, false);
                                        if (clsWE.fnElementExist("Intake Name", "//*[@data-bind='text: IntakeName']", false, false))
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resume button opens the Intake Flow Page successfully.", "Pass", true, false);
                                            //Go back to previous screen
                                            clsWebBrowser.objDriver.Navigate().Back();
                                            Thread.Sleep(TimeSpan.FromSeconds(4));
                                            clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Intake Details']"), "Intake Details Page", false, false);
                                            //Verify Cancel Button
                                            clsReportResult.fnLog("Search Results", "Verifying Cancel Button", "Info", true, false);
                                            clsWE.fnClick(clsWE.fnGetWe("//div[@class='card-body']//button[text()='Cancel']"), "Click Edit Button", false, false);
                                            if (clsWE.fnElementExist("Cancel Intake Popup", "//div[@id='abandonModal' and contains(@style, 'display: block')]", false, false))
                                            {
                                                clsMG.fnSelectDropDownWElm("Abandon Reason", "//div[select[contains(@data-bind, 'abandonReason')]]//input", "Other reasons", false, false, "", false);
                                                clsMG.fnCleanAndEnterText("Description", "//textarea[@id='abandonDescription']", "Claim Canceled by Automation Script.", true, false, "", false);
                                                clsWE.fnClick(clsWE.fnGetWe("//button[text()='Confirm']"), "Click Confirm Button", false, false);
                                                //Verify it Redirects to home page
                                                clsWE.fnPageLoad(clsWE.fnGetWe("//*[@id='recentCalls']"), "Home Page", false, false);
                                                if (clsWE.fnElementExist("Home Page", "//*[@id='recentCalls']", false, false))
                                                {
                                                    clsReportResult.fnLog("Search Results", "After confirm and abandon the claim, the page was redirected to Home Page as expected.", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Search Results", "After confirm and abandon the claim, the page was not redirected to Home Page.", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Search Results", "The Cancel Intake Popup is not displayed after click on Cancel Button.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resume button did not open the Intake Flow Page as expected.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Search Results", "The Resume/Cancel button is not displaye on Intake Details and should be displayed both.", "Fail", true, false);
                                        blResult = false;
                                    }
                                }
                                break;
                            case "PAGINATIONREVIEW":
                                clsWE.fnScrollTo(clsWE.fnGetWe("//div[@id='bottomMenu']"), "Scroll to Pagination", true, false);
                                IList<IWebElement> lsNewPagButton = clsWebBrowser.objDriver.FindElements(By.XPath("//li[contains(@class, 'paginate_button page-item')]"));
                                blFound = true;
                                //No Retuls
                                if (lsNewPagButton.Count == 2)
                                {
                                    foreach (IWebElement element in lsNewPagButton) 
                                    {
                                        if (element.GetAttribute("class").Contains("disabled"))
                                        {
                                            clsReportResult.fnLog("Search Results", "The pagination button is disabled as expected for WebElement: " + element.GetAttribute("id") + ".", "Pass", true, false);
                                        }
                                        else 
                                        {
                                            clsReportResult.fnLog("Search Results", "The pagination button should disabled WebElement: " + element.GetAttribute("id") + " but is enable: "+ element.GetAttribute("class") + ".", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                }
                                else if (lsNewPagButton.Count == 3)
                                {
                                    int intSelected = 1;
                                    foreach (IWebElement element in lsNewPagButton)
                                    {
                                        if (intSelected != 2)
                                        {
                                            if (!element.GetAttribute("class").Contains("disabled"))
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button is active as expected for WebElement button [1].", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button should be enabled for button [1].", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            if (element.GetAttribute("class").Contains("disabled"))
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button is disabled as expected for WebElement: " + element.GetAttribute("id") + ".", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button should disabled WebElement: " + element.GetAttribute("id") + " but is enable: " + element.GetAttribute("class") + ".", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        intSelected++;
                                    }
                                }
                                else if (lsNewPagButton.Count > 3) 
                                {
                                    IWebElement elmTotalButtons = clsWebBrowser.objDriver.FindElement(By.XPath("//ul[@class='pagination']//li[last()-1]"));
                                    string intTotalButtons = elmTotalButtons.GetAttribute("innerText");
                                    for (int intOption = 1; intOption <= Convert.ToInt32(intTotalButtons)+2; intOption++) 
                                    {
                                        //Verify Previous Button
                                        if (intOption == 1)
                                        {
                                            IWebElement elmPrevious = clsWebBrowser.objDriver.FindElement(By.XPath("//li[contains(@class, 'paginate_button page-item')][1]"));
                                            if (elmPrevious.GetAttribute("class").Contains("disabled"))
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button is disabled as expected for WebElement: " + elmPrevious.GetAttribute("id") + ".", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Search Results", "The pagination button should disabled WebElement: " + elmPrevious.GetAttribute("id") + " but is enable: " + elmPrevious.GetAttribute("class") + ".", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else 
                                        {
                                            //Verify Page from1 to N
                                            if (intOption >= 3 && intOption < Convert.ToInt32(Convert.ToInt32(intTotalButtons)) + 2)
                                            {
                                                IWebElement elmNext = clsWebBrowser.objDriver.FindElement(By.XPath("//li[@id='results_next']"));
                                                elmNext.Click();
                                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                                IWebElement elmCurrentButton = clsWebBrowser.objDriver.FindElement(By.XPath("//li[contains(@class, 'paginate_button page-item') and a[text()='"+ (intOption-2)+ "']]"));
                                                //elmCurrentButton = clsWebBrowser.objDriver.FindElement(By.XPath("//li[contains(@class, 'paginate_button page-item')][" + intOption + "]"));
                                                if (!elmCurrentButton.GetAttribute("class").Contains("disabled"))
                                                {
                                                    clsReportResult.fnLog("Search Results", "The pagination button is active as expected for WebElement button [" + (intOption-1) + "].", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Search Results", "The pagination button should be enabled for button [" + (intOption-1) + "].", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                            else if (intOption == Convert.ToInt32(Convert.ToInt32(intTotalButtons))+2)
                                            {
                                                IWebElement elmNext = clsWebBrowser.objDriver.FindElement(By.XPath("//li[@id='results_next']"));
                                                if (elmNext.GetAttribute("class").Contains("disabled"))
                                                {
                                                    clsReportResult.fnLog("Search Results", "The pagination button is disabled as expected for WebElement: " + elmNext.GetAttribute("id") + ".", "Pass", true, false);
                                                }
                                                else
                                                {
                                                    clsReportResult.fnLog("Search Results", "The pagination button should disabled WebElement: " + elmNext.GetAttribute("id") + " but is enable: " + elmNext.GetAttribute("class") + ".", "Fail", true, false);
                                                    blResult = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "SORTINGREVIEW":
                                //Verify if results exist
                                IList<IWebElement> lsHeaders = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//th"));
                                if (lsHeaders.Count > 0)
                                {
                                    //Iterate
                                    for (int intElement=2; intElement <= lsHeaders.Count; intElement++) 
                                    {
                                        //Get Initial Values
                                        IWebElement objHeader = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//th[" + intElement + "]"));
                                        objHeader.Click();
                                        Thread.Sleep(TimeSpan.FromSeconds(1));
                                        IWebElement objRow = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//tbody//tr[1]//td[" + intElement + "]"));
                                        string strName = objHeader.GetAttribute("innerText");
                                        string strInitial = objHeader.GetAttribute("aria-sort");
                                        string strInitalValRow = objRow.GetAttribute("innerText");

                                        //Get Final Values
                                        objHeader = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//th[" + intElement + "]"));
                                        objHeader.Click();
                                        Thread.Sleep(TimeSpan.FromSeconds(1));
                                        objRow = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//tbody//tr[1]//td[" + intElement + "]"));
                                        string strFinal = objHeader.GetAttribute("aria-sort");
                                        string strFinalValRow = objRow.GetAttribute("innerText");

                                        //Verify Sorting property
                                        if (strInitial != strFinal)
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting property on " + strName + " header is changing from " + strInitial + " to " + strFinal + " as expected.", "Pass", true, false);
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting property on " + strName + " header is not changing after click on it and keeps the " + strInitial + " sorting.", "Warning", true, false);
                                            blResult = false;
                                        }
                                        //Verify Sorted Value
                                        if (strInitalValRow != strFinalValRow)
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting on " + strName + " row is changing from " + strInitalValRow + " to " + strFinalValRow + " as expected.", "Pass", true, false);
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting on " + strName + " row is not changing and return the same value: " + strInitalValRow + ".", "Warning", true, false);
                                            blResult = false;
                                        }
                                    }
                                    blFound = true;
                                }
                                else
                                {
                                    //Iterate
                                    for (int intElement = 2; intElement <= lsHeaders.Count; intElement++)
                                    {
                                        //Get Initial Values
                                        IWebElement objHeader = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//th[" + intElement + "]"));
                                        objHeader.Click();
                                        Thread.Sleep(TimeSpan.FromSeconds(1));
                                        string strName = objHeader.GetAttribute("innerText");
                                        string strInitial = objHeader.GetAttribute("aria-sort");

                                        //Get Final Values
                                        objHeader = clsWebBrowser.objDriver.FindElement(By.XPath("//table[@id='results']//th[" + intElement + "]"));
                                        objHeader.Click();
                                        Thread.Sleep(TimeSpan.FromSeconds(1));
                                        string strFinal = objHeader.GetAttribute("aria-sort");

                                        //Verify Sorting property
                                        if (strInitial != strFinal)
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting property on " + strName + " header is changing from " + strInitial + " to " + strFinal + " as expected.", "Pass", true, false);
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The sorting property on " + strName + " header is not changing after click on it and keeps the " + strInitial + " sorting.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                    blFound = true;
                                }
                                break;
                            case "OLDERDATES":
                                blFound = true;
                                clsWE.fnClick(clsWE.fnGetWe("//table[@id='results']//thead//th[contains(@aria-label, 'CREATED DATE')]"), "Created Date Sorting", true);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//table[@id='results']//thead//th[contains(@aria-label, 'CREATED DATE')]"), "Sorting", false, false);
                                string strColDate = clsWE.fnGetAttribute(clsWE.fnGetWe("//table[@id='results']//tbody//tr[1]//td[6]"), "Column Value", "innerText", false, false);
                                var initialDate = DateTime.Parse(pobjData.fnGetValue("StartDate", ""));
                                var tableDate = DateTime.Parse(strColDate);
                                if (tableDate >= initialDate)
                                {
                                    clsReportResult.fnLog("Search Results", "The table returns greater dates han: "+ initialDate.ToString() + " as expected.", "Pass", true, false);
                                }
                                else 
                                {
                                    clsReportResult.fnLog("Search Results", "The table should return dates equal or greater than " + initialDate.ToString("MM/dd/yyyy") + " but it returned "+ tableDate.ToString("MM/dd/yyyy") + ".", "Fail", true, false);
                                    blResult = false;
                                }
                                break;
                            case "LOBRESTRICTION":
                                blFound = true;
                                clsWE.fnScrollTo(clsWE.fnGetWe("//input[contains(@data-bind, 'ConfirmationNumber')]"), "Scroll Up", true, false);
                                //Click LOB
                                clsWE.fnClick(clsWE.fnGetWe("//div[select[contains(@data-bind, 'SearchParameters.Lob')]]//input"), "LOB Dropdown", false);
                                if (pobjData.fnGetValue("RestrictedLOB", "").Contains(";"))
                                {
                                    string[] arrLOB = pobjData.fnGetValue("RestrictedLOB", "").Split(';');
                                    for (int intLob = 0; intLob <= arrLOB.Length; intLob++ ) 
                                    {
                                        if (clsMG.IsElementPresent("//li[@class='select2-results__option' and contains(text(), '" + arrLOB[intLob] + "')]"))
                                        {
                                            clsReportResult.fnLog("Search Results", "The Restricted LOB: " + arrLOB[intLob] + " was found as expected .", "Pass", true, false);
                                        }
                                        else 
                                        {
                                            clsReportResult.fnLog("Search Results", "The Restricted LOB "+ arrLOB[intLob] + " was not found in the dropdown.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                }
                                else 
                                {
                                    IList<IWebElement> lsLOB = clsWebBrowser.objDriver.FindElements(By.XPath("//li[contains(@class, 'select2-results__option ')]"));
                                    foreach (var value in lsLOB) 
                                    {
                                        if (value.GetAttribute("innerText").ToUpper() == pobjData.fnGetValue("RestrictedLOB", "").ToUpper())
                                        {
                                            clsReportResult.fnLog("Search Results", "The Restricted LOB: " + value.GetAttribute("innerText").ToUpper() + " was found as expected .", "Pass", true, false);
                                        }
                                        else 
                                        {
                                            clsReportResult.fnLog("Search Results", "The Restricted LOB should be "+ pobjData.fnGetValue("RestrictedLOB", "") + " but was found "+ value + ".", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                }

                                break;
                            default:
                                break;

                        }
                        if (blFound) { break; }
                    }

                    if (!blFound) 
                    {
                        if (!clsMG.IsElementPresent("//li[@id='results_next']/a")) { clsWebBrowser.objDriver.Navigate().Back(); }
                        clsWE.fnPageLoad(clsWE.fnGetWe("//li[@id='results_next']/a"), "Wait Next Button", false, false);
                        if (intPage + 1 > 1 && lsPagButton.Count > 3) { clsWE.fnClick(clsWE.fnGetWe("//li[@id='results_next']/a"), "Next Button", false, false); }
                    }
                    if (blFound) { break; }
                }

                //Verify that Resume button is not displayed
                if (!blFound)
                {
                    clsReportResult.fnLog("Search Results", "After iterate by Page/Row no claim with the criteria provided were found.", "Fail", true, false);
                    blResult = false;
                }
            }
            else 
            {
                if (pobjData != null)
                {
                    if (pobjData.fnGetValue("NegativeScenario", "NO").ToUpper() == "YES" || pobjData.fnGetValue("NegativeScenario", "FALSE").ToUpper() == "TRUE")
                    {
                        clsReportResult.fnLog("Search Results", "The search does not return any record with the criteria provided as expected.", "Pass", true, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Search Results", "The search does not return any record with the criteria provided.", "Fail", true, false);
                        blResult = false;
                    }
                }
                else 
                {
                    if (pstrSearchType.ToUpper() == "YES" || pstrSearchType.ToUpper() == "TRUE")
                    {
                        clsReportResult.fnLog("Search Results", "The search does not return any record with the criteria provided as expected.", "Pass", true, false);
                    }
                    else
                    {
                        clsReportResult.fnLog("Search Results", "The search does not return any record with the criteria provided.", "Fail", true, false);
                        blResult = false;
                    }
                }

                
            }

            return blResult;
        }

        public bool fnVerifyTrainingModeClaims()
        {
            string[] arrClaim = { clsConstants.strSubmitClaimTrainingMode, clsConstants.strResumeClaimTrainingMode };
            bool blResult = true;
            clsReportResult.fnLog("Search Training Claims", "<<<<<<<<<< The Search Training Claims Functions Starts. >>>>>>>>>>", "Info", false);
            foreach (string claim in arrClaim)
            {
                //Search Claim
                fnSearchResults("Yes", "", "", "", claim, "", "");
                IList<IWebElement> lsRow = clsWebBrowser.objDriver.FindElements(By.XPath("//table[@id='results']//tbody//tr[@role='row']"));
                if (lsRow.Count > 0)
                {
                    clsReportResult.fnLog("Search Training Claims", "The claims created on Training Mode should not be displayed out of training mode.", "Fail", true);
                    blResult = false;
                }
                else
                {
                    clsReportResult.fnLog("Search Training Claims", "The claims created on Training Mode are not retrived on normal session as expected.", "Pass", true);
                }
            }
            return blResult;
        }

    }
}
