using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class DuplicateCCModel
    {
        #region Duplicate Claim check
        public static string strEnvironmentBar = "//*[@id='EnvironmentBar']";
        public static string strDuplicateCheckPage = "//span[contains(text(), 'Duplicate Claim')]";
        public static string strDuplicatePageLabel = "//span[@data-bind='text:Value.Label']";
        
        public static string strLossIncidentDate = "//div[@class='row' and div[span[text()='Incident Date'] or span[text()='Loss Date']]]//input[@class='form-control']";
        public static string strLossIncidentTime = "//div[@class='row' and div[span[text()='Loss Time'] or span[text()='Incident Time']]]//input[@class='form-control']";
        public static string strReportedBy = "//div[@class='row' and div[span[text()='Reported By']]]//span[@class='select2-selection select2-selection--single']";
        public static string strReporterType = "//div[@class='row' and div[span[text()='Reporter Type']]]//span[@class='select2-selection select2-selection--single']";

        public static string strNextbutton = "//button[text()='Next']";
        public static string strRedWarning = "//*[@data-bind='text:ValidationMessage']";
        public static string strRedModalDialog = "//div[@class='md-toast md-toast-error']";


        #endregion
    }
}
