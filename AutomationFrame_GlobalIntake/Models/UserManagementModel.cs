using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using System;
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
        public static string strSelectClientsModal = "//div[@id='clientSelectorModal_User' and contains(@style, 'display: block')]";
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



        public bool fnSelectClients(List<string> clientIds)
        {
            return clsUtils.TryExecute(
                () =>
                {
                    var button = this.driver.FindElement(objSelectClientsButton);
                    driver.fnScrollToElement(button);
                    button.Click();
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(strSelectClientsModal), TimeSpan.FromSeconds(1), 10);
                    var modalVisible = this.driver.fnWaitUntilElementVisible(objSelectClientsModal);

                    clsReportResult.fnLog(
                        "Opening Select Clients Modal",
                        "Opening Select Clients Modal",
                        modalVisible ? "Pass" : "Fail",
                        true,
                        !modalVisible,
                        "Modal is not visible"
                    );

                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//table[@id='clientSelectorTable_User']//tbody/tr[@role='row']"), TimeSpan.FromSeconds(1), 10);
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//a[@id='btn_close_client']"), TimeSpan.FromSeconds(1), 10);
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
                            clsReportResult.fnLog("Sendkeys", $"The SendKeys for: Client Number with value: {clientId} was done.", modalVisible ? "Pass" : "Fail", true);
                        }    
                    );
                    modal.FindElement(objSelectClientsSaveButton).Click();
                }
            );
        }
    }
}
