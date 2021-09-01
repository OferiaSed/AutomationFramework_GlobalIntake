using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationFrame_GlobalIntake.Models
{
    public static class CreateIntakeScreen
    {
        public static By objFloatingListSelector = By.XPath("//div[@id='list-example']/a[span]");
        public static By objAllLabels = By.XPath("//div[contains(@class, 'question-row')]/div/div[@class='row']//div[@class='row']//div[@class='col-md-12']");
        public static By objIsThisTheLossLocation = By.XPath("//div[@class='row' and div[span[contains(text(), 'Is This The Loss Location?')]]]//span[@class='select2-selection select2-selection--single']");      
    }
}
