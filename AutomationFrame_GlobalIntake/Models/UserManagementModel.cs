using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace AutomationFrame_GlobalIntake.Models
{
    public class UserManagementModel : BasePageModel
    {
        public UserManagementModel(IWebDriver driver, clsMegaIntake clsMG) : base(driver, clsMG) { }

        // User Management Screen
        public static By objUserManagementButtons(int n) => By.XPath($"//div[@class='py-3']//button[{n}]");
        public By objImportUsers = objUserManagementButtons(2);
        public By objExportUsersSelector = objUserManagementButtons(3);
        public By objImportUsersInput = By.Id("file-import");

        // Add User Screen
        public static string strRestrictionTypeDropdown = "//*[@id='select2-restriction-type-select-container']";
        public static string strSelectClientsModal = "//div[@id='clientSelectorModal_User']/div";
        public static string strClientSecurityTypes = "//div[select[contains(@data-bind, 'ClientSecurityTypes')]]//span[@class='select2-selection__rendered']";
        public static string strSearchAccountNumberInput = "//input[contains(@data-bind,'searchAcctNumber')]";
        public static string strTagDropdown = "//span[contains(@class, 'tag-select-container-custom')]/ul[@class='select2-selection__rendered']";
        public static string strSearchUnitNumberInput = "//input[contains(@data-bind,'searchUnitNumber')]";

        public static By objSelectClientsButton = By.XPath("//button[@data-target='#clientSelectorModal_User']");
        public static By objSelectClientsModal = By.XPath(strSelectClientsModal);
        public static By objSelectClientsSaveButton = By.XPath(".//a[@id='btn_close_client']");
        public static By objSelectClientsCheckboxByClientId(string clientId) => By.XPath($".//td[text()='{clientId}']/../td//input/../label");
        public static By objSeachAccountUnitButton = By.XPath("//button[contains(@data-bind,'searchAccountUnit')]");
        public static By objSelectRestrictionAcountByAccountNumber(string accountNumber) => By.XPath($"//tr[td[contains(@data-bind,'AccountNumber') and text()='{accountNumber}']]//Label");
        public static By objSelectRestrictionAcountByUnitNumber(string unitNumber) => By.XPath($"//tr[td[contains(@data-bind,'UnitNumber') and text()='{unitNumber}']]//Label");

        //Group Management screen
        public static string strGroupManagementPage = "//h2[contains(text(),'Create Client Groups')]";
        public static string strParentGroupName = "//input[contains(@data-bind,'Description')]";
        public static string strAvaliableRestr = "//h4[contains(text(),'Available Restriction')]";
        public static string strSelectedLocTable = "//table[tbody[contains(@data-bind,'AccountUnitWhiteList')]]";
        public static string strCardDescription = ".//input[contains(@data-bind, 'data.Description')]";
        public static string strSelectClientButton = ".//button[contains(@data-bind, 'clientSelectorModal')]";
        public static string strSaveGroupButton = ".//button[contains(@data-bind,'saveClientGroup')]";
        public static string strSelectorClientModal = "//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]";
        public static string strClientNumberName = "//input[@placeholder='Client Number or Name']";
        public static string strCheckboxElement = "(//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]//tr)[3]//label[@class='form-check-label']";
        public static string strCloseClientModal = "//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]//a[contains(@id, 'btn_close_client')]";
        

        public bool fnSelectClients(List<string> clientIds)
        {
            return clsUtils.TryExecute(
                () =>
                {
                    var button = this.driver.FindElement(objSelectClientsButton);
                    driver.fnScrollToElement(button);
                    button.Click();
                    clsMG.WaitWEUntilAppears("Wait for Select Clients Modal", strSelectClientsModal, 0);
                    var modalVisible = this.driver.fnWaitUntilElementVisible(objSelectClientsModal);

                    clsReportResult.fnLog(
                        "Opening Select Clients Modal",
                        "Opening Select Clients Modal",
                        modalVisible ? "Pass" : "Fail",
                        true,
                        !modalVisible,
                        "Modal is not visible"
                    );

                    var modal = driver.FindElement(objSelectClientsModal);
                    var filterByClientId = modal.FindElement(By.XPath(".//input"));
                    clientIds.ForEach(
                        clientId =>
                        {
                            filterByClientId.Clear();
                            filterByClientId.SendKeys(clientId);
                            var clientCheckbox = objSelectClientsCheckboxByClientId(clientId);
                            this.driver.fnWaitUntilElementVisible(clientCheckbox);
                            modal.FindElement(clientCheckbox).Click();
                        }    
                    );
                    modal.FindElement(objSelectClientsSaveButton).Click();
                }
            );
        }
    }
}
