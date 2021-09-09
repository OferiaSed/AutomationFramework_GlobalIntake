using AutomationFrame_GlobalIntake.Models;
using AutomationFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.POM
{
    public class clsDissemination
    {
        private clsWebElements clsWE = new clsWebElements();
        private clsMegaIntake clsMG = new clsMegaIntake();
        private clsIntakeFlow clsIF = new clsIntakeFlow();


        public bool fnDisseminationPage(string pstrSetNo) 
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Dissemination Page", "<<<<<<<<<< The Dissemination Page Starts. >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "Dissemination");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //Go to New Intake and select Intake
                    clsMG.fnHamburgerMenu("Disseminations");
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(DisseminationModel.strDisseminationPage), TimeSpan.FromSeconds(1), 10);
                    if (clsWE.fnElementExist("Verify Dissemination Page", DisseminationModel.strDisseminationPage, true))
                    {
                        //Clear Filter
                        clsWE.fnClick(clsWE.fnGetWe(DisseminationModel.strClearButton), "Clear Filter", false, false);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        clsMG.fnGoTopPage();

                        //Verify Action
                        var actionDriver = objData.fnGetValue("Action");
                        var actions = actionDriver.Split(';').ToList();
                        actions.ForEach(action =>
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            switch (action.ToUpper()) 
                            {
                                case "VERIFYEMAILOFFICENUMBER":
                                    break;
                                case "VERIFYDISSEMINATION":
                                    if (objData.fnGetValue("DisseminationType", "JurisDissemination").ToUpper() == "JURISDISSEMINATION")
                                    {
                                        clsReportResult.fnLog("Search Results", ">>>>> The Search for JURIS Dissemination Starts ", "Info", false, false);
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//table[@id='results']"), TimeSpan.FromSeconds(2), 10);
                                        //Click edit button
                                        clsWE.fnClick(clsWE.fnGetWe("(//tr[td[text()='JURISDissemination']]//a[@class='btn-floating btn-sm details-button'])[1]"), "Click Details Button", true, false);
                                        //Loading Intake Details screen
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//div[@id='contentModal' and contains(@style, 'display: block;')]//div[contains(@class,'modal-content')]"), TimeSpan.FromSeconds(2), 10);
                                        //Is dissemination modal header displayed
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//p[contains(text(),'Dissemination Details')]"), TimeSpan.FromSeconds(2), 10);
                                        if (clsMG.IsElementPresent("//div[contains(text(),'Success for DisseminationInstance Id')]"))
                                        {
                                            clsReportResult.fnLog("Search Results", "The Success message is present on screen", "Pass", true, false);
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The Success message is not present on screen", "Fail", true, false);
                                        }
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Search Results", "Please enter a Disemination Type to contiue with the scenario", "Fail", false, false);
                                    }
                                    break;
                                case "RESENDFAILDISSEMINATION":
                                    if (objData.fnGetValue("DisseminationType", "JurisDissemination").ToUpper() == "JURISDISSEMINATION")
                                    {
                                        clsReportResult.fnLog("Search Results", ">>>>> The Search for JURIS Fail Dissemination Starts ", "Info", false, false);
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//table[@id='results']"), TimeSpan.FromSeconds(2), 10);
                                        //Click to select juris
                                        if (objData.fnGetValue("SelectToResend", "False").ToUpper() == "YES" || objData.fnGetValue("SelectToResend", "False").ToUpper() == "TRUE")
                                        { clsWE.fnClick(clsWE.fnGetWe("(//input[contains(@class,'form-check-input dissemination-check')]/following-sibling::label[contains(@class,'form-check-label')])[1]"), "Select Juris to Resend", false); }
                                        //Waiting resend button on screen
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//button[contains(text(),'Resend Selected')]"), TimeSpan.FromSeconds(2), 10);
                                        clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Resend Selected')]"), "Select Juris to Resend", false);
                                        //Is dissemination modal header displayed
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//p[contains(text(),'Resend Disseminations')]"), TimeSpan.FromSeconds(2), 10);
                                        if (clsMG.IsElementPresent("//div[@id='megaModalDialog' and contains(@style, 'display: block;')]//div[contains(@class,'modal-content')]"))
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resend Dissemination modal is present on screen", "Pass", true, false);
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("Search Results", "The Resend Dissemination modal is not present on screen", "Fail", true, false);
                                        }
                                    }
                                    else
                                    {
                                        clsReportResult.fnLog("Search Results", "Please enter a Disemination Type to contiue with the scenario", "Fail", false, false);
                                    }
                                    break;
                                case "SEARCH":
                                    clsWE.fnClick(clsWE.fnGetWe(DisseminationModel.strSearchButton), "Search Button", false, false);
                                    Thread.Sleep(TimeSpan.FromSeconds(3));
                                    break;
                                case "FILLDATA":
                                    //Select Client
                                    if (objData.fnGetValue("ClientNo", "") != "" && objData.fnGetValue("ClientName", "") != "") 
                                    {
                                        clsMG.fnGoTopPage();
                                        if (!clsIF.fnSelectClientPopup(objData.fnGetValue("ClientNo", ""), objData.fnGetValue("ClientName", ""))) { blResult = false; } ; 
                                    }
                                    //Dissemination type
                                    clsMG.fnSelectDropDownWElm("Dissemination Type", DisseminationModel.strDisseminationType, objData.fnGetValue("DisseminationType", ""), false, false);
                                    //Dissemination Status
                                    clsMG.fnSelectDropDownWElm("Dissemination Status", DisseminationModel.strDisseminationStatus, objData.fnGetValue("DisseminationStatus", ""), false, false);
                                    //Start Date
                                    clsMG.fnEnterDatePicker("Start Date", DisseminationModel.strStartDate, objData.fnGetValue("StartDate", ""), false, false);
                                    //End Date
                                    clsMG.fnEnterDatePicker("End Date", DisseminationModel.strEndDate, objData.fnGetValue("EndDate", ""), false, false);
                                    //Dissemination Id
                                    clsMG.fnCleanAndEnterText("Dissemination Id", DisseminationModel.strDisseminationId, objData.fnGetValue("DisseminationId", ""), false, false, "", false);
                                    //Instance Id
                                    clsMG.fnCleanAndEnterText("Instance Id", DisseminationModel.strInstanceId, objData.fnGetValue("InstanceId", ""), false, false, "", false);
                                    //Confirmation Number
                                    clsMG.fnCleanAndEnterText("Confirmation Number", DisseminationModel.strConfirmationNumber, objData.fnGetValue("ConfirmationNumber", ""), false, false, "", false);
                                    //Claim Number
                                    if (objData.fnGetValue("Action", "VerifyDissemination").ToUpper() == "VERIFYDISSEMINATION")
                                    {
                                        string ClaimNumbervalue = GetExcelValue(32, "EventInfo", "ClaimNumber");
                                        clsMG.fnCleanAndEnterText("Claim Number", DisseminationModel.strClaimNumber, ClaimNumbervalue, false, false, "", false);

                                    }
                                    clsMG.fnCleanAndEnterText("Claim Number", DisseminationModel.strClaimNumber, objData.fnGetValue("ClaimNumber", ""), false, false, "", false);
                                    //Group by
                                    clsMG.fnSelectDropDownWElm("Group by", DisseminationModel.strGroupby, objData.fnGetValue("GroupBy", ""), false, false);
                                    break;
                            }
                        });
                    }
                    else
                    {
                        clsReportResult.fnLog("Dissemination Page", "The Dissemination Page was not opened and test cannot continue", "Fail", true, false);
                        blResult = false;
                    }
                }
            }
            return blResult;
        }


        public string GetExcelValue(int intRow, string strDataSheet, string strColumn)
        {
            clsData objGetConfNumber = new clsData();
            objGetConfNumber.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], strDataSheet);
            objGetConfNumber.CurrentRow = intRow;
            var strValue = objGetConfNumber.fnGetValue(strColumn, "");
            return strValue;
        }

    }
}
