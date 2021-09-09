using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class DisseminationModel
    {
        public static string strDisseminationPage = "//*[text()='Search Disseminations']";
        public static string strDisseminationType = "//div[select[contains(@data-bind, 'DisseminationType')]]//span[@role='combobox']";
        public static string strDisseminationStatus = "//div[select[contains(@data-bind, 'DisseminationStatus')]]//span[@role='combobox']";
        public static string strStartDate = "//input[@id='date-picker-Start-Date']";
        public static string strEndDate = "//input[@id='date-picker-End-Date']";
        public static string strDisseminationId = "//input[@id='disseminationId']";
        public static string strInstanceId = "//input[@id='instanceId']";
        public static string strConfirmationNumber = "//input[contains(@data-bind, 'ConfirmationNumber')]";
        public static string strClaimNumber = "//input[contains(@data-bind, 'ClaimNumber')]";
        public static string strGroupby = "//div[select[contains(@data-bind, 'groupBy')]]//span[@role='combobox']";
        public static string strClearButton = "//button[@id='primaryClear']";
        public static string strSearchButton = "//button[contains(@data-bind, 'searchDisseminations')]";
        public static string strFilterResults = "//input[@placeholder='Filter Results']";
        public static string strDetailButtonList = "//tr[td[text()='{DisseminationType}']]//a[contains(@class, 'details-button')]";
        public static string strDetailModal = "//div[@id='detailsModal' and contains(@style, 'display: block')]";
        public static string strDetailEmailMessage = "//div[@id='detailsModal' and contains(@style, 'display: block')]//div[@data-bind='text: Message']";
        public static string strCloseButton = "//div[@id='detailsModal' and contains(@style, 'display: block')]//a[text()='Close']";
    }
}
