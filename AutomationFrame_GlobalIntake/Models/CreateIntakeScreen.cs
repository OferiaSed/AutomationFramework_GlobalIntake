using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationFrame_GlobalIntake.Models
{
    public class CreateIntakeScreen
    {
        public static By objFloatingListSelector = By.XPath("//div[@id='list-example']/a[span]");

        #region Client/Location Information
        //Is This The Loss Location
        public static By objIsThisTheLossLocation = By.XPath("//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']");

        #endregion



    }
}
