using AutomationFramework;
using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Mail;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace AutomationFrame_GlobalIntake.TestCases
{
    [TestFixture]
    class clsTestDataDriven : clsWebBrowser
    {

        public bool blStop;
        public clsReportResult clsRR = new clsReportResult();
        public clsWebElements clsWE = new clsWebElements();


        [OneTimeSetUp]
        public void BeforeClass()
        {
            blStop = clsReportResult.fnExtentSetup();
            if (!blStop)
                AfterClass();
        }

               
        [SetUp]
        public void SetupTest(string pstrTestCase)
        {
            clsReportResult.objTest = clsReportResult.objExtent.CreateTest(pstrTestCase);
            fnOpenBrowser("Chrome");
            //Console.WriteLine("");

        }




        [Test]
        public void test()
        {
            clsData objData = new clsData();
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "SearchIntake");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == "12")
                {
                    string strTemp = objData.fnGetValue("StartDate", "");

                }
            }


            string strURL = "https://intake-uat.sedgwick.com/Account/NewUser?userName=IntakeAuto08162021_0&code=CfDJ8PZO%2B6QMFUlBtIJA6TJ7NdUq6VGEJICUwPrRN3Y6g5rVJhPqs06ADlI1ezCiZHDkqqFb8UcxWiIBdDbKIwHyl2tfpB%2B2mFAfPIup8LYB3FG4WGN9nJALN8CtATzw5wtaLAoXcnhOubo5v31XByd5RS05TXcd%2B3x8IyF27lswFMScUIsUehG7WKpkUIAv5Q3PXP1bxTQYILnfMDtpLpQEoYum7PO1MA0hbL%2B4bei87RhA&confirmEmail=CfDJ8PZO%2B6QMFUlBtIJA6TJ7NdVN0AW6ILGvWyWHaW%2Br%2F0R%2BBg4SSHC15AJQunUWRTbm5Uwh%2BDpElDUkk65e%2FdFV2UrUHsyZbwc4OCweVNm%2BA9KK8mimqsGl1Wm4GugRyAjEE9GdIhnxH7gA05eSaScUYBBmhoOX4x4tUaoZOFuNFSfN2nMTJsMzVfa%2FiwkjbwZwnzodYfjHbkm512aRA%2BfhbkFfIl0PTn8FbE5gYLIiaBvrVN2YWQOX%2BEH6ojOFZhDeyQ%3D%3D";
            clsWebBrowser.objDriver.Navigate().GoToUrl(strURL);
            
            clsMegaIntake clsMG = new clsMegaIntake();
            clsMG.fnCleanAndEnterText("New Password", "//input[@id='new-pwd']", "DSFSDFDS", false, false, "", false);
            clsWE.fnClick(clsWE.fnGetWe("//h3[text()='Create Password']"), "", false);

            IWebElement objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//input[@id='new-pwd']"));
            Actions action = new Actions(clsWebBrowser.objDriver);
            objWebEdit.Click();
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            objWebEdit.SendKeys(Keys.Tab);
            objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//div[label[contains(text(), 'Confirm New Password')]]//input"));
            objWebEdit.SendKeys("sdfsdfdsfsdf");
            string x;
            /*
            Imap4Client client = new Imap4Client();
            client.ConnectSsl("imap.gmail.com", 993);
            client.Login("intakemanagement1@gmail.com", "P@ssw0rd!123");
            Mailbox mails = client.SelectMailbox("Inbox");
            MessageCollection message = mails.SearchParse("abc");
            */

        }


        [Test]
        public void fnTest_DataDriven()
        {
            bool blStatus;
            clsMegaIntake clsMG = new clsMegaIntake();
            clsLogin clsLogin = new clsLogin();
            clsData objData = new clsData();
            clsUserManagment clsUM = new clsUserManagment();
            clsIntakeFlow clsIntake = new clsIntakeFlow();
            clsAPI clsAPI = new clsAPI();
            clsSearch clsSearch = new clsSearch();

            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Sheet"]);
            fnNavigateToUrl(clsMG.fnGetURLEnv(clsDataDriven.strReportEnv));

            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Run", "") == "1")
                {
                    SetupTest(objData.fnGetValue("Description", ""));

                    blStatus = true;
                    string[] arrFunctions = objData.fnGetValue("Funcions").Split(';');
                    string[] arrValue = objData.fnGetValue("Values").Split(';');
                    int intCount = -1;
                    foreach (string item in arrFunctions)
                    {
                        intCount = intCount + 1;
                        var TempValue = "";
                        if (intCount < arrValue.Length) 
                        {
                            if (arrValue[intCount] != "") { TempValue = arrValue[intCount].Split('=')[1]; }
                        }
                        /*
                        switch (item.ToUpper())
                        {
                            case "LOGIN":
                                if (!clsLogin.fnLoginData(TempValue)) { blStatus = false; }
                                break;
                            case "2FALOGIN":
                                if (!clsLogin.fnTwoFactorsVerification(TempValue)) { blStatus = false; }
                                break;
                            case "FORGOTPASSWORD":
                                if (!clsLogin.fnForgotPasswordVerification(TempValue)) { blStatus = false; }
                                break;
                            case "FORGOTUSERNAME":
                                if (!clsLogin.fnForgotUsernameVerification(TempValue)) { blStatus = false; }
                                break;
                            case "EXPIREDUSERRESTRICTION":
                                if (!clsLogin.fnExpiredUserRestriction(TempValue)) { blStatus = false; }
                                break;
                            case "TIMEOUTSESSION":
                                if (!clsLogin.fnTimeoutSessionVerification(TempValue)) { blStatus = false; }
                                break;
                            case "USERMANAGEMENT":
                                if (!clsUM.fnUserMagmtWebUser(TempValue)) { blStatus = false; }
                                break;
                            case "ACCOUNTUNITSECURITY":
                                if (!clsIntake.fnAccountUnitSecurityVerification(TempValue)) { blStatus = false; }
                                break;
                            case "CREATECLAIM":
                                if (!clsIntake.fnCreateAndSubmitClaim(TempValue)) { blStatus = false; }
                                break;
                            case "SEARCHTRAININGCLAIM":
                                if (!clsIntake.fnVerifyTrainingModeClaims()) { blStatus = false; }
                                break;
                            case "POLICYLOOKUP":
                                if (!clsIntake.fnPolicyLookupVerification(TempValue)) { blStatus = false; }
                                break;
                            case "INTAKEONLYRESUME":
                                if (!clsIntake.fnIntakeOnlyResumeVerification(TempValue)) { blStatus = false; }
                                break;
                            case "REPORTEDBYRESTRICTION":
                                if (!clsIntake.fnReportedByVerification(TempValue)) { blStatus = false; }
                                break;
                            case "INTAKEINSTANCEAPI":
                                if (!clsAPI.fnIntakeInstanceAPI(TempValue)) { blStatus = false; }
                                break;
                            case "INTAKEONLYDASHBOARD":
                                if (!clsIntake.fnIntakeOnlyDashboardResumeAPI()) { blStatus = false; }
                                break;
                            case "IMAGEBLOCKANNONYMOUSACCESS":
                                if (!clsIntake.fnImagesBlockedAnnonymousAccess()) { blStatus = false; }
                                break;
                            case "IMAGEBLOCKANNONYMOUSACCESSLOGIN":
                                if (!clsIntake.fnImagesBlockedAnnonymousAccessLogin()) { blStatus = false; }
                                break;
                            case "SEARCHRESULTS":
                                if (!clsSearch.fnSearchResults(TempValue)) { blStatus = false; }
                                break;
                            default:
                                clsReportResult.fnLog("Data Driven Test", "The action: does not exsit.", "Fail", false);
                                blStatus = false;
                                break;
                        }
                        */
                    }
                    //Save Execution Status
                    if (blStatus)
                    { objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "TestCases", "Status", intRow, "Pass"); }
                    else
                    { objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "TestCases", "Status", intRow, "Fail"); }
                }
            }
        }



        [TearDown]
        public void CloseTest()
        {
            fnCloseBrowser();
            clsReportResult.fnExtentClose();
        }

        [OneTimeTearDown]
        public void AfterClass()
        {
            try
            {
                clsReportResult.objExtent.Flush();

            }
            catch (Exception objException)
            {
                Console.WriteLine("Error: {0}", objException.Message);
            }
        }


    }
}
