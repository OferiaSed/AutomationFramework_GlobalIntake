using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.POM
{
    public class clsAPI
    {
        clsMegaIntake clsMG = new clsMegaIntake();
        
        private IntakeResponse fnAPISubmitClaim(string pstrApiUser, string pstrApiPassword, string strJsonPath) 
        {
            IntakeResponse claim;
            if (File.Exists(@strJsonPath))
            {
                string strEndPoint = "api/v1/Intake/SubmitIntakeInstance";
                string strBaseURL = clsMG.fnGetURLEnv(clsDataDriven.strReportEnv);
                var json = File.ReadAllText(@strJsonPath);
                clsRest rest = new clsRest(strBaseURL);
                rest.AddHeader("Authorization", "Bearer " + fnGetTokenAccess(pstrApiUser, pstrApiPassword));
                HTTP_RESPONSE resp = rest.POST(strEndPoint, json.ToString());
                claim = JsonConvert.DeserializeObject<IntakeResponse>(resp.MessageBody);
            }
            else 
            {
                clsReportResult.fnLog("API Submit Intake Instance", "The Json/txt File was not found in the path: " + strJsonPath, "Fail", false, false);
                claim = null;
            }

            return claim;
        }

        private IntakeResponse fnAPICreateInstance(string pstrApiUser, string pstrApiPassword, string strJsonPath)
        {
            IntakeResponse claim;
            if (File.Exists(@strJsonPath)) 
            {
                string strEndPoint = "api/v1/Intake/CreateIntakeInstance";
                string strBaseURL = clsMG.fnGetURLEnv(clsDataDriven.strReportEnv);
                var json = File.ReadAllText(@strJsonPath);
                clsRest rest = new clsRest(strBaseURL);
                rest.AddHeader("Authorization", "Bearer " + fnGetTokenAccess(pstrApiUser, pstrApiPassword));
                HTTP_RESPONSE resp = rest.POST(strEndPoint, json.ToString());
                claim = JsonConvert.DeserializeObject<IntakeResponse>(resp.MessageBody);
            }
            else
            {
                clsReportResult.fnLog("API Create Intake Instance", "The Json/txt File was not found in the path: " + strJsonPath, "Fail", false, false);
                claim = null;
            }
            return claim;
        }

        private string fnGetTokenAccess(string pstrApiUser, string pstrApiPassword)
        {
            string strEndPoint = "api/v1/Authentication/Login";
            string strBaseURL = clsMG.fnGetURLEnv(clsDataDriven.strReportEnv);
            //Set Credentials
            TokenRequest tokenRequest = new TokenRequest();
            tokenRequest.Username = pstrApiUser;
            tokenRequest.Password = pstrApiPassword;
            string json = JsonConvert.SerializeObject(tokenRequest);
            //Get Token
            clsRest rest = new clsRest(strBaseURL);
            HTTP_RESPONSE resp = rest.POST(strEndPoint, json);
            TokenResponse objToken = JsonConvert.DeserializeObject<TokenResponse>(resp.MessageBody);
            return objToken.Token;
        }

        public bool fnIntakeInstanceAPI(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("Intake Instance API", "<<<<<<<<<< Intake Instance API Function Starts. >>>>>>>>>>", "Info", false);
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "API");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    switch (objData.fnGetValue("Action", "").ToUpper()) 
                    {
                        case "CREATEINSTANCE":
                            IntakeResponse ciResponse = fnAPICreateInstance(objData.fnGetValue("ApiUser", ""), objData.fnGetValue("ApiPassword", ""), objData.fnGetValue("JsonPath", ""));
                            if (ciResponse != null)
                            {
                                clsReportResult.fnLog("Intake Instance API", "The Intake Instance was created with IntakeInstanceId: "+ ciResponse.IntakeInstanceId + " and ConfirmationNumber: "+ ciResponse.ConfirmationNumber + " and IncidentNumber: "+ ciResponse.IncidentNumber + ".", "Pass", false);
                                clsData objSaveData = new clsData();
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "IntakeInstanceId", intRow, ciResponse.IntakeInstanceId);
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "ConfirmationNumber", intRow, ciResponse.ConfirmationNumber);
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "IncidentNumber", intRow, ciResponse.IncidentNumber);
                            }
                            else 
                            {
                                clsReportResult.fnLog("Intake Instance API", "The Create Intake Instance was not created as expected via API.", "Fail", false);
                                blResult = false;
                            }
                            break;
                        case "SUBMITINSTANCE":
                            IntakeResponse sbResponse = fnAPISubmitClaim(objData.fnGetValue("ApiUser", ""), objData.fnGetValue("ApiPassword", ""), objData.fnGetValue("JsonPath", ""));
                            if (sbResponse != null)
                            {
                                clsReportResult.fnLog("Intake Instance API", "The Intake Instance was created with IntakeInstanceId: " + sbResponse.IntakeInstanceId + " and ConfirmationNumber: " + sbResponse.ConfirmationNumber + ".", "Pass", false);
                                clsData objSaveData = new clsData();
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "IntakeInstanceId", intRow, sbResponse.IntakeInstanceId);
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "ConfirmationNumber", intRow, sbResponse.ConfirmationNumber);
                                objSaveData.fnSaveValue(clsDataDriven.strDataDriverLocation, "API", "IncidentNumber", intRow, sbResponse.IncidentNumber);
                                clsConstants.strTempConfirmationNo = sbResponse.ConfirmationNumber;
                                clsConstants.strTempClaimNo = sbResponse.IncidentNumber;
                            }
                            else
                            {
                                clsReportResult.fnLog("Intake Instance API", "The Submit Intake Instance was not created as expected via API.", "Fail", false);
                                blResult = false;
                            }
                            break;
                    }

                }
            }
            return blResult;
        }



    }
}
