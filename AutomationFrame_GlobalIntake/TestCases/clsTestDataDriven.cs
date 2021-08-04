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
            fnOpenBrowser(clsDataDriven.strBrowser);
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


            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Sheet"]);
            fnNavigateToUrl(clsMG.fnGetURLEnv(clsDataDriven.strReportEnv));

            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Run", "") == "1")
                {
                    blStatus = true;
                    string[] arrFunctions = objData.fnGetValue("Funcions").Split(';');
                    string[] arrValue = objData.fnGetValue("Values").Split(';');
                    int intCount = -1;
                    foreach (string item in arrFunctions)
                    {
                        intCount = intCount + 1;
                        var TempValue = "";
                        if (arrValue[intCount]!= "") { TempValue = arrValue[intCount].Split('=')[1]; }
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
