using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Models
{
    public class LoginModel
    {
        public static string strBeginButton = "//button[text()='BEGIN']";
        public static string strAcceptCookies = "//button[@id='cookie-accept']";
        public static string strLoggedInLabel = "//span[contains(text(), 'You are currently logged into')]";
        public static string strLoggedInTraining = "//span[text()='Training Mode - ']";
        public static string strNoAccessMessage = "//*[contains(text(), 'You do not have permission to access Training Mode. Please return to the main login screen.')]";

    }
}
