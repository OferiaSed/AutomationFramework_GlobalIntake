using AutomationFrame_GlobalIntake.Models;
using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.POM
{
    public class clsGroupManagement
    {
        private clsWebElements clsWE = new clsWebElements();
        private clsMegaIntake clsMG = new clsMegaIntake();
        private clsIntakeFlow clsIF = new clsIntakeFlow();

        public bool fnUserGroups(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< User groups validation funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserGroups");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    clsMG.fnHamburgerMenu("User Management;Group Management");
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strGroupManagementPage), TimeSpan.FromSeconds(5), 10);
                    clsMG.fnGoTopPage();
                    if (clsWE.fnElementExist("Verify Groups Management Page", GroupManagementModel.strGroupManagementPage, true))
                    {
                        var strTemp = fnReadingClientsGroup(objData);
                        objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "UserGroups", "ClientsFromGM", intRow, strTemp);

                    }
                }
            }
            return blResult;
        }


        public bool fnverifyClientsPopup(string pstrSetNo)
        {
            bool blResult = true;
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< Verify Clients Popup validation funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserGroups");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    var strTemp = fnGettingClientsFromPopup(objData);
                    objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "UserGroups", "ClientsFromPopup", intRow, strTemp);
                    int count = 0;
                    string[] arrClients = objData.fnGetValue("ClientsFromGM", "").Split(';');
                    foreach (var client in arrClients)
                    {
                        if (client != "")
                        {
                            var locators = client.Split('|');
                            if (clsWE.fnElementExist("Locators", "//table[@id='clientSelectorTable_']//tr[td[text()='" + locators[0] + "'] and td[text()='" + locators[1] + "']]", false, false, ""))
                            {
                                clsReportResult.fnLog("Group Management Clients", "The clients displayed in screen matched as expected.", "Pass", false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Group Management Clients", "The clients displayed in screen does not matched as expected.", "Fail", false, true);
                            }
                            count++;
                        }

                    }
                }
            }
            return blResult;
        }

        public string fnReadingClientsGroup(clsData objData)
        {
            IList<IWebElement> cardList = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@data-bind, 'clientGroups')]//div[@class='card-body']"));
            string strTempClient = "";
            for (int i = 0; i < cardList.Count; i++)
            {
                IWebElement groupName = cardList.ElementAt(i).FindElement(By.XPath(".//input[contains(@data-bind, 'data.Description')]"));
                if (groupName.GetAttribute("value") == objData.fnGetValue("GroupName", ""))
                {
                    clsReportResult.fnLog("Group Management", "******Group name matched******.", "Pass", false, false);
                    //clsWE.fnScrollTo(groupName,"Scrolling to Group Name element");
                    IList<IWebElement> clientNumber = cardList.ElementAt(i).FindElements(By.XPath(".//div[contains(@data-bind,'ClientNumber')]"));
                    IList<IWebElement> clientName = cardList.ElementAt(i).FindElements(By.XPath(".//div[contains(@data-bind,'Name')]"));
                    for (int j = 0; j < clientNumber.Count; j++)
                    {
                        strTempClient = strTempClient + clientName.ElementAt(j).Text + "|" + clientNumber.ElementAt(j).Text + ";";
                    }
                }
                else
                {
                    clsReportResult.fnLog("Group Management", "******Group name not matched******.", "Info", false);
                }
            }
            return strTempClient;

        }

        public string fnGettingClientsFromPopup(clsData objData)
        {
            bool blResult = true;
            string strTemp = "";
            clsMG.fnHamburgerMenu("New Intake");
            if (clsWE.fnElementExist("Select Intake", "//h4[text()='Select Intake']", true))
            {
                clsWE.fnClick(clsWE.fnGetWe("//button[@id='selectClient_']"), "Select Client Button", false);
                clsWE.fnPageLoad(clsWE.fnGetWe("//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]"), "Select Client Popup", false, false);
                if (clsWE.fnElementExist("Select Client Popup", "//div[@id='clientSelectorModal_' and contains(@style, 'display: block;')]", true, false))
                {

                    IWebElement clientsTable = clsWebBrowser.objDriver.FindElement(By.XPath("//div[contains(@id,'clientSelectorModal_')]//table[contains(@id,'clientSelectorTable_')]"));
                    ICollection<IWebElement> rows = clientsTable.FindElements(By.XPath(".//tbody//tr"));

                    foreach (IWebElement row in rows)
                    {
                        List<IWebElement> addValue = new List<IWebElement>();
                        ICollection<IWebElement> cells = row.FindElements(By.XPath(".//td"));
                        IWebElement clientName = cells.ElementAt(1);
                        IWebElement clientNumber = cells.ElementAt(2);
                        strTemp = strTemp + clientName.Text + " " + clientNumber.Text + ";";
                    }
                    blResult = true;
                }
                else
                {
                    clsReportResult.fnLog("Select Client Popup", "The select intake popup was not displayed", "Fail", true, true);
                    blResult = false;
                }
            }
            //clsWE.fnClick(clsWE.fnGetWe("//a[contains(@id,'btn_close_client')]"), "closing modal", false, false);
            return strTemp;
        }
        /*
            public bool fnUpdateUserGroups(string pstrSetNo)
            {
                bool blResult = true;
                clsData objData = new clsData();
                clsReportResult.fnLog("", "<<<<<<<<<< User groups update funtion start >>>>>>>>>>.", "Info", false);
                objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserGroups");
                for (int intRow = 2; intRow <= objData.RowCount; intRow++)
                {
                    objData.CurrentRow = intRow;
                    if (objData.fnGetValue("Set", "") == pstrSetNo)
                    {
                        clsMG.fnHamburgerMenu("User Management;Group Management");
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strGroupManagementPage), TimeSpan.FromSeconds(5), 10);
                        clsMG.fnGoTopPage();
                        if (clsWE.fnElementExist("Verify Groups Management Page", GroupManagementModel.strGroupManagementPage, true))
                        {
                            IList<IWebElement> cardList = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@data-bind, 'clientGroups')]//div[@class='card-body']"));

                            for (int i = 0; i < cardList.Count; i++)
                            {
                                IWebElement groupName = cardList.ElementAt(i).FindElement(By.XPath(".//input[contains(@data-bind, 'data.Description')]"));
                                if (groupName.GetAttribute("value") == objData.fnGetValue("GroupName", ""))
                                {
                                    clsReportResult.fnLog("Group Management", "******Group name matched******.", "Pass", false, false);
                                    IWebElement SelectClientsButton = cardList.ElementAt(i).FindElement(By.XPath(".//button[contains(@data-bind, 'clientSelectorModal')]"));
                                    IWebElement SaveGroupButton = cardList.ElementAt(i).FindElement(By.XPath(".//button[contains(@data-bind,'saveClientGroup')]"));
                                    SelectClientsButton.Click();
                                    clsMG.fnGenericWait(() => clsMG.IsElementPresent("//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]"), TimeSpan.FromSeconds(1), 10);
                                    if (clsMG.IsElementPresent("//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]"))
                                    {
                                        clsWE.fnClick(clsWE.fnGetWe("(//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]//tr)[3]//label[@class='form-check-label']"),"Click checkbox",false,false);
                                        clsMG.fnGenericWait(() => clsMG.IsElementPresent("//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]//a[contains(@id, 'btn_close_client')]"), TimeSpan.FromSeconds(1), 10);
                                        clsWE.fnClick(clsWE.fnGetWe("//div[contains(@id,'clientSelectorModal_clientGroup') and contains(@style,'display: block')]//a[contains(@id, 'btn_close_client')]"), "Click Save", true, false);
                                    }
                                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strGroupManagementPage), TimeSpan.FromSeconds(1), 10);
                                    fnSaveGroupData(pstrSetNo);
                                    SaveGroupButton.Click();
                                }

                            }
                        }
                    }
                }
                return blResult;
            }
        */

        public bool fnReadingSelectedLocations(string pstrSetNo)
        {
            bool blResult = true;
            string strTempLoc = "";
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< Reading selected location funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "AccUnitSec");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strAvaliableRestr), TimeSpan.FromSeconds(5), 10);
                    clsWE.fnScrollTo(clsWE.fnGetWe(GroupManagementModel.strAvaliableRestr),"scrolling to available restriction section",true,false,"");
                    if (clsWE.fnElementExist("Verify Edit User Page", GroupManagementModel.strAvaliableRestr, true))
                    {
                        IWebElement SelectedLocTable = clsWebBrowser.objDriver.FindElement(By.XPath(GroupManagementModel.strSelectedLocTable));
                        ICollection<IWebElement> rows = SelectedLocTable.FindElements(By.XPath(".//tbody//tr"));

                        foreach (IWebElement row in rows)
                        {
                            List<IWebElement> addValue = new List<IWebElement>();
                            ICollection<IWebElement> cells = row.FindElements(By.XPath(".//td"));
                            IWebElement restrictionType = cells.ElementAt(0);
                            IWebElement accountNumber = cells.ElementAt(2);
                            IWebElement unitNumber = cells.ElementAt(5);
                            if(restrictionType.Text == "Account")
                            {
                                strTempLoc = strTempLoc + restrictionType.Text + ":=" + accountNumber.Text + ";";
                                objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "AccUnitSec", "AccUnitVal", intRow, strTempLoc);
                            }
                            if (restrictionType.Text == "Unit")
                            {
                                strTempLoc = strTempLoc + restrictionType.Text + ":=" + unitNumber.Text + ";";
                                objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "AccUnitSec", "AccUnitVal", intRow, strTempLoc);
                            }
                        }

                    }
                }
            }
            return blResult;
        }

        public bool fnRVerifyLocationLookup(string pstrSetNo)
        {
            bool blResult = true;
            string strTempLoc = "";
            clsData objData = new clsData();
            clsReportResult.fnLog("", "<<<<<<<<<< Reading and verifying selected location funtion start >>>>>>>>>>.", "Info", false);
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserMGMT");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    clsMG.fnGenericWait(() => clsMG.IsElementPresent(GroupManagementModel.strAvaliableRestr), TimeSpan.FromSeconds(5), 10);
                    clsWE.fnScrollTo(clsWE.fnGetWe(GroupManagementModel.strAvaliableRestr), "scrolling to available restriction section", true, false, "");
                    if (clsWE.fnElementExist("Verify Edit User Page", GroupManagementModel.strAvaliableRestr, true))
                    {
                        IWebElement SelectedLocTable = clsWebBrowser.objDriver.FindElement(By.XPath(GroupManagementModel.strSelectedLocTable));
                        ICollection<IWebElement> rows = SelectedLocTable.FindElements(By.XPath(".//tbody//tr"));

                        foreach (IWebElement row in rows)
                        {
                            List<IWebElement> addValue = new List<IWebElement>();
                            ICollection<IWebElement> cells = row.FindElements(By.XPath(".//td"));
                            IWebElement restrictionType = cells.ElementAt(0);
                            IWebElement accountNumber = cells.ElementAt(2);
                            IWebElement accountName = cells.ElementAt(3);
                            IWebElement unitName = cells.ElementAt(4);
                            IWebElement unitNumber = cells.ElementAt(5);
                            strTempLoc = strTempLoc + restrictionType.Text + "|" + accountNumber.Text + "|" + accountName.Text + "|" + unitName.Text + "|" + unitNumber.Text + ";";
                            objData.fnSaveValue(ConfigurationManager.AppSettings["FilePath"], "UserMGMT", "SelectedLocations", intRow, strTempLoc);
                        }

                    }
                }
            }
            return blResult;
        }
    }
}
