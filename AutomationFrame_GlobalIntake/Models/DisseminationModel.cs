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
    public static string strDetailButtonList = "//tr[td[text()='{DisseminationType}']]//a[contains(@class, '{MSGTYPE}-button')]";
    public static string strDetailModal = "//div[@aria-labelledby='myModalLabel' and contains(@style, 'display: block')]";
    public static string strDetailMessage = "//div[@aria-labelledby='myModalLabel' and contains(@style, 'display: block')]//div[@data-bind='text: Message']";
    public static string strContentMessage = "//div[@aria-labelledby='myModalLabel' and contains(@style, 'display: block')]//div[contains(@data-bind, 'Content()')]";
    public static string strCloseButton = "//div[@aria-labelledby='myModalLabel' and contains(@style, 'display: block')]//a[text()='Close']";
    public static string strRowCheckbox = "(//table[@id='results']//label[@class='form-check-label'])[2]";
    public static string strResendButton = "//button[contains(text(), 'Resend Selected')]";
    public static string strResendModal = "//button[contains(text(), 'Resend Selected')]";
    public static string strCancelResend = "//div[@id='megaModalDialog' and contains(@style, 'display: block')]//button[text()='Cancel']";
    public static string strConfirmResend = "//div[@id='megaModalDialog' and contains(@style, 'display: block')]//button[text()='Confirm']";
    public static string strResendGreenMessage = "//div[text()='Resend dissemination scheduled.']";
}