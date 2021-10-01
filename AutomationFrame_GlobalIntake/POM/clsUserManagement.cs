using AutomationFrame_GlobalIntake.Models;
using AutomationFrame_GlobalIntake.Utils;
using MyUtils.Email;
using AutomationFramework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        private clsLogin clsLN = new clsLogin();

        /// <summary>
        /// Last used username, it can be used across tests and actions
        /// </summary>
        private string strLastUsername;

        public bool fnUserManagementPage(string pstrSetNo)
        {
            bool blResult = true;

            clsData objData = new clsData();
            clsReportResult.fnLog("User Management", "<<<<<<<<<< User Management Function Starts >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "UserMGMT");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    var action = objData.fnGetValue("Action", "");
                    var templateFoundStep = "Assert User Import Template was found";

                    #region local functions
                    void ExportUsersExcelTemplate()
                    {
                        var umPage1 = new UserManagementModel(clsWebBrowser.objDriver, clsMG);
                        var exportButton = clsWebBrowser.objDriver.FindElement(umPage1.objExportUsersSelector);
                        clsWebBrowser.objDriver.fnScrollToElement(exportButton);
                        clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false);
                        clsWE.fnClick(exportButton, "Export Button", false, false);
                        var successToaster = umPage1.fnUntilSuccessToasterVisible();
                        // TODO: Add Warn to AutomationFramework
                        clsReportResult.fnLog("Success Toaster was showed", "Success Toaster was showed after clicking Expor Users", successToaster ? "Pass" : "Warn", true);
                    }

                    string ReadExportedEmailAttachments()
                    {
                        var ExportedExcelPath = "";
                        var blAttachmentFound = clsMG.fnGenericWait(
                        () =>
                        {
                            var readEmail = clsUtils.fnFindGeneratedEmail(objData.fnGetValue("SetCredentials", ""), "Sedgwick Global Intake - UAT - User Export Completed", "User Export job has completed with a status of Success", false, true);
                            ExportedExcelPath = readEmail.Attachments.SingleOrDefault(path => path.Contains("SedgwickAllClientAccessUsersAsOf"));
                            return !string.IsNullOrEmpty(ExportedExcelPath);
                        },
                               TimeSpan.FromSeconds(15),
                               3
                        );
                        clsReportResult.fnLog(templateFoundStep, templateFoundStep, blAttachmentFound ? "Info" : "Warn", false);
                        return ExportedExcelPath;
                    }

                    var email = new clsEmailV2(objData.fnGetValue("Email", ""), objData.fnGetValue("Password", ""), clsEmailV2.emServer.POPGMAIL, true);
                    string ReadUsersExcelTemplateFromEmail(TimeSpan spareTime, int maxAttempts)
                    {
                        var emailInfo = new {
                            subject = "Sedgwick Global Intake - UAT - User Export Completed",
                            content = "User Export job has completed with a status of Success"
                        };

                        // Finding Excel for Client Restricted Users
                        string pathToExcel = null;
                        var findingTemplateStep = "Look for User Import Template within emails";
                        clsReportResult.fnLog(findingTemplateStep, $"{findingTemplateStep} - Email info: {emailInfo}", "Info", false);

                        var attachmentFound = clsMG.fnGenericWait(
                            () =>
                            {
                                email.fnReadEmail(emailInfo.subject, emailInfo.content);
                                pathToExcel = email.Attachments.SingleOrDefault(path => path.Contains("SedgwickClientAccountUnitRestrictionsUsersAsOf"));
                                return !string.IsNullOrEmpty(pathToExcel);
                            },
                            spareTime,
                            maxAttempts
                        );

                        clsReportResult.fnLog(templateFoundStep, templateFoundStep, attachmentFound ? "Info" : "Warn", false);
                        return pathToExcel;
                    };

                    bool ClearUsersSheet(string pathToExcel) => clsUtils.TryExecute(
                        () =>
                        {
                            var documentToClear = new SLDocument(pathToExcel);
                            documentToClear.SelectWorksheet("Users");
                            var numberOfRows = documentToClear.GetWorksheetStatistics().NumberOfRows;
                            documentToClear.ClearRowContent(2, numberOfRows);
                            documentToClear.Save();
                        }
                    );

                    bool AddUserToExcelFile(string pathToExcel) => clsUtils.TryExecute(
                        () =>
                        {
                            var documentToAddUserTo = new SLDocument(pathToExcel);
                            documentToAddUserTo.SelectWorksheet("Users");
                            var parametersString = objData.fnGetValue("ImportExportParameters");
                            var values = parametersString.Split(',').Select(val => val.fnTextBetween("\"", "\"")).ToList();
                            var columns = clsConstants.ClientAccountUnitRestrictionsUserSheetColumns.ToList();
                            columns.ForEach(
                                column =>
                                {
                                    var columnIndex = columns.IndexOf(column);
                                    var value = values[columnIndex];
                                    if (column.Equals("UserName"))
                                    {
                                        this.ResolveUsername(value, objData);
                                        value = this.strLastUsername;
                                    }
                                    if (int.TryParse(value, out int intValue))
                                        documentToAddUserTo.SetCellValue(2, columnIndex + 1, intValue);
                                    else
                                        documentToAddUserTo.SetCellValue(2, columnIndex + 1, value);
                                }
                            );
                            documentToAddUserTo.Save();
                        }
                    );
                    bool searchUser()
                    {
                        var username = objData.fnGetValue("Username", this.strLastUsername);
                        clsMG.fnCleanAndEnterText("Username", "//input[@placeholder='Username']", username, false, false, "", false);
                        clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search", false);
                        return clsWebBrowser.objDriver.fnWaitUntilElementVisible(By.XPath("//tr[td[text()='" + username + "']]//a"), TimeSpan.FromSeconds(5));
                    }
                    #endregion
                    switch (action.ToUpper())
                    {
                        case "DELETE":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                //Click on Delete button
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(), 'Delete')]"), "Delete Button", false);
                                clsMG.fnGenericWait(() => clsMG.IsElementPresent("//div[@id='removeUserModal' and contains(@style, 'display: block')]"), TimeSpan.FromSeconds(1), 10);
                                //Wait Popup
                                clsWE.fnClick(clsWE.fnGetWe("//div[@id='removeUserModal' and contains(@style, 'display: block')]//button[text()='YES']"), "Yes Button", true);
                                //Verify Success Message
                                var succesMessage = clsMG.fnGenericWait(() => clsWE.fnElementExist("Success Message", "//div[@class='md-toast md-toast-success']", false), TimeSpan.FromSeconds(1), 10);
                                if (succesMessage)
                                { clsReportResult.fnLog("User Management", "The user was deleted as expected.", "Pass", true); }
                                else
                                { clsReportResult.fnLog("User Management", "The user was not deleted as expected.", "Fail", true); }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be enabled since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "VERIFYUNDO":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                //Edit Name, Last Name, Email
                                clsReportResult.fnLog("User Management", "Edit user before undo action.", "Info", true);
                                clsMG.fnGenericWait(() => clsMG.IsElementPresent("//input[@placeholder='First Name *']"), TimeSpan.FromSeconds(1), 10);
                                clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Header Intake", false);
                                clsMG.fnCleanAndEnterText("First Name", "//input[@placeholder='First Name *']", clsConstants.strUsrMngFirstName, false, false, "", false);
                                clsMG.fnCleanAndEnterText("Last Name", "//input[@placeholder='Last Name *']", clsConstants.strUsrMngLastName, false, false, "", false);
                                clsMG.fnCleanAndEnterText("Email", "//input[@placeholder='Email *']", clsConstants.strUsrMngEmail, false, false, "", false);
                                //Click on Undo button
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(), 'Undo')]"), "Undo Button", false);
                                clsReportResult.fnLog("User Management", "Edit user after undo action.", "Info", true);
                                //Get User Name, Last Name, Email
                                var strEditName = clsWE.fnGetAttribute(clsWE.fnGetWe("//input[@placeholder='First Name *']"), "Get Username", "value", false);
                                var strEditLastName = clsWE.fnGetAttribute(clsWE.fnGetWe("//input[@placeholder='Last Name *']"), "Get Last Name", "value", false);
                                var strEditEmail = clsWE.fnGetAttribute(clsWE.fnGetWe("//input[@placeholder='Email *']"), "Get Email", "value", false);
                                //Review Undo Action
                                if (strEditName != clsConstants.strUsrMngFirstName && strEditLastName != clsConstants.strUsrMngLastName && strEditEmail != clsConstants.strUsrMngEmail)
                                { clsReportResult.fnLog("User Management", "The Undo action was executed as expected", "Pass", true); }
                                else
                                { clsReportResult.fnLog("User Management", "The Undo action was done as expected", "Fail", true); }
                                clsConstants.strTempUserName = "";
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be enabled since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "EDIT":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                //Frist Name
                                clsMG.fnCleanAndEnterText("First Name", "//input[@placeholder='First Name *']", objData.fnGetValue("FirstName", ""), false, false, "", false);
                                //Last Name
                                clsMG.fnCleanAndEnterText("Last Name", "//input[@placeholder='Last Name *']", objData.fnGetValue("LastName", ""), false, false, "", false);
                                //Email Email *
                                clsMG.fnCleanAndEnterText("Email", "//input[@placeholder='Email *']", objData.fnGetValue("Email", ""), false, false, "", false);
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be enabled since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "SEARCH": 
                            fnNavigateToUserManagement(objData.fnGetValue("ScreenMenu", ""));
                            if (searchUser())
                             {
                                clsWE.fnClick(clsWE.fnGetWe($"//tr[td[text()='{objData.fnGetValue("Username", this.strLastUsername)}']]//a"), "Edit Record", false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Edit User']"), "Edit User", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The search did not return any record with the criteria provided.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "ENABLE":
                            //var x = clsUtils.fnIsElementEnabledVisible(By.XPath("//button[contains(@data-bind, 'unlockUser')]"), clsWebBrowser.objDriver);

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
                            if (clsUtils.fnIsElementEnabledVisible(By.XPath("//button[contains(@data-bind, 'unlockUser')]"), clsWebBrowser.objDriver))
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
                                clsReportResult.fnLog("User Management", "The unlock button is not displayed on the screen.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "SAVE":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'onSaveChanges')]"), "Save Record", false);
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
                        case "DISABLE":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//span[contains(text(), 'Disable')]"), "Disable User", false);
                                if (clsWE.fnElementExist("Success Message", "//div[@class='md-toast md-toast-success']", false))
                                { clsReportResult.fnLog("User Management", "The user was disabled successfully.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "An error message was identified after disable the user.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The user cannot be disable since Edit Page was not loaded.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "VALID RESET PASSWORD":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'resetPassword')]"), "Reset Password", false);
                                if (clsWE.fnElementExist("Error Message", "//div[@class='md-toast md-toast-success']", false))
                                { clsReportResult.fnLog("User Management", "The pop up message is displayed after trying to reset user password.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "The pop up message is not displayed after trying to reset user password.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "Edit user is not found on page.", "Fail", true);
                                blResult = false;
                            }
                            break;

                        case "INVALID RESET PASSWORD":
                            if (clsWE.fnElementExist("Edit Record", "//h4[text()='Edit User']", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'resetPassword')]"), "Reset Password", false);
                                if (clsWE.fnElementExist("Error Message", "//div[@class='md-toast md-toast-error']", false))
                                { clsReportResult.fnLog("User Management", "The pop up message is displayed after trying to reset user password.", "Info", true); }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "The pop up message is not displayed after trying to reset user password.", "Fail", true);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "Edit user is not found on page.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "NEWRANDOMUSER":
                            fnNavigateToUserManagement(objData.fnGetValue("ScreenMenu", ""));
                            if (clsMG.IsElementPresent("//h4[contains(text(),'Users')]"))
                            {
                                clsWE.fnScrollTo(clsWE.fnGetWe("//button[contains(@data-bind, 'addUser') and contains(text(),'Add User')]"), "Scrolling to Add user button", true, false);
                                clsWE.fnClick(clsWE.fnGetWe("//button[contains(@data-bind, 'addUser') and contains(text(),'Add User')]"), "Clicking add user button", false);
                                var userAdded = fnAddNewUser(objData);
                                if (userAdded)
                                {
                                    var performFirstLogin = bool.Parse(objData.fnGetValue("PerformFirstLogin", "true"));
                                    if (performFirstLogin)
                                    {
                                        if (fnReadEmailAndSetPassword(objData))
                                        {
                                            if (fnSecurityQuestionsAndAnswers(objData.fnGetValue("SetSecurityQuestions", "")))
                                            {
                                                clsReportResult.fnLog("User Management", "The user was activated as successfully.", "Pass", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("User Management", "The user was not activated as expected.", "Fail", true, false);
                                                blResult = false;
                                            }
                                        }
                                        else
                                        {
                                            clsReportResult.fnLog("User Management", "The email or password was not changes as expected.", "Fail", true, false);
                                            blResult = false;
                                        }
                                    }
                                }
                                else
                                {
                                    clsReportResult.fnLog("User Management", "The new usar was not created as expected", "Fail", true, false);
                                    blResult = false;
                                }
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The User Management > User Page was not loaded as expected", "Fail", true, false);
                                blResult = false;
                            }

                            break;
                        //Can be executed after new user created
                        //Validates that the set tag is still there
                        case "VALIDATE TAG":
                            fnNavigateToUserManagement(objData.fnGetValue("ScreenMenu", ""));
                            clsMG.fnCleanAndEnterText("Username", "//input[@placeholder='Username']", this.strLastUsername, false, false, "", false);
                            clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search", false);
                            var userRow = "//tr[td[text()='" + this.strLastUsername + "']]//a";
                            clsWebBrowser.objDriver.fnScrollToElement(clsWebBrowser.objDriver.FindElement(By.XPath(userRow)));
                            clsMG.fnGenericWait(
                                () => clsWE.fnElementExist(
                                        "User Record",
                                        userRow,
                                        false
                                ),
                                TimeSpan.FromSeconds(1),
                                3
                            );
                            clsWE.fnClick(clsWE.fnGetWe("//tr[td[text()='" + this.strLastUsername + "']]//a"), "Edit Record", false);
                            var tag = objData.fnGetValue("Tag", "");
                            var tagAdded = clsMG.IsElementPresent($"{UserManagementModel.strTagDropdown}/li[@title='{tag}']");
                            clsReportResult.fnLog("Selected Tag in dropdown", $"Selected Tag: {tag}", tagAdded ? "Pass" : "Fail", true);
                            break;
                        case "EXPORTUSERS":
                            {
                                fnNavigateToUserManagement(objData.fnGetValue("ScreenMenu", ""));
                                ExportUsersExcelTemplate();
                                var strPatFileAttached = ReadExportedEmailAttachments();
                                var blSuccess = false;
                                if (strPatFileAttached != "")
                                {
                                    var excel = new clsData();
                                    excel.fnLoadFile(strPatFileAttached, "Users");
                                    excel.CurrentRow = 2;
                                    while (excel.CurrentRow <= excel.RowCount)
                                    {
                                        this.ResolveUsername(objData.fnGetValue("ImportExportParameters", "").fnTextBetween("\"", "\""), objData);
                                        if (excel.fnGetValue("UserName", "") == this.strLastUsername)
                                        {
                                            blSuccess = true;
                                            break;
                                        }
                                        excel.CurrentRow++;
                                    }
                                }
                                clsReportResult.fnLog(
                                    "Verify created user was exported",
                                    $"User {this.strLastUsername} is present in the attachments of Export Users Email",
                                    blSuccess ? "Pass" : "Fail",
                                    false
                                );
                                break;
                            }
                        case "IMPORT USERS":
                            {
                                fnNavigateToUserManagement(objData.fnGetValue("ScreenMenu", ""));
                                string pathToExcel = null;
                                var attachmentFound = clsMG.fnGenericWait(
                                    condition: () =>
                                    {
                                        ExportUsersExcelTemplate();
                                        pathToExcel = ReadUsersExcelTemplateFromEmail(TimeSpan.FromMilliseconds(700), 5);
                                        return !string.IsNullOrEmpty(pathToExcel);
                                    },
                                    sleepTime: TimeSpan.Zero,
                                    attempts: 25
                                );

                                // TODO: Disable screenshot once fnLog gets fixed
                                clsReportResult.fnLog(templateFoundStep, templateFoundStep, attachmentFound ? "Pass" : "Fail", true, !attachmentFound, $"Attachment was never found");

                                string importAction = objData.fnGetValue("ImportExportParameters").Split(',').First().fnTextBetween("\"", "\"");

                                // Clear Users Sheet
                                var sheetIsClean = ClearUsersSheet(pathToExcel);
                                clsReportResult.fnLog($"{importAction} User Sheet Status", "Sheet has been cleaned", sheetIsClean ? "Info" : "Warn", false);

                                // Add new user to excel file
                                var userRowAdded = AddUserToExcelFile(pathToExcel);
                                clsReportResult.fnLog($"{importAction} User Sheet Status", $"{importAction} User row added to sheet", userRowAdded ? "Info" : "Warn", false);

                                var umPage = new UserManagementModel(clsWebBrowser.objDriver, clsMG);

                                // Upload excel File 
                                clsWebBrowser.objDriver.FindElement(umPage.objImportUsersInput).SendKeys(pathToExcel);

                                clsReportResult.fnLog("Success Toaster was showed", "Success Toaster was showed after Importing Users", umPage.fnUntilSuccessToasterVisible() ? "Pass" : "Warn", true);

                                var filename = Path.GetFileName(pathToExcel);
                                var successEmail = clsMG.fnGenericWait(
                                    condition: () => email.fnReadEmail("User Import Completed", $"{filename} has completed with a status of Success."),
                                    sleepTime: TimeSpan.FromMilliseconds(750),
                                    attempts: 25
                                );

                                clsReportResult.fnLog("Success Email was received", $"Success Email was received after Importing Users - File{filename}", successEmail ? "Pass" : "Fail", true);

                                // Search user
                                var userFound = searchUser();

                                var success = importAction != "Delete" ? userFound : !userFound;

                                // Final Assertion
                                clsReportResult.fnLog("User search", $"User {this.strLastUsername} {(importAction + "ed").Replace("ee", "e")} successfully", success ? "Pass" : "Fail", true);
                                break;
                            }
                        case "UPDATEUSERGROUPS":
                            clsMG.fnHamburgerMenu("User Management;Group Management");
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                            if (clsWE.fnElementExist("Verify Groups Management Page", UserManagementModel.strGroupManagementPage, true))
                            {
                                IList<IWebElement> cardList = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@data-bind, 'clientGroups')]//div[@class='card-body']"));

                                for (int i = 0; i < cardList.Count; i++)
                                {
                                    IWebElement groupName = cardList.ElementAt(i).FindElement(By.XPath(UserManagementModel.strCardDescription));
                                    if (groupName.GetAttribute("value") == objData.fnGetValue("GroupName", ""))
                                    {
                                        clsReportResult.fnLog("Group Management", "******Group name matched******.", "Pass", false, false);
                                        IWebElement SelectClientsButton = cardList.ElementAt(i).FindElement(By.XPath(UserManagementModel.strSelectClientButton));
                                        IWebElement SaveGroupButton = cardList.ElementAt(i).FindElement(By.XPath(UserManagementModel.strSaveGroupButton));
                                        SelectClientsButton.Click();
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent(UserManagementModel.strSelectorClientModal), TimeSpan.FromSeconds(1), 10);
                                        if (clsMG.IsElementPresent(UserManagementModel.strSelectorClientModal))
                                        {
                                            //Apply the filter
                                            clsMG.fnCleanAndEnterText("Client Number or Name", UserManagementModel.strClientNumberName, objData.fnGetValue("ClientNumber", ""), false, false, "", false);
                                            Thread.Sleep(TimeSpan.FromSeconds(2));
                                            //Check if the client exist
                                            if (clsMG.IsElementPresent("//tr[td[contains(text(), '" + objData.fnGetValue("ClientNumber", "") + "')] and td[contains(text(), '" + objData.fnGetValue("ClientName", "") + "')]]"))
                                            {
                                                clsWE.fnClick(clsWE.fnGetWe(UserManagementModel.strCheckboxElement), "Click checkbox", false, false);
                                                clsWE.fnClick(clsWE.fnGetWe(UserManagementModel.strCloseClientModal), "Click Save", true, false);
                                            }
                                            else
                                            {
                                                clsReportResult.fnLog("Select Client Popup", "The client: " + objData.fnGetValue("ClientNumber", "") + " was not found in the popup", "Fail", true, true);
                                                blResult = false;
                                            }
                                        }
                                    }
                                }
                                fnSavingClientsGroup(objData);
                            }
                            fnReadingClientsGroup(objData);
                            fnUserGroups(objData,intRow);
                            break;
                        case "READSELECTEDLOCATIONS":
                            string strTempLocAccUnit = "";
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strAvaliableRestr), TimeSpan.FromSeconds(5), 10);
                            clsWE.fnScrollTo(clsWE.fnGetWe(GroupManagementModel.strAvaliableRestr), "scrolling to available restriction section", true, false, "");
                            if (clsWE.fnElementExist("Verify Edit User Page", GroupManagementModel.strAvaliableRestr, true))
                            {
                                IWebElement SelectedLocTable = clsWebBrowser.objDriver.FindElement(By.XPath(GroupManagementModel.strSelectedLocTable));
                                ICollection<IWebElement> rows = SelectedLocTable.FindElements(By.XPath(".//tbody//tr"));

                                foreach (IWebElement row in rows)
                                {
                                    List<IWebElement> addValue = new List<IWebElement>();
                                    ICollection<IWebElement> cells = row.FindElements(By.XPath(".//td"));
                                    IWebElement restrictionType = cells.ElementAt(0);
                                    IWebElement accountNumber = cells.ElementAt(2);
                                    IWebElement unitNumber = cells.ElementAt(5);
                                    if (restrictionType.Text == "Account")
                                    {
                                        strTempLocAccUnit = strTempLocAccUnit + restrictionType.Text + ":=" + accountNumber.Text + ";";
                                        objData.fnSaveValue(clsDataDriven.strDataDriverLocation, "AccUnitSec", "AccUnitVal", intRow, strTempLocAccUnit);
                                    }
                                    if (restrictionType.Text == "Unit")
                                    {
                                        strTempLocAccUnit = strTempLocAccUnit + restrictionType.Text + ":=" + unitNumber.Text + ";";
                                        objData.fnSaveValue(clsDataDriven.strDataDriverLocation, "AccUnitSec", "AccUnitVal", intRow, strTempLocAccUnit);
                                    }
                                }
                            }
                                break;
                    }
                }
            }
            if (blResult)
            { clsReportResult.fnLog("User Management", "The User Management Function was executed successfully.", "Pass", true); }
            else
            { clsReportResult.fnLog("User Management", "The User Management Function was not executed successfully.", "Fail", true); }

            return blResult;
        }

        /// <summary>
        /// Generate or get username
        /// </summary>
        /// <param name="username">RND, RANDOM, LASTUSER</param>
        void ResolveUsername(string username, clsData objData)
        {
            string result = null;
            switch (username)
            {
                case "RND":
                case "RANDOM":
                    result = "IntAuto" + DateTime.Now.ToString("Mddyyyyhhmmss");
                    break;
                case "LASTUSERNAME":
                    if (string.IsNullOrEmpty(this.strLastUsername))
                    {
                        throw new NullReferenceException("No last username is defined, make sure you are running actions in correct order");
                    }
                    return;
                case "":
                    result = objData.fnGetValue("FirstName", "");
                    break;
                default:
                    result = username;
                    break;
            }
            this.strLastUsername = result;
        }

        public bool fnAddNewUser(clsData objData)
        {
            bool blResult = true;
            if (clsWE.fnElementExist("Add New User Screen", "//h4[contains(text(),'Add User')]", false))
            {
                clsReportResult.fnLog("Add user form", "The Add User form exist proceed to fill and save.", "Pass", true);
                this.ResolveUsername(objData.fnGetValue("Username", ""), objData);
                clsMG.fnCleanAndEnterText("Username", "//input[contains(@data-bind,'UserName')]", this.strLastUsername, bWaitHeader: false);
                clsMG.fnCleanAndEnterText("First name", "//input[contains(@data-bind,'FirstName')]", objData.fnGetValue("FirstName", ""), false, false, "", false);
                clsMG.fnCleanAndEnterText("Last name", "//input[contains(@data-bind,'LastName')]", objData.fnGetValue("LastName", ""), false, false, "", false);
                clsMG.fnCleanAndEnterText("Email", "//input[contains(@data-bind,'Email')]", objData.fnGetValue("Email", ""), false, false, "", false);
                clsMG.fnCleanAndEnterText("Phone", "//input[contains(@data-bind,'PhoneNumber')]", objData.fnGetValue("PhoneNumber", ""), false, false, "", false);
                clsMG.fnSelectDropDownWElm("Role", "//span[@data-select2-id=5]", objData.fnGetValue("Role", ""), true, false, "", false);

                //Client Restriction Section
                var clients = objData.fnGetValue("Clients", "");
                clsMG.fnSelectDropDownWElm("Clients", UserManagementModel.strClientSecurityTypes, clients, false, false, "", false);
                var clientIds = objData.fnGetValue("ClientIds", "");
                if (clients.Equals("Client") && !string.IsNullOrEmpty(clientIds))
                {
                    var newUserPage = new UserManagementModel(clsWebBrowser.objDriver, clsMG);
                    newUserPage.fnSelectClients(clientIds.Split(',').ToList());
                }

                var tag = objData.fnGetValue("Tag", "");
                if (!tag.Equals(string.Empty))
                {
                    clsReportResult.fnLog("Selecting Tag in dropdown", $"Selecting Tag: {tag}", "Info", false);
                    clsMG.fnSelectDropDownWElm(
                        "Tag Dropdown",
                        UserManagementModel.strTagDropdown,
                        tag,
                        true
                    );
                    var tagAdded = clsMG.IsElementPresent($"{UserManagementModel.strTagDropdown}/li[@title='{tag}']");
                    clsReportResult.fnLog("Selected Tag in dropdown", $"Selected Tag: {tag}", tagAdded ? "Pass" : "Fail", true);
                }

                clsWE.fnScrollTo(clsWE.fnGetWe("//input[contains(@data-bind,'PhoneNumber')]"), "Scrolling to checkbox two factor authentication", true, false);
                //MultiFactor Authentication
                var twoFA = objData.fnGetValue("2FA", "False").ToUpper();
                if (twoFA == "YES" || twoFA == "TRUE")
                { clsWE.fnClick(clsWE.fnGetWe("//label[contains(text(),'Enable Multifactor Authentication')]"), "Two Factor Autphentication", false); }
                //Line of business
                if (objData.fnGetValue("Lob", "") != "")
                {
                    clsMG.WaitWEUntilAppears("Waiting for Line of bussiness", "//input[contains(@class,'select-dropdown form-control')]", 10);
                    clsWE.fnClick(clsWE.fnGetWe("//div[select[contains(@data-bind, 'LinesOfBusiness')]]//input[@class='select-dropdown form-control']"), "Line of bussiness", false);
                    clsMG.WaitWEUntilAppears("Waiting for LOB active", "//input[contains(@class,'select-dropdown form-control active')]", 5);
                    if (clsMG.IsElementPresent("//input[contains(@class,'select-dropdown form-control active')]"))
                    {
                        //Deselect all LOBs
                        if (objData.fnGetValue("Lob", "").Contains(";"))
                        {
                            clsWE.fnDoubleClick(clsWE.fnGetWe("//label[contains(text(),'Select all')]"), "Select all", true, false);
                            string[] arrLOB = objData.fnGetValue("Lob", "").Split(';');
                            foreach (string value in arrLOB)
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//span[contains(text(),'" + value + "')]"), "Select LOB: " + value, true, false);
                            }
                        }
                        else
                        {
                            clsWE.fnDoubleClick(clsWE.fnGetWe("//label[contains(text(),'Select all')]"), "Select all", true, false);
                            clsWE.fnClick(clsWE.fnGetWe("//span[contains(text(),\"" + objData.fnGetValue("Lob", "") + "\")]"), "Select one LOB", true, false);
                        }
                    }
                    clsWE.fnClick(clsWE.fnGetWe("//*[@id='EnvironmentBar']"), "Env Bar", false, false);
                }

                //Restriction Type section
                var restrictionType = objData.fnGetValue("RestrictionType", "");
                if (restrictionType != "")
                {
                    clsMG.fnSelectDropDownWElm("Restriction Type Dropdown", UserManagementModel.strRestrictionTypeDropdown, restrictionType, true);
                    clsReportResult.fnLog("Restriction Type", "Step - Choosing Restriction Type", "Info", false);
                    var restrictionAccountNumber = objData.fnGetValue("AccountNumber", "");
                    clsMG.fnCleanAndEnterText("Search Account Number", UserManagementModel.strSearchAccountNumberInput, restrictionAccountNumber, true);
                    clsWebBrowser.objDriver.FindElement(UserManagementModel.objSeachAccountUnitButton).Click();
                    if (restrictionType.ToUpper() == "ACCOUNT")
                    {
                        var accountCheckboxSelector = UserManagementModel.objSelectRestrictionAcountByAccountNumber(restrictionAccountNumber);
                        clsWebBrowser.objDriver.fnWaitUntilElementVisible(accountCheckboxSelector);
                        var accountCheckbox = clsWebBrowser.objDriver.FindElement(accountCheckboxSelector);
                        clsWebBrowser.objDriver.fnScrollToElement(accountCheckbox);
                        accountCheckbox.Click();
                    }
                    else if (restrictionType.ToUpper() == "UNIT")
                    {
                        var restrictionUnitNumber = objData.fnGetValue("UnitNumber", "");
                        var unitCheckboxSelector = UserManagementModel.objSelectRestrictionAcountByUnitNumber(restrictionUnitNumber);
                        clsWebBrowser.objDriver.fnWaitUntilElementVisible(unitCheckboxSelector);
                        var unitCheckbox = clsWebBrowser.objDriver.FindElement(unitCheckboxSelector);
                        clsWebBrowser.objDriver.fnScrollToElement(unitCheckbox);
                        unitCheckbox.Click();
                    }
                }


                //Save Changes
                clsMG.WaitWEUntilAppears("Save button", "//button[contains(text(),'Save Changes')]", 10);
                clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Save Changes')]"), "Saving changes", false);
                //Verify is Error Messages are displayed
                if (!clsMG.IsElementPresent("//i[contains(@class,'fa fa-warning fa-exclamation-triangle red-text')]"))
                {
                    clsReportResult.fnLog("Form filled correctly", "The Form was filled successfully.", "Pass", false, false);
                }
                else
                {
                    clsReportResult.fnLog("Form not filled correctly", "The Form was not filled successfully.", "Fail", false, false);
                    blResult = false;
                }
            }
            else
            {
                clsReportResult.fnLog("Add user form not displayed", "The Add User form is not displayed.", "Fail", true);
                blResult = false;
            }
            return blResult;
        }

        public bool fnReadEmailAndSetPassword(clsData objData)
        {
            bool blResult = true;
            var readEmail = clsUtils.fnFindGeneratedEmail(objData.fnGetValue("SetCredentials", ""), "Welcome to Sedgwick Global Intake", "Please activate your account");
            var strURLReset = readEmail.fnGetContentAsString("your browser: ", "---", "your browser: ", "</span>");

            if (strURLReset != "")
            {
                clsWebBrowser.objDriver.Navigate().GoToUrl(strURLReset);
                clsWE.fnPageLoad(clsWE.fnGetWe("//h3[text()='Create Password']"), "Create password", true, false);
                while (!clsMG.IsElementPresent("//input[@id='new-pwd']")) { Thread.Sleep(TimeSpan.FromSeconds(2)); }

                clsMG.fnCleanAndEnterText("New Password", "//input[@id='new-pwd']", objData.fnGetValue("NewPass", ""), false, false, "", false);
                clsWE.fnClick(clsWE.fnGetWe("//h3[text()='Create Password']"), "", false);
                IWebElement objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//input[@id='new-pwd']"));
                Actions action = new Actions(clsWebBrowser.objDriver);
                objWebEdit.Click();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                objWebEdit.SendKeys(Keys.Tab);
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                if (clsWE.fnElementExist("Confirm Password", "//div[label[contains(text(), 'Confirm New Password')]]//input", false))
                {
                    objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//div[label[contains(text(), 'Confirm New Password')]]//input"));
                    objWebEdit.SendKeys(objData.fnGetValue("ConfPass", ""));
                    clsWE.fnClick(clsWE.fnGetWe("//button[text()='Submit']"), "Submit", true);
                    if (clsMG.IsElementPresent("//i[contains(@class,'fa fa-warning fa-exclamation-triangle red-text')]"))
                    {
                        clsReportResult.fnLog("Set Password Form not filled correctly", "The Set Password Form was not filled successfully.", "Fail", false, false);
                        blResult = false;
                    }
                    else
                    {
                        clsReportResult.fnLog("Form filled correctly", "The Set Password Form was filled successfully.", "Pass", false, false);
                    }
                }
                else
                {
                    clsReportResult.fnLog("Confirm password field", "Confirm password field is not displayed on screen", "Fail", true, false);
                    blResult = false;
                }
            }
            else
            {
                clsReportResult.fnLog("Activate Account", "The activate account email was not found successfully and test cannot continue.", "Fail", false, false);
                blResult = false;
            }
            return blResult;
        }


        public bool fnSecurityQuestionsAndAnswers(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Security Question", "<<<<<<<<<< The Set Security Question Function Starts. >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "SecQuestions");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    //QUESTION 1
                    clsWE.fnPageLoad(clsWE.fnGetWe("//h3[text()='Security Questions']"), "Security Questions", false, false);
                    clsMG.fnSelectCustomDropDown("Question1", "(//input[contains(@class,'select-dropdown form-control')])[1]", objData.fnGetValue("Question1", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer1", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[1]", objData.fnGetValue("Value1", ""), false, false, "", false);
                    //QUESTION 2
                    clsMG.fnSelectCustomDropDown("Question2", "(//input[contains(@class,'select-dropdown form-control')])[2]", objData.fnGetValue("Question2", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer2", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[2]", objData.fnGetValue("Value2", ""), false, false, "", false);
                    //QUESTION 3
                    clsMG.fnSelectCustomDropDown("Question2", "(//input[contains(@class,'select-dropdown form-control')])[3]", objData.fnGetValue("Question3", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer3", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[3]", objData.fnGetValue("Value3", ""), false, false, "", false);
                    //QUESTION 4
                    clsMG.fnSelectCustomDropDown("Question4", "(//input[contains(@class,'select-dropdown form-control')])[4]", objData.fnGetValue("Question4", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer4", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[4]", objData.fnGetValue("Value4", ""), false, false, "", false);
                    //QUESTION 5
                    clsMG.fnSelectCustomDropDown("Question5", "(//input[contains(@class,'select-dropdown form-control')])[5]", objData.fnGetValue("Question5", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer5", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[5]", objData.fnGetValue("Value5", ""), false, false, "", false);

                    clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Submit')]"), "Submit button", false);
                    if (clsMG.IsElementPresent("//i[contains(@class,'fa fa-warning fa-exclamation-triangle red-text')]"))
                    {
                        clsReportResult.fnLog("Questions Form not filled correctly", "The Questions Form was not filled successfully.", "Fail", false, false);
                        blResult = false;
                    }
                    else
                    {
                        clsReportResult.fnLog("Form filled correctly", "The Questions Form was filled successfully.", "Pass", false, false);
                    }
                }
            }
            return blResult;
        }



        public bool fnSecurityQuestionsAndAnswers_(clsData objData)
        {
            bool blResult = true;
            Thread.Sleep(TimeSpan.FromSeconds(5));
            //QUESTION 1
            clsWE.fnPageLoad(clsWE.fnGetWe("//h3[text()='Security Questions']"), "Security Questions", false, false);
            clsMG.fnSelectDropDownWElm("Question1", "(//input[contains(@class,'select-dropdown form-control')])[1]", objData.fnGetValue("Question1", ""), false, false, "", false);
            clsMG.fnCleanAndEnterText("Answer1", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[1]", "Test1", true, false);
            //QUESTION 2
            clsMG.fnSelectDropDownWElm("Question2", "(//input[contains(@class,'select-dropdown form-control')])[2]", objData.fnGetValue("Question2", ""), false, false, "", false);
            clsMG.fnCleanAndEnterText("Answer2", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[2]", "Test2", true, false);
            //QUESTION 3
            clsMG.fnSelectDropDownWElm("Question2", "(//input[contains(@class,'select-dropdown form-control')])[3]", objData.fnGetValue("Question3", ""), false, false, "", false);
            clsMG.fnCleanAndEnterText("Answer3", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[3]", "Test3", true, false);
            //QUESTION 4
            clsMG.fnSelectDropDownWElm("Question4", "(//input[contains(@class,'select-dropdown form-control')])[4]", objData.fnGetValue("Question4", ""), false, false, "", false);
            clsMG.fnCleanAndEnterText("Answer4", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[4]", "Test4", true, false);
            //QUESTION 5
            clsMG.fnSelectDropDownWElm("Question5", "(//input[contains(@class,'select-dropdown form-control')])[5]", objData.fnGetValue("Question5", ""), false, false, "", false);
            clsMG.fnCleanAndEnterText("Answer5", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[5]", "Test5", false, false);

            clsWE.fnClick(clsWE.fnGetWe("//button[contains(text(),'Submit')]"), "Submit button", false);
            if (clsMG.IsElementPresent("//i[contains(@class,'fa fa-warning fa-exclamation-triangle red-text')]"))
            {

                clsReportResult.fnLog("Questions Form not filled correctly", "The Questions Form was not filled successfully.", "Fail", false, false);
                blResult = false;
            }
            else
            {
                clsReportResult.fnLog("Form filled correctly", "The Questions Form was filled successfully.", "Pass", false, false);
            }
            return blResult;
        }

        private void fnNavigateToUserManagement(string pstrScreenMenu)
        {
            if (clsWebBrowser.objDriver.Url.EndsWith("/UserManagement"))
            {
                clsWebBrowser.objDriver.Navigate().Refresh();
                return;
            }
            clsMG.fnHamburgerMenu($"User Management;{pstrScreenMenu}");
            switch (pstrScreenMenu)
            {
                case "Web Users":
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//h4[contains(text(),'Users')]"), TimeSpan.FromSeconds(1), 10);
                    clsWE.fnPageLoad(clsWE.fnGetWe("//h4[contains(text(),'Users')]"), "Users", true, false);
                    break;
                case "Shared Logins":
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//*[contains(text(),'Shared Logins')]"), TimeSpan.FromSeconds(1), 10);
                    clsWE.fnPageLoad(clsWE.fnGetWe("//*[contains(text(),'Shared Logins')]"), "Shared Logins", true, false);
                    break;
                case "Group Management":
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//*[contains(text(),'Client Groups')]"), TimeSpan.FromSeconds(1), 10);
                    clsWE.fnPageLoad(clsWE.fnGetWe("//*[contains(text(),'Client Groups')]"), "Client Groups", true, false);
                    break;
            }

        }




        public bool fnUserGroups(clsData objData,int intRow)
        {
            bool blResult = true;
                    clsMG.fnHamburgerMenu("User Management;Group Management");
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(UserManagementModel.strGroupManagementPage), TimeSpan.FromSeconds(5), 10);
                    //clsMG.fnGoTopPage();
                    if (clsWE.fnElementExist("Verify Groups Management Page", UserManagementModel.strGroupManagementPage, true))
                    {
                        var strTemp = fnReadingClientsGroup(objData);
                        objData.fnSaveValue(clsDataDriven.strDataDriverLocation,"UserMGMT", "ClientsFromGM", intRow, strTemp);
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
            return blResult;
        }

        public string fnReadingClientsGroup(clsData objData)
        {
            IList<IWebElement> cardList = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@data-bind, 'clientGroups')]//div[@class='card-body']"));
            string strTempClient = "";
            for (int i = 0; i < cardList.Count; i++)
            {
                IWebElement groupName = cardList.ElementAt(i).FindElement(By.XPath(".//input[contains(@data-bind, 'data.Description')]"));
                if (groupName.GetAttribute("value") == objData.fnGetValue("GroupName", ""))
                {
                    clsReportResult.fnLog("Group Management", "******Group name matched******.", "Pass", false, false);
                    //clsWE.fnScrollTo(groupName,"Scrolling to Group Name element");
                    IList<IWebElement> clientNumber = cardList.ElementAt(i).FindElements(By.XPath(".//div[contains(@data-bind,'ClientNumber')]"));
                    IList<IWebElement> clientName = cardList.ElementAt(i).FindElements(By.XPath(".//div[contains(@data-bind,'Name')]"));
                    for (int j = 0; j < clientNumber.Count; j++)
                    {
                        strTempClient = strTempClient + clientName.ElementAt(j).Text + "|" + clientNumber.ElementAt(j).Text + ";";
                    }
                }
                else
                {
                    clsReportResult.fnLog("Group Management", "******Group name not matched******.", "Info", false);
                }
            }
            return strTempClient;

        }

        public string fnGettingClientsFromPopup(clsData objData)
        {
            bool blResult = true;
            string strTemp = "";
            clsMG.fnHamburgerMenu("New Intake");
            if (clsWE.fnElementExist("Select Intake", "//h4[text()='Select Intake']", true))
            {
                clsWE.fnClick(clsWE.fnGetWe("//button[@id='selectClient_']"), "Select Client Button", false);
                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]"), "Select Client Popup", false, false);
                if (clsWE.fnElementExist("Select Client Popup", "//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]", true, false))
                {

                    IWebElement clientsTable = clsWebBrowser.objDriver.FindElement(By.XPath("//div[contains(@id,'clientSelectorModal_')]//table[contains(@id,'clientSelectorTable_')]"));
                    ICollection<IWebElement> rows = clientsTable.FindElements(By.XPath(".//tbody//tr"));

                    foreach (IWebElement row in rows)
                    {
                        List<IWebElement> addValue = new List<IWebElement>();
                        ICollection<IWebElement> cells = row.FindElements(By.XPath(".//td"));
                        IWebElement clientName = cells.ElementAt(1);
                        IWebElement clientNumber = cells.ElementAt(2);
                        strTemp = strTemp + clientName.Text + " " + clientNumber.Text + ";";
                    }
                    blResult = true;
                }
                else
                {
                    clsReportResult.fnLog("Select Client Popup", "The select intake popup was not displayed", "Fail", true, true);
                    blResult = false;
                }
            }
            return strTemp;
        }

        public bool fnReadingSelectedLocations(string pstrSetNo)
        {
            bool blResult = true;
            string strTempLoc = "";
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< Reading selected location funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "AccUnitSec");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(UserManagementModel.strAvaliableRestr), TimeSpan.FromSeconds(5), 10);
                    clsWE.fnScrollTo(clsWE.fnGetWe(UserManagementModel.strAvaliableRestr), "scrolling to available restriction section", true, false, "");
                    if (clsWE.fnElementExist("Verify Edit User Page", UserManagementModel.strAvaliableRestr, true))
                    {
                        IWebElement SelectedLocTable = clsWebBrowser.objDriver.FindElement(By.XPath(UserManagementModel.strSelectedLocTable));
                        ICollection<IWebElement> rows = SelectedLocTable.FindElements(By.XPath(".//tbody//tr"));

                        //ICollection<IWebElement> restrictionType = SelectedLocTable.FindElements(By.XPath(".//td[contains(@data-bind,'RestrictionType')]"));
                        //foreach(IWebElement cell in restrictionType)
                        //{
                        //    System.Console.WriteLine(cell.Text);
                        //}
                        foreach (IWebElement row in rows)
                        {
                            List<IWebElement> addValue = new List<IWebElement>();
                            ICollection<IWebElement> cells = SelectedLocTable.FindElements(By.XPath(".//td"));
                            IWebElement restrictionType = cells.ElementAt(0);
                            IWebElement accountNumber = cells.ElementAt(2);
                            IWebElement unitNumber = cells.ElementAt(5);
                            if (restrictionType.Text == "Account")
                            {
                                strTempLoc = strTempLoc + restrictionType.Text + ":=" + accountNumber.Text + ";";
                                objData.fnSaveValue(clsDataDriven.strDataDriverLocation, "AccUnitSec", "AccUnitVal", intRow, strTempLoc);
                            }
                            if (restrictionType.Text == "Unit")
                            {
                                strTempLoc = strTempLoc + restrictionType.Text + ":=" + unitNumber.Text + ";";
                                objData.fnSaveValue(clsDataDriven.strDataDriverLocation, "AccUnitSec", "AccUnitVal", intRow, strTempLoc);
                            }
                        }

                    }
                }
            }
            return blResult;
        }

        public bool fnSavingClientsGroup(clsData objData)
        {
            bool blResult = true;
            
                Thread.Sleep(TimeSpan.FromSeconds(10));
                if (clsWE.fnElementExist("Verify Groups Management Page", UserManagementModel.strGroupManagementPage, true))
                {
                    IList<IWebElement> cardList = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@data-bind, 'clientGroups')]//div[@class='card-body']"));
                    for (int i = 0; i < cardList.Count; i++)
                    {
                        IWebElement groupName = cardList.ElementAt(i).FindElement(By.XPath(".//input[contains(@data-bind, 'data.Description')]"));
                        if (groupName.GetAttribute("value") == objData.fnGetValue("GroupName", ""))
                        {
                            clsReportResult.fnLog("Group Management", "******Group name matched to save******.", "Pass", false, false);
                            IWebElement SaveGroupButton = cardList.ElementAt(i).FindElement(By.XPath(".//button[contains(@data-bind,'saveClientGroup')]"));
                            SaveGroupButton.Click();
                        }
                    }
                }
            
            return blResult;
        }

        public bool fnverifyClientsPopup(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< Verify Clients Popup validation funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "UserMGMT");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    var strTemp = fnGettingClientsFromPopup(objData);
                    objData.fnSaveValue(clsDataDriven.strDataDriverLocation, "UserMGMT", "ClientsFromPopup", intRow, strTemp);
                    int count = 0;
                    string[] arrClients = objData.fnGetValue("ClientsFromGM", "").Split(';');
                    foreach (var client in arrClients)
                    {
                        if (client != "")
                        {
                            var locators = client.Split('|');
                            if (clsWE.fnElementExist("Locators", "//table[@id='clientSelectorTable_']//tr[td[text()='" + locators[0] + "'] and td[text()='" + locators[1] + "']]", false, false, ""))
                            {
                                clsReportResult.fnLog("Group Management Clients", "The clients displayed in screen matched as expected.", "Pass", false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Group Management Clients", "The clients displayed in screen does not matched as expected.", "Fail", false, true);
                            }
                            count++;
                        }

                    }
                }
            }
            return blResult;
        }
    }
}
