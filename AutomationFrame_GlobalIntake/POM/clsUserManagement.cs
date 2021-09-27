using AutomationFrame_GlobalIntake.Models;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
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
        private clsLogin clsLN = new clsLogin();
        private string strCreatedUsername;

        public bool fnUserMagmtWebUser(string pstrSetNo)
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
                    switch (objData.fnGetValue("Action", "").ToUpper())
                    {
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
                            clsMG.fnHamburgerMenu("User Management;Web Users");
                            clsMG.fnCleanAndEnterText("Username", "//input[@placeholder='Username']", objData.fnGetValue("Username", ""), false, false, "", false);
                            clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search", false);
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            //if (clsWE.fnElementExist("User Record", "//tr[td[contains(text(), '" + objData.fnGetValue("Username", "") + "')]]//a", false))
                            if (clsWE.fnElementExist("User Record", "//tr[td[text()='" + objData.fnGetValue("Username", "") + "']]//a", false))
                            {
                                clsWE.fnClick(clsWE.fnGetWe("//tr[td[text()='" + objData.fnGetValue("Username", "") + "']]//a"), "Edit Record", false);
                                clsWE.fnPageLoad(clsWE.fnGetWe("//h4[text()='Edit User']"), "Edit User", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("User Management", "The search did not return any record with the criteria provided.", "Fail", true);
                                blResult = false;
                            }
                            break;
                        case "ENABLE":
                            var x = clsUtils.fnIsElementEnabledVisible(By.XPath("//button[contains(@data-bind, 'unlockUser')]"), clsWebBrowser.objDriver);

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
                            clsMG.fnHamburgerMenu("User Management;Web Users");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//h4[contains(text(),'Users')]"), "Users", true, false);
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
                            clsMG.fnHamburgerMenu("User Management;Web Users");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//h4[contains(text(),'Users')]"), "Users", true, false);
                            clsMG.fnCleanAndEnterText("Username", "//input[@placeholder='Username']", this.strCreatedUsername, false, false, "", false);
                            clsWE.fnClick(clsWE.fnGetWe("//button[text()='Search']"), "Search", false);
                            var userRow = "//tr[td[text()='" + this.strCreatedUsername + "']]//a";
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
                            clsWE.fnClick(clsWE.fnGetWe("//tr[td[text()='" + this.strCreatedUsername + "']]//a"), "Edit Record", false);
                            var tag = objData.fnGetValue("Tag", "");
                            var tagAdded = clsMG.IsElementPresent($"{UserManagementModel.strTagDropdown}/li[@title='{tag}']");
                            clsReportResult.fnLog("Selected Tag in dropdown", $"Selected Tag: {tag}", tagAdded ? "Pass" : "Fail", true);
                            break;
                    }
                }
            }
            if (blResult)
            { clsReportResult.fnLog("User Management", "The User Management Function was executed successfully.", "Pass", true); }
            else
            { clsReportResult.fnLog("User Management", "The User Management Function was not executed successfully.", "Fail", true); }
            //clsLN.fnLogOffSession();

            return blResult;
        }



        public bool fnAddNewUser(clsData objData)
        {
            bool blResult = true;
            if (clsWE.fnElementExist("Add New User Screen", "//h4[contains(text(),'Add User')]", false))
            {
                clsReportResult.fnLog("Add user form", "The Add User form exist proceed to fill and save.", "Pass", true);
                if (objData.fnGetValue("Username", "").ToUpper() == "RND" || objData.fnGetValue("Username", "").ToUpper() == "RANDOM")
                {
                    this.strCreatedUsername = "IntAuto" + DateTime.Now.ToString("Mddyyyyhhmmss");
                    clsMG.fnCleanAndEnterText("Username", "//input[contains(@data-bind,'UserName')]", strCreatedUsername, false, false);
                }
                else
                { clsMG.fnCleanAndEnterText("Username", "//input[contains(@data-bind,'UserName')]", objData.fnGetValue("FirstName", ""), false, false, "", false); }
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
                if(twoFA == "YES" || twoFA == "TRUE") 
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
            clsEmail clsEmail = new clsEmail();
            string strURLReset = clsEmail.fnReadConfirmationEmail(objData.fnGetValue("SetCredentials", ""), "Please activate your account", "your browser: ", "---", "your browser: ", "</span>");
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
                    clsMG.fnSelectDropDownWElm("Question1", "(//input[contains(@class,'select-dropdown form-control')])[1]", objData.fnGetValue("Question1", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer1", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[1]", objData.fnGetValue("Value1", ""), false, false, "", false);
                    //QUESTION 2
                    clsMG.fnSelectDropDownWElm("Question2", "(//input[contains(@class,'select-dropdown form-control')])[2]", objData.fnGetValue("Question2", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer2", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[2]", objData.fnGetValue("Value2", ""), false, false, "", false);
                    //QUESTION 3
                    clsMG.fnSelectDropDownWElm("Question2", "(//input[contains(@class,'select-dropdown form-control')])[3]", objData.fnGetValue("Question3", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer3", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[3]", objData.fnGetValue("Value3", ""), false, false, "", false);
                    //QUESTION 4
                    clsMG.fnSelectDropDownWElm("Question4", "(//input[contains(@class,'select-dropdown form-control')])[4]", objData.fnGetValue("Question4", ""), false, false, "", false);
                    clsMG.fnCleanAndEnterText("Answer4", "(//div[@class='md-form pb-2']//div[@class='col-10']//input[@placeholder='Answer *'])[4]", objData.fnGetValue("Value4", ""), false, false, "", false);
                    //QUESTION 5
                    clsMG.fnSelectDropDownWElm("Question5", "(//input[contains(@class,'select-dropdown form-control')])[5]", objData.fnGetValue("Question5", ""), false, false, "", false);
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



    }
}
