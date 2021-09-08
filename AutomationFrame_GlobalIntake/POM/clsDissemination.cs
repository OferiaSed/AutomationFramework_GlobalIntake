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
                        //Verify Action
                        var actionDriver = objData.fnGetValue("Action");
                        var actions = actionDriver.Split(';').ToList();
                        actions.ForEach(action =>
                        {
                            //Clear Filter
                            clsWE.fnClick(clsWE.fnGetWe(DisseminationModel.strClearButton), "Clear Filter", false, false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            switch (action.ToUpper()) 
                            {
                                case "VERIFYEMAILOFFICENUMBER":
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


    }
}
