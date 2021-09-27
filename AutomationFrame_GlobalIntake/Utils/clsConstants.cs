using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Utils
{
    public class clsConstants
    {
        //DB Crdentials
        public static string strDBHost = "lltcsed1dvq-scan";
        public static string strDBPort = "1521";
        public static string strDBService = "viaoneR";
        public static string strDBUser = "oferia";
        public static string strDBPass = "P@ssw0rd#04";

        //Training Mode
        public static bool blTrainingMode = false;
        public static bool blLogin = false;
        public static string strSubmitClaimTrainingMode = "";
        public static string strResumeClaimTrainingMode = "";
        public static string strSubmitClaimNormalMode = "";
        public static string strResumeClaimNormalgMode = "";

        //Claim Creation
        public static string strTempClaimNo = "";
        public static string strTempConfirmationNo = "";
        public static string strOfficeEmail = "";

        public static string ssnMask = "XXX-XX-";

    }
}
