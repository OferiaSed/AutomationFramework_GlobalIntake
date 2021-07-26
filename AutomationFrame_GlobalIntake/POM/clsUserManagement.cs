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
    class clsUserManagment
    {
        private clsWebElements clsWE = new clsWebElements();
        private clsMegaIntake clsMG = new clsMegaIntake();

        public bool fnUserMagmtWebUser(string pstrSetNo)
        {
            bool blResult = true;

            clsData objData = new clsData();
            clsReportResult.fnLog("User Management", "<<<<<<<<<< User Management Function Starts >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserMGMT");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    switch (objData.fnGetValue("Action", "").ToUpper())
                    {
                        case "SEARCH":
                            clsMG.fnHamburgerMenu("User Management;Web Users");
                            clsMG.fnCleanAndEnterText("Username", "//input[@placeholder='Username']", objData.fnGetValue("Username", ""), false, false, "", false);
                            clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search", false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            if (clsWE.fnElementExist("User Record", "//tr[td[contains(text(), '" + objData.fnGetValue("Username", "") + "')]]//a", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//tr[td[contains(text(), '" + objData.fnGetValue("Username", "") + "')]]//a"), "Edit Record", false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Edit User']"), "Edit User", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The search did not return any record with the criteria provided.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "ENABLE":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'toggleEnableUser')]"), "Enable User", false);
                                if (clsWE.fnElementExist("Success Message", "//div[@class='md-toast md-toast-success']", false))
                                { clsReportResult.fnLog("User Management", "The user was enabled successfully.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "An error message was identified after enable the user.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be enabled since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "UNLOCK":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'unlockUser')]"), "Unlock User", false);
                                if (clsWE.fnElementExist("Success Message", "//div[@class='md-toast md-toast-success']", false))
                                { clsReportResult.fnLog("User Management", "The user was unlocked successfully.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "An error message was identified after unlock the user.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be unlocked since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "SAVE":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'saveChanges')]"), "Save Record", false);
                                if (clsWE.fnElementExist("Success Message", "//div[@class='md-toast md-toast-success']", false))
                                { clsReportResult.fnLog("User Management", "The changes was saved successfully.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "An error message was identified after save the changes.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot save the changes since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            if (blResult)
            { clsReportResult.fnLog("User Management", "The User Management Function was executed successfully.", "Pass", true); }
            else
            { clsReportResult.fnLog("User Management", "The User Management Function was not executed successfully.", "Fail", true); }

            return blResult;
        }




    }
}
