using AutomationFrame_GlobalIntake.POM;
using AutomationFramework;
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
    class clsTestDD : clsWebBrowser
    {
        public bool blStop;
        public bool blStatus;
        public string dtStartTime;
        public string dtEndTime;
        clsData objData = new clsData();
        clsLogin clsLG = new clsLogin();
        clsAPI clsAPI = new clsAPI();
        clsSearch clsSearch = new clsSearch();
        clsMegaIntake clsMG = new clsMegaIntake();
        clsIntakeFlow clsIntake = new clsIntakeFlow();
        clsUserManagment clsUM = new clsUserManagment();

        [OneTimeSetUp]
        public void BeforeClass()
        {
            blStop = clsReportResult.fnExtentSetup();
            if (!blStop)
                AfterClass();
        }

        public void SetupTest(string pstrTestCase)
        {
            clsReportResult.objTest = clsReportResult.objExtent.CreateTest(pstrTestCase);
            fnOpenBrowser(clsDataDriven.strBrowser);
        }

        [Test]
        public void fnTest_DataDriven()
        {
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Sheet"]);
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Run", "") == "1")
                {
                    //Setup Report, Execution Functions
                    blStatus = true;
                    dtStartTime = DateTime.Now.ToString("MMddyyyy_hhmmss");
                    string[] arrFunctions = objData.fnGetValue("Funcions").Split(';');
                    string[] arrValue = objData.fnGetValue("Values").Split(';');
                    int intCount = -1;
                    SetupTest(objData.fnGetValue("Description", ""));
                    fnNavigateToUrl(clsMG.fnGetURLEnv(clsDataDriven.strReportEnv));

                    //Iterate and select function
                    foreach (string item in arrFunctions) 
                    {
                        intCount = intCount + 1;
                        var TempValue = "";
                        //if (intCount < arrValue.Length)
                        if (intCount < arrValue.Length && blStatus)
                            { if (arrValue[intCount] != "") { TempValue = arrValue[intCount].Split('=')[1]; } }
                        switch (item.ToUpper()) 
                        {
                            case "LOGIN":
                                if (!clsLG.fnLoginData(TempValue)) { blStatus = false; }
                                break;
                            case "2FALOGIN":
                                if (!clsLG.fnTwoFactorsVerification(TempValue)) { blStatus = false; }
                                break;
                            case "FORGOTPASSWORD":
                                if (!clsLG.fnForgotPasswordVerification(TempValue)) { blStatus = false; }
                                break;
                            case "FORGOTUSERNAME":
                                if (!clsLG.fnForgotUsernameVerification(TempValue)) { blStatus = false; }
                                break;
                            case "EXPIREDUSERRESTRICTION":
                                if (!clsLG.fnExpiredUserRestriction(TempValue)) { blStatus = false; }
                                break;
                            case "TIMEOUTSESSION":
                                if (!clsLG.fnTimeoutSessionVerification(TempValue)) { blStatus = false; }
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
                                if (!clsSearch.fnVerifyTrainingModeClaims()) { blStatus = false; }
                                break;
                            case "POLICYLOOKUP":
                                if (!clsIntake.fnPolicyLookupVerification(TempValue)) { blStatus = false; }
                                break;
                            case "INTAKEONLYRESUME":
                                //if (!clsIntake.fnIntakeOnlyResumeVerification(TempValue)) { blStatus = false; }
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
                            case "RESTRICTEDLOBBYUSER":
                                    if (!clsIntake.fnUserWithLOBRestriction(TempValue)) { blStatus = false; }
                                break;
                            default:
                                clsReportResult.fnLog("Data Driven Test", "The action: does not exsit.", "Fail", false);
                                blStatus = false;
                                break;
                        }
                    }
                    //Check Status
                    if (blStatus)
                    { objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "TestCases", "Status", intRow, "Pass"); }
                    else
                    { objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "TestCases", "Status", intRow, "Fail"); }
                    CloseTest();
                }
            }
            
        }

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
                Console.WriteLine(objException.Message);
            }
        }

    }
}
