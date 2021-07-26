using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFrame_GlobalIntake.Utils
{
    public class clsEmail
    {
        public string strServer;
        public string strFromEmail;
        public string strPassword;
        public string strToEmail;
        public string strSubject;
        public string strContentBody;
        private string[] arrSTP = new string[2];

        public clsEmail()
        {
            strServer = "";
            strFromEmail = "";
            strPassword = "";
            strToEmail = "";
            strSubject = "";
            strContentBody = "";
        }


        public void fnSendSimpleEmail()
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(strFromEmail);
            mail.To.Add(strToEmail);
            mail.Subject = strSubject;
            mail.Body = strContentBody;
            mail.IsBodyHtml = true;
            fnGetServerName(strServer);
            if (strServer != "" && arrSTP[0] != "invalid" && strFromEmail != "" && strPassword != "")
            {
                SmtpClient smtp = new SmtpClient(arrSTP[0], Convert.ToInt32(arrSTP[1]));
                smtp.Credentials = new NetworkCredential(strFromEmail, strPassword);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        private void fnGetServerName(string pstrServer)
        {
            switch (strServer.ToUpper())
            {
                case "POPGMAIL":
                    arrSTP[0] = "pop.gmail.com";
                    arrSTP[1] = "995";
                    break;
                case "GMAIL":
                    arrSTP[0] = "smtp.gmail.com";
                    arrSTP[1] = "587";
                    break;
                case "OUTLOOK":
                    arrSTP[0] = "smtp.live.com";
                    arrSTP[1] = "587";
                    break;
                case "HOTMAIL":
                    arrSTP[0] = "smtp.live.com";
                    arrSTP[1] = "465";
                    break;
                case "OFFICE365":
                    arrSTP[0] = "smtp.office365.com";
                    arrSTP[1] = "587";
                    break;
                default:
                    arrSTP[0] = "invalid";
                    arrSTP[1] = "invalid";
                    break;
            }
        }

        public bool fnReadSimpleEmail(string pstrUser, string pstrPassword, string pstrVal)
        {
            int intTimeAttemp = 0;
            bool bFound = false;
            fnGetServerName(strServer);
            do
            {
                Pop3Client client = new Pop3Client();
                client.Connect(arrSTP[0], Convert.ToInt32(arrSTP[1]), true);
                client.Authenticate(strFromEmail, strPassword, AuthenticationMethod.UsernameAndPassword);
                int intEmailcount = client.GetMessageCount();
                for (int intRow = intEmailcount; intRow >= 1; intRow--)
                {
                    Message message = client.GetMessage(intRow);
                    MessagePart messagepart = message.FindFirstPlainTextVersion();
                    if (messagepart != null)
                    {
                        //Get Token as Plan Text
                        string strTemp = messagepart.GetBodyAsText();
                        if (strTemp.Contains(pstrVal))
                        {
                            bFound = true;
                            break;
                        }
                    }
                    else
                    {
                        //Get Token as HTML
                        messagepart = message.FindFirstHtmlVersion();
                        if (messagepart != null)
                        {
                            string strTemp = messagepart.GetBodyAsText();
                            if (strTemp.Contains(pstrVal))
                            {
                                bFound = true;
                                break;
                            }
                        }
                    }
                }
                intTimeAttemp++;
                if (!bFound) { Thread.Sleep(TimeSpan.FromSeconds(10)); }
                client.Disconnect();
            }
            while (intTimeAttemp < 12 && !bFound);
            return bFound;
        }


        public string fnReadEmailText(string pstrContainsText, string pstrStartWithPlainText, string pstrEndwithPlainText, string pstrStartWithHtml, string pstrEndwithHtml)
        {
            string strEmailText = "";
            int intTimeAttemp = 0;
            fnGetServerName(strServer);
            do
            {
                Pop3Client client = new Pop3Client();
                client.Connect(arrSTP[0], Convert.ToInt32(arrSTP[1]), true);
                client.Authenticate(strFromEmail, strPassword, AuthenticationMethod.UsernameAndPassword);
                int intEmailcount = client.GetMessageCount();
                for (int intRow = intEmailcount; intRow >= 1; intRow--)
                {
                    Message message = client.GetMessage(intRow);
                    MessagePart messagepart = message.FindFirstPlainTextVersion();
                    if (messagepart != null)
                    {
                        //Get Token as Plan Text
                        string strTemp = messagepart.GetBodyAsText();
                        if (strTemp.Contains(pstrContainsText))
                        {
                            string[] arrSeparators = { pstrStartWithPlainText, pstrEndwithPlainText };
                            string[] arrToken = strTemp.Split(arrSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                            strEmailText = arrToken[1].Replace("\n", "").Replace("\r", "");
                            break;
                        }
                    }
                    else
                    {
                        //Get Token as HTML
                        messagepart = message.FindFirstHtmlVersion();
                        if (messagepart != null)
                        {
                            string strTemp = messagepart.GetBodyAsText();
                            if (strTemp.Contains(pstrContainsText))
                            {
                                string[] arrSeparators = { pstrStartWithHtml, pstrEndwithHtml };
                                string[] arrToken = strTemp.Split(arrSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                                strEmailText = arrToken[1].Replace("\n", "").Replace("\r", "");
                                break;
                            }
                        }
                    }
                }
                intTimeAttemp++;
                if (strEmailText == "") { Thread.Sleep(TimeSpan.FromSeconds(10)); }
                client.Disconnect();
            }
            while (intTimeAttemp < 12 && strEmailText == "");
            return strEmailText;
        }


    }
}
