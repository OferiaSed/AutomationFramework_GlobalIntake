﻿using AutomationFrame_GlobalIntake.POM;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace AutomationFrame_GlobalIntake.Models
{
    public class UserManagementModel : BasePageModel
    {
        public UserManagementModel(IWebDriver driver, clsMegaIntake clsMG) : base(driver, clsMG) { }

        // Add User Screen
        public static By objSelectClientsButton = By.XPath("//button[@data-target='#clientSelectorModal_User']");
        public static By objSelectClientsModal = By.XPath(strSelectClientsModal);
        public static By objSelectClientsSaveButton = By.XPath(".//a[@id='btn_close_client']");
        public static By objSelectClientsCheckboxByClientId(string clientId) => By.XPath($".//td[text()='{clientId}']/../td//input/../label");

        public static string strRestrictionTypeDropdown = "//*[@id=select2-restriction-type-select-container']";
        public static string strSelectClientsModal = "//div[@id='clientSelectorModal_User']/div";
        public static string strClientSecurityTypes = "//div[select[contains(@data-bind, 'ClientSecurityTypes')]]//span[@class='select2-selection__rendered']";

        public bool fnSelectClients(List<string> clientIds)
        {
            return clsUtils.TryExecute(
                () =>
                {
                    var button = this.driver.FindElement(objSelectClientsButton);
                    driver.fnScrollToElement(button);
                    button.Click();
                    clsMG.WaitWEUntilAppears("Wait for Select Clients Modal", strSelectClientsModal, 0);
                    var modalVisible = this.driver.fnIsElementVisible(objSelectClientsModal);

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
                            clsMG.fnWaitUntilElementVisible(clientCheckbox);
                            modal.FindElement(clientCheckbox).Click();
                        }    
                    );
                    modal.FindElement(objSelectClientsSaveButton).Click();
                }
            );
        }
    }
}
