using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Utils
{
    public class TokenRequest
    {
        public string Username;
        public string Password;
    }

    public class TokenResponse
    {
        public string Token;
    }

    public class IntakeResponse
    {
        public string Type;
        public string State;
        public string IntakeInstanceId;
        public string ConfirmationNumber;
        public string IncidentNumber;
    }


}
