using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    class GroupManagementModel
    {
        public static string strGroupManagementPage = "//h2[contains(text(),'Create Client Groups')]";
        public static string strParentGroupName = "//input[contains(@data-bind,'Description')]";
        public static string strAvaliableRestr = "//h4[contains(text(),'Available Restriction')]";
        public static string strSelectedLocTable = "//table[tbody[contains(@data-bind,'AccountUnitWhiteList')]]";
        
    }
}
