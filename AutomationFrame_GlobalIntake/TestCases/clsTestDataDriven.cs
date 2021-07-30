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
        public void SetupTest()
        {
            clsReportResult.objTest = clsReportResult.objExtent.CreateTest(TestContext.CurrentContext.Test.Name);
            fnOpenBrowser(ConfigurationManager.AppSettings["Browser"]);
        }

        [Test]
        public void fnTest_DataDriven()
        {
            bool blStatus = true;
            clsMegaIntake clsMG = new clsMegaIntake();
            clsLogin clsLogin = new clsLogin();
            clsData objData = new clsData();
            clsUserManagment clsUM = new clsUserManagment();
            clsIntakeFlow clsIntake = new clsIntakeFlow();


            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Sheet"]);
            fnNavigateToUrl(clsMG.fnGetURLEnv((objData.fnGetValue("Environment", ""))));

            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Run", "") == "1")
                {
                    string[] arrFunctions = objData.fnGetValue("Funcions").Split(';');
                    string[] arrValue = objData.fnGetValue("Values").Split(';');
                    int intCount = -1;
                    foreach (string item in arrFunctions)
                    {
                        intCount = intCount + 1;
                        var TempValue = arrValue[intCount].Split('=')[1];
                        switch (item.ToUpper())
                        {
                            case "LOGIN":
                                blStatus = clsLogin.fnLoginData(TempValue);
                                break;
                            case "2FALOGIN":
                                blStatus = clsLogin.fnTwoFactorsVerification(TempValue);
                                break;
                            case "FORGOTPASSWORD":
                                blStatus = clsLogin.fnForgotPasswordVerification2(TempValue);
                                break;
                            case "FORGOTUSERNAME":
                                blStatus = clsLogin.fnForgotUsernameVerification(TempValue);
                                break;
                            case "EXPIREDUSERRESTRICTION":
                                blStatus = clsLogin.fnExpiredUserRestriction(TempValue);
                                break;
                            case "TIMEOUTSESSION":
                                blStatus = clsLogin.fnTimeoutSessionVerification(TempValue);
                                break;
                            case "USERMANAGEMENT":
                                blStatus = clsUM.fnUserMagmtWebUser(TempValue);
                                break;
                            case "ACCOUNTUNITSECURITY":
                                blStatus = clsIntake.fnAccountUnitSecurityVerification(TempValue);
                                break;
                            default:
                                break;
                        }

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
