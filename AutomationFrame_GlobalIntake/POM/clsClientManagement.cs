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
    public class clsClientManagement
    {
        private static clsMegaIntake clsMG = new clsMegaIntake();
        private readonly clsWebElements clsWE = new clsWebElements();

        public bool fnUsersClientMgmtRestrictions(string pstrSetNo)
        {
            bool blResult = true;

            clsData objData = new clsData();
            objData.fnLoadFile(ConfigurationManager.AppSettings["FilePath"], "UserMGMT");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    switch (objData.fnGetValue("Role", "").ToUpper())
                    {
                        case "CLIENT INTAKE ONLY":
                            clsReportResult.fnLog("Users Client Management Restriction", "<<<<<<<<<< Users Client Management Restriction for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            fnClickHamburgerMenu();
                            if (!clsMG.IsElementPresent("//li//a[contains(text(),'Client Management')]"))
                            {
                                clsReportResult.fnLog("Users Client Management Restriction", "The Client Management section is not displayed for " + objData.fnGetValue("Role", "") + " as expected.", "Pass", true, false);
                            }
                            else 
                            {
                                clsReportResult.fnLog("Users Client Management Restriction", "The Client Management section is displayed for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                            }
                            break;
                        case "CLIENT INTAKE ONLY WITH DASHBOARD":
                            clsReportResult.fnLog("Users Client Management Restriction", "<<<<<<<<<< Users Client Management Restriction for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            fnClickHamburgerMenu();
                            if (!clsMG.IsElementPresent("//li//a[contains(text(),'Client Management')]"))
                            {
                                clsReportResult.fnLog("Users Client Management Restriction", "The Client Management section is not displayed for " + objData.fnGetValue("Role", "") + " as expected.", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Client Management Restriction", "The Client Management section is displayed for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                            }
                            break;
                        case "INTERNAL INTAKE USER":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "INTAKE LEAD":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "POWER USER ADMIN":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "INTAKE SUPER USER":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "INTAKE ADMIN":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getMyCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< MY INTAKES >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getResumeCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< RESUME INTAKES >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getAbandonedCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< CANCELLED INTAKES >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getDisseminationFailCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< FAILED DISSEMINATIONS >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section and buttons are displayed correctly on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "PRODUCT ADMIN":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getMyCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< MY INTAKES >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getAbandonedCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< CANCELLED INTAKES >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                if (clsMG.IsElementPresent("//a[contains(@data-bind,'getDisseminationFailCalls')]"))
                                {
                                    clsReportResult.fnLog("Users Home Restriction", "The <<< FAILED DISSEMINATIONS >>> button exist in CALLS section and is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                                }
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section and buttons are displayed correctly on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The My Intakes table is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "TENANT ADMIN":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            if (!clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home Grid is not displayed for " + objData.fnGetValue("Role", "") + " as expected.", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home Grid is displayed for " + objData.fnGetValue("Role", "") + ".Please verify you have the right access.", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "AUDIT USER":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            if (!clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home screen is not displayed for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home screen is displayed for " + objData.fnGetValue("Role", "") + ".Please verify you have the right access.", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "QUALITY USER":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            clsMG.fnGenericWait(() => clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"), TimeSpan.FromSeconds(1), 10);
                            if (clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The CALLS section is NOT displayed on Home screen for " + objData.fnGetValue("Role", "") + ".", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                        case "SIMPLE CASE USER":
                            clsReportResult.fnLog("Users Home Restriction", "<<<<<<<<<< Home Validation for " + objData.fnGetValue("Role", "") + "Starts. >>>>>>>>>>>", "Info", false);
                            clsMG.fnHamburgerMenu("Home");
                            clsWE.fnPageLoad(clsWE.fnGetWe("//span[contains(text(), 'You are currently logged into')]"), "Logged message", false, false);
                            if (!clsMG.IsElementPresent("//section[contains(@id,'calls-section')]"))
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home screen is not displayed for " + objData.fnGetValue("Role", "") + ".", "Pass", true, false);
                            }
                            else
                            {
                                clsReportResult.fnLog("Users Home Restriction", "The Home screen is displayed for " + objData.fnGetValue("Role", "") + ".Please verify you have the right access.", "Fail", true, false);
                                blResult = false;
                            }
                            break;
                    }
                }
            }
            if (blResult)
            { clsReportResult.fnLog("Users Home Restriction", "The Users Home Restriction Function was executed successfully.", "Pass", false); }
            else
            { clsReportResult.fnLog("Users Home Restriction", "The Users Home Restriction Function was not executed successfully.", "Fail", false); }

            return blResult;
        }

        public void fnClickHamburgerMenu()
        {
            //Verify if menu is collapsed
            var isElementStillPresent = clsMG.fnGenericWait(
                () =>
                {
                    return clsMG.IsElementPresent("//div[@id='slide-out' and contains(@style, 'translateX(-100%)')]");
                },
                TimeSpan.FromMilliseconds(500),
                3
            );

            if (isElementStillPresent)
            {
                clsWE.fnClick(clsWE.fnGetWe("//div[@class='float-left']//i"), "Hamburger Button", true, false);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
