using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;

namespace AutomationFrame_GlobalIntake.POM
{
    class clsMegaIntake
    {
        //Object Declaration
        private clsWebElements clsWE = new clsWebElements();

        //Common Functions

        public bool fnEnterDatePicker(string pstrElement, string pstrWebElement, string pstrValue, bool pblScreenShot = false, bool pblHardStop = false, string pstrHardStopMsg = "")
        {
            bool blResult = true;
            if (pstrValue != "" && pstrWebElement != "")
            {
                try
                {
                    if (pstrWebElement != "" && pstrValue !="") 
                    {
                        clsReportResult.fnLog("SendKeys", "Step - Sendkeys on " + pstrElement, "info", false, false);
                        //Click on element
                        string[] arrdate = pstrValue.Split('/');
                        IWebElement objInput = clsWebBrowser.objDriver.FindElement(By.XPath(pstrWebElement));
                        objInput.Click();
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        //Select Year
                        IWebElement objYear = clsWebBrowser.objDriver.FindElement(By.XPath("//div[contains(@class, '--focused') or contains(@class, '--opened')]//select[@class='picker__select--year']"));
                        SelectElement selectItem = new SelectElement(objYear);
                        selectItem.SelectByText(arrdate[2]);

                        //Select Month
                        Dictionary<string, string> dicMonths = new Dictionary<string, string>();
                        dicMonths.Add("01", "January");
                        dicMonths.Add("02", "February");
                        dicMonths.Add("03", "March");
                        dicMonths.Add("04", "April");
                        dicMonths.Add("05", "May");
                        dicMonths.Add("06", "June");
                        dicMonths.Add("07", "July");
                        dicMonths.Add("08", "August");
                        dicMonths.Add("09", "September");
                        dicMonths.Add("10", "October");
                        dicMonths.Add("11", "November");
                        dicMonths.Add("12", "December");
                        IWebElement objMonth = clsWebBrowser.objDriver.FindElement(By.XPath("//div[contains(@class, '--focused') or contains(@class, '--opened')]//select[@class='picker__select--month']"));
                        selectItem = new SelectElement(objMonth);
                        selectItem.SelectByText(dicMonths[arrdate[0]]);

                        //Select Day
                        IList<IWebElement> lsDays = clsWebBrowser.objDriver.FindElements(By.XPath("//div[contains(@class, 'picker--focused') or contains(@class, '--opened')]//td[@role='presentation']//div[contains(@class, 'day--infocus')]"));
                        foreach (IWebElement optionDays in lsDays)
                        {
                            string strTempDay = optionDays.GetAttribute("innerText");
                            if (strTempDay.Length == 1) { strTempDay = "0" + strTempDay; }
                            if (strTempDay == arrdate[1]) { optionDays.Click(); break; }
                        }
                    }

                    blResult = true;
                }
                catch (Exception objException)
                {
                    blResult = false;
                    clsWebElements.fnExceptionHandling(objException);
                }
                if (blResult)
                {
                    //Step - Click on Submit Button
                    clsReportResult.fnLog("SendKeys", "The SendKeys for: " + pstrElement + " with value: " + pstrValue + " was done successfully.", "Pass", pblScreenShot, pblHardStop);
                }
                else
                {
                    blResult = false;
                    clsReportResult.fnLog("SendKeys", "The SendKeys for: " + pstrElement + " with value: " + pstrValue + " has failed.", "Fail", true, pblHardStop, pstrHardStopMsg);
                }
            }
            return blResult;
        }

        public bool IsElementPresent(string pstrWebElement)
        {
            try
            {
                IWebElement objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath(pstrWebElement));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void WaitWEUntilAppears(string pstrStepName, string pstrLocator, int pintTime)
        {
            int intCount = 0;
            do { Thread.Sleep(TimeSpan.FromSeconds(pintTime)); intCount++; }
            while (!clsWE.fnElementExistNoReport(pstrStepName, pstrLocator, false) && intCount <= pintTime);
        }

        /// <summary>
        /// Wait for condition to be true
        /// </summary>
        /// <param name="condition">Condition to wait for</param>
        /// <param name="sleepTime">Time to sleep between reattempts</param>
        /// <param name="attempts">Max number of attempts</param>
        /// <returns></returns>
        public bool fnGenericWait(Func<bool> condition, TimeSpan sleepTime, int attempts)
        {
            var count = 0;
            bool success, repeat;
            do
            {
                success = condition.Invoke();
                count++;
                repeat = count < attempts && !success;
                if (repeat) Thread.Sleep((int)sleepTime.TotalMilliseconds);
            }
            while (repeat);
            return success;
        }

        public bool fnCleanAndEnterText(string pstrElement, string pstrWebElement, string pstrValue, bool pblScreenShot = false, bool pblHardStop = false, string pstrHardStopMsg = "", bool bWaitHeader = true)
        {
            bool blResult = true;
            if (pstrValue != "" && pstrWebElement != "")
            {
                try
                {
                    clsReportResult.fnLog("SendKeys", "Step - Sendkeys on " + pstrElement, "info", false, false);
                    IWebElement objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath(pstrWebElement));
                    Actions action = new Actions(clsWebBrowser.objDriver);
                    objWebEdit.Click();
                    action.KeyDown(Keys.Control).SendKeys(Keys.Home).Perform();
                    objWebEdit.SendKeys(Keys.Delete);
                    objWebEdit.SendKeys(pstrValue);
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    //Thread.Sleep(1000);

                    if (bWaitHeader)
                    {
                        if (IsElementPresent("//*[@id='EnvironmentBar']"))
                        {
                            objWebEdit = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
                            Thread.Sleep(TimeSpan.FromMilliseconds(500));
                            objWebEdit.Click();
                            Thread.Sleep(TimeSpan.FromMilliseconds(500));
                        }
                    }
                    blResult = true;
                }
                catch (Exception objException)
                {
                    blResult = false;
                    clsWebElements.fnExceptionHandling(objException);
                }
                if (blResult)
                {
                    //Step - Click on Submit Button
                    clsReportResult.fnLog("SendKeys", "The SendKeys for: " + pstrElement + " with value: " + pstrValue + " was done successfully.", "Pass", pblScreenShot, pblHardStop);
                }
                else
                {
                    blResult = false;
                    clsReportResult.fnLog("SendKeys", "The SendKeys for: " + pstrElement + " with value: " + pstrValue + " has failed.", "Fail", true, pblHardStop, pstrHardStopMsg);
                }
            }
            return blResult;
        }

        public bool fnSelectDropDownWElm(string pstrElement, string pstrWebElement, string pstrValue, bool pblScreenShot = false, bool pblHardStop = false, string pstrHardStopMsg = "", bool bWaitHeader = true)
        {
            clsWebElements clsWE = new clsWebElements();
            bool blResult = true;
            IWebElement objDropDownContent;
            IList<IWebElement> objOptions;

            try
            {
                if (pstrElement != "" && pstrValue != "")
                {
                    clsReportResult.fnLog("SelectDropdown", "Step - Select Dropdown: " + pstrElement + " With Value: " + pstrValue, "Info", false);
                    IWebElement objDropDown = clsWebBrowser.objDriver.FindElement(By.XPath(pstrWebElement));
                    objDropDown.Click();
                    Thread.Sleep(1000);

                    if (IsElementPresent("//span[@class='select2-results']"))
                    {
                        //Common Dropdown
                        objDropDownContent = clsWebBrowser.objDriver.FindElement(By.XPath("//span[@class='select2-results']"));
                        objOptions = objDropDownContent.FindElements(By.XPath("//li[@role='treeitem']"));
                    }
                    else
                    {
                        //two Factor Dropdown
                        objDropDownContent = clsWebBrowser.objDriver.FindElement(By.XPath("//ul[@class='dropdown-content select-dropdown w-100 active']"));
                        objOptions = objDropDownContent.FindElements(By.XPath("//span[@class='filtrable']"));
                    }

                    foreach (IWebElement objOption in objOptions)
                    {
                        string pstrDropdownText = (objOption.GetAttribute("innerText"));
                        if (pstrDropdownText == pstrValue)
                        {
                            objOption.Click();
                            blResult = true;
                            break;
                        }
                    }

                    if (bWaitHeader) 
                    {
                        if (IsElementPresent("//nav[@id='EnvironmentBar']"))
                        {
                            objDropDown = clsWebBrowser.objDriver.FindElement(By.XPath("//nav[@id='EnvironmentBar']"));
                            Thread.Sleep(TimeSpan.FromMilliseconds(500));
                            objDropDown.Click();
                        }
                    }
                }
            }
            catch (Exception objException)
            {
                blResult = false;
                clsWebElements.fnExceptionHandling(objException);
                //Console.WriteLine("DropDown is not working for: " + pstrWebElement + " an exception was found: " + objException.Message);
                //clsReportResult.fnLog("DropdownSelectFail", "DropDown is not working for: " + pstrWebElement + " with value: " + pstrValue, "Fail", pblScreenShot, pblHardStop);
            }
            if (pstrElement != "" && pstrValue != "") 
            {
                if (blResult && pstrElement != "" && pstrValue != "")
                {
                    clsReportResult.fnLog("SelectListPass", "Select Dropdown for element: " + pstrElement + " was done successfully.", "Pass", pblScreenShot);
                }
                else
                {
                    blResult = false;
                    clsReportResult.fnLog("SelectListFail", "Select Dropdown for element: " + pstrElement + " has failed with value: " + pstrValue + " and locator " + pstrWebElement, "Fail", pblScreenShot);
                    //clsReportResult.fnLog("DropdownSelectFail", "DropDown is not working for: " + pstrElement + " with value: " + pstrValue + " and locator: " + pstrWebElement, "Fail", pblScreenShot, pblHardStop); 
                }
            }

            return blResult;
        }

        public void fnReadCaptcha()
        {
            var objCaptcha = clsWebBrowser.objDriver.FindElement(By.XPath("//img[@id='ForgotUserNameCaptcha_CaptchaImage']"));
            Point location = objCaptcha.Location;

            var screenshot = (clsWebBrowser.objDriver as ChromeDriver).GetScreenshot();
            using (MemoryStream stream = new MemoryStream(screenshot.AsByteArray))
            {
                using (Bitmap bitmap = new Bitmap(stream))
                {
                    RectangleF part = new RectangleF(location.X, location.Y, objCaptcha.Size.Width, objCaptcha.Size.Height);
                    using (Bitmap bn = bitmap.Clone(part, bitmap.PixelFormat))
                    {
                        bn.Save(@"\\memfp02\share\Any\4th_Automation\VisualStudio\Captcha\" + "CaptchaImg.Png");
                    }
                }
            }

            //Read Text from Image
            string strLangLoc = @"\\memfp02\share\Any\4th_Automation\VisualStudio\Tesseract\Lang";
            using (var engine = new TesseractEngine(strLangLoc, "eng", EngineMode.Default))
            {
                Page ocrPage = engine.Process(Pix.LoadFromFile(@"\\memfp02\share\Any\4th_Automation\VisualStudio\Captcha\" + "CaptchaImg.Png"), PageSegMode.AutoOnly);
                var captchaText = ocrPage.GetText();
            }



        }


        //GI Functions

        public string fnGetURLEnv(string pstrEnv)
        {
            string URL = "";
            switch (pstrEnv.ToUpper())
            {
                case "QA":
                    URL = ConfigurationManager.AppSettings["UrlQA"];
                    break;
                case "UAT":
                    URL = ConfigurationManager.AppSettings["UrlUAT"];
                    break;
                case "E2E":
                    URL = ConfigurationManager.AppSettings["UrlE2E"];
                    break;
                default:
                    URL = "";
                    break;
            }
            return URL;
        }

        public string fnExecuteQuery(string pstrContract, string pstrSatate)
        {
            string strResult = "";
            clsDB objDBOR = new clsDB();
            objDBOR.fnOpenConnection(objDBOR.GetConnectionString(ConfigurationManager.AppSettings["Host"], ConfigurationManager.AppSettings["Port"], ConfigurationManager.AppSettings["Service"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]));
            DataTable datatable = new DataTable();
            string strQuery = "select * from viaone.cont_st_off where cont_num = '" + pstrContract + "' and deleted = 'N' and data_set = 'WC' and state = '" + pstrSatate + "'";
            datatable = objDBOR.fnDataSet(strQuery);
            Thread.Sleep(3000);
            if (datatable != null && datatable.Rows.Count > 0)
            {
                DataRow row = datatable.Rows[0];
                strResult = Convert.ToString(row["EX_OFFICE"]);
            }
            else
            {
                strResult = "";
            }
            objDBOR.fnCloseConnection();

            return strResult;
        }

        public void fnHamburgerMenu(string pstrMenu)
        {
            clsReportResult.fnLog("Hamburger Menu", "Selecting a Menu Option: " + pstrMenu.ToString(), "Info", false);
            //Verify if menu is collapsed
            var isElementStillPresent = this.fnGenericWait(
                () =>
                {
                    return !IsElementPresent("//div[@id='slide-out' and not(contains(@style, 'translateX(0px)'))]");
                },
                TimeSpan.FromMilliseconds(500),
                10
            );
            clsUtils.fnExecuteIf(isElementStillPresent,
                () =>
                {
                    clsWE.fnClick(clsWE.fnGetWe("//div[@class='float-left']//i"), "Hamburger Button", true, false);
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            );

            //Select Menu Item
            if (!pstrMenu.Contains(";"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                if (IsElementPresent("//a[contains(text(), '" + pstrMenu + "')]"))
                {
                    clsWE.fnClick(clsWE.fnGetWe("//a[contains(text(), '" + pstrMenu + "')]"), pstrMenu + " Link", false, false);
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                else
                {
                    clsReportResult.fnLog("Hamburger Menu", "The menu/submenu: " + pstrMenu + " does not exist.", "Info", false);
                }
            }
            else
            {
                string[] arrMenu = pstrMenu.Split(';');
                for (int intMenu = 0; intMenu <= arrMenu.Length - 1; intMenu++)
                {
                    if (intMenu == 0)
                    {
                        if (IsElementPresent("//li[contains(@data-bind, '" + arrMenu[intMenu].Replace(" ", "") + "')]/a"))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            clsWE.fnClick(clsWE.fnGetWe("//li[contains(@data-bind, '" + arrMenu[intMenu].Replace(" ", "") + "')]/a"), arrMenu[intMenu] + " Link", false, false);
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            clsReportResult.fnLog("Hamburger Menu", "The menu/submenu: " + arrMenu[intMenu] + " does not exist.", "Info", false);
                        }
                    }
                    else
                    {
                        if (IsElementPresent("//a[contains(text(), '" + arrMenu[intMenu] + "')]"))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            clsWE.fnClick(clsWE.fnGetWe("//a[contains(text(), '" + arrMenu[intMenu] + "')]"), arrMenu[intMenu] + " Link", false, false);
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            clsReportResult.fnLog("Hamburger Menu", "The menu/submenu: " + arrMenu[intMenu] + " does not exist.", "Info", false);
                        }
                    }

                }

            }
        }

        public void fnSwitchToWindowAndClose(int pSwithToWindow) 
        {
            clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles[pSwithToWindow]);
            clsWebBrowser.objDriver.Close();
        }

        public void fnSwitchToWindow(string pstrTitle)
        {
            bool blFound = false;
            foreach (var handle in clsWebBrowser.objDriver.WindowHandles) 
            {
                clsWebBrowser.objDriver.SwitchTo().Window(handle);
                string strTitle = clsWebBrowser.objDriver.Title;
                if (pstrTitle.ToUpper() == strTitle.ToUpper())
                { 
                    blFound = true;
                    break;
                }
            }
            if (blFound)
            { clsReportResult.fnLog("Switch to Windown", "The Switch to Window: "+ pstrTitle + " was completed successfully.", "Info", false); }
            else 
            { 
                clsReportResult.fnLog("Switch to Windown", "The Window Page: "+ pstrTitle + " was not found.", "Fail", false);
                clsWebBrowser.objDriver.SwitchTo().Window(clsWebBrowser.objDriver.WindowHandles[0]);
            }
        }

        public void fnGoTopPage() 
        {
            IWebElement objWeBar = clsWebBrowser.objDriver.FindElement(By.XPath("//*[@id='EnvironmentBar']"));
            Actions action = new Actions(clsWebBrowser.objDriver);
            objWeBar.Click();
            action.SendKeys(Keys.Home).Perform();
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }

        public void fnHighlight(IWebElement pElement)
        {
            clsWE.fnScrollTo(pElement, "Scroll to element", false, false);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            IJavaScriptExecutor js = (IJavaScriptExecutor)clsWebBrowser.objDriver;
            js.ExecuteScript("arguments[0].setAttribute('style', 'background: transparent; border: 2px solid red;');", pElement);
        }



    }
}
