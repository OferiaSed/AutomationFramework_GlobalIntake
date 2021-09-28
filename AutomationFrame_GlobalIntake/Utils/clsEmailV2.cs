using AutomationFrame_GlobalIntake.Utils;
using AutomationFramework;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MyUtils.Email
{
    public class clsEmailV2
    {
        public enum emServer
        {
            POPGMAIL,
            GMAIL,
            OUTLOOK,
            HOTMAIL,
            OFFICE365
        }

        private string strEmail;
        private string strEmailPass;
        private bool blSaveAttachments;
        private emServer server;

        private string[] arrSTP = new string[2];
        private Pop3Client client;

        public List<string> Attachments = new List<string>();
        public string MessageID;
        public string FromID;
        public string FromName;
        public string Subject;
        public string Body;
        public string Html;

        public clsEmailV2(string strEmail, string strEmailPass, emServer server, bool blSaveAttachments = false)
        {
            this.strEmail = strEmail;
            this.strEmailPass = strEmailPass;
            this.blSaveAttachments = blSaveAttachments;
            this.server = server;
        }

        private void fnResolvePop3ServerName(emServer server)
        {
            switch (server)
            {
                case emServer.POPGMAIL:
                    arrSTP[0] = "pop.gmail.com";
                    arrSTP[1] = "995";
                    break;
                case emServer.GMAIL:
                    arrSTP[0] = "smtp.gmail.com";
                    arrSTP[1] = "587";
                    break;
                case emServer.OUTLOOK:
                    arrSTP[0] = "pop-mail.outlook.com";
                    arrSTP[1] = "995";
                    break;
                case emServer.HOTMAIL:
                    arrSTP[0] = "smtp.live.com";
                    arrSTP[1] = "465";
                    break;
                case emServer.OFFICE365:
                    arrSTP[0] = "outlook.office365.com";
                    arrSTP[1] = "995";
                    break;
                default:
                    arrSTP[0] = "invalid";
                    arrSTP[1] = "invalid";
                    break;
            }
        }

        public bool fnReadEmail(string pstrSubject, string pstrContainsText) 
        {
            //Clear Attachments of possible past read emails
            this.Attachments.Clear();

            client = new Pop3Client();
            try
            {
                //Connect to Email
                fnResolvePop3ServerName(server);
                client.Connect(arrSTP[0], Convert.ToInt32(arrSTP[1]), true);
                client.Authenticate(strEmail, strEmailPass, AuthenticationMethod.Auto);
                var messageCount = client.GetMessageCount();
                var Messages = new List<Message>(messageCount);

                //Get all messagesh
                for (int intEmailIndex = messageCount; intEmailIndex >= messageCount; intEmailIndex--)
                {
                    Message getMessage = client.GetMessage(intEmailIndex);
                    Messages.Add(getMessage);
                }

                //Find Email by partial subject
                var message = Messages.FirstOrDefault(
                    msg =>
                    {
                        var subject = msg.Headers.Subject.Trim();
                        var bodyOk = true;
                        if (!string.IsNullOrEmpty(pstrContainsText))
                        {
                            var stringToReview = "";
                            var plainContent = msg.FindFirstPlainTextVersion();
                            var htmlContent = msg.FindFirstHtmlVersion();
                            if (plainContent != null) stringToReview += plainContent.GetBodyAsText();
                            if (htmlContent != null) stringToReview += htmlContent.GetBodyAsText();
                            bodyOk = stringToReview.Contains(pstrContainsText);
                        }
                        return subject.Contains(pstrSubject) && bodyOk;
                    }
                );

                if (message == null)
                {
                    return false;
                }
                
                //Retrive Email Message
                this.MessageID = message.Headers.MessageId == null ? "" : message.Headers.MessageId.Trim();
                this.FromID = message.Headers.From.Address.Trim();
                this.FromName = message.Headers.From.DisplayName.Trim();
                this.Subject = message.Headers.Subject.Trim();

                //Get Text Message (Body and HTML as string)
                MessagePart plainTextPart = null, HTMLTextPart = null;
                plainTextPart = message.FindFirstPlainTextVersion();
                this.Body = (plainTextPart == null ? "" : plainTextPart.GetBodyAsText().Trim());
                HTMLTextPart = message.FindFirstHtmlVersion();
                this.Html = (HTMLTextPart == null ? "" : HTMLTextPart.GetBodyAsText().Trim());

                //Save Attachments
                if (this.blSaveAttachments)
                {
                    var list = message.FindAllAttachments();

                    //Create Attachment Directories
                    var strExtentAttachmentsDir = $@"{clsDataDriven.strReportLocation}\Attachments".Replace("\\\\", "\\");
                    if (list.Any())
                    {
                        Directory.CreateDirectory(strExtentAttachmentsDir);
                    }

                    foreach(var attachment in list)
                    {
                        var fileName = attachment.FileName.Split('.');
                        string strFilePath = Path.Combine(strExtentAttachmentsDir, $"{fileName[0].fnOnlyAlphanumericChars()}.{fileName[1]}");
                        FileStream stream = new FileStream(strFilePath, FileMode.Create);
                        BinaryWriter binaryStream = new BinaryWriter(stream);
                        binaryStream.Write(attachment.Body);
                        binaryStream.Close();
                        this.Attachments.Add(strFilePath);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                //TODO: Improve exception handling and add logging
                return false;
            }
            finally 
            {
                if(client.Connected) { client.Dispose(); }
            }
        }

        public string fnGetContentAsString(string pstrPlainTextStart, string pstrPlainTextEnd, string pstrHTMLTextStart, string pstrHTMLTextEnd)
        {
            string strResult = "";
            if (!string.IsNullOrEmpty(Body))
            {
                strResult = this.Body.fnTextBetween(pstrPlainTextStart, pstrPlainTextEnd);
            }
            else if (!string.IsNullOrEmpty(Html))
            {
                strResult = this.Html.fnTextBetween(pstrHTMLTextStart, pstrHTMLTextEnd);
            }

            return strResult.fnToSingleLineText();
        }

        public static Dictionary<string, string> fnGetUserAndPasswordEmail(string pstrSetNo)
        {
            clsData objData = new clsData();
            Dictionary<string, string> dicCredentials = new Dictionary<string, string>();
            objData.fnLoadFile(clsDataDriven.strDataDriverLocation, "LogInData");
            for (int intRow = 2; intRow <= objData.RowCount; intRow++)
            {
                objData.CurrentRow = intRow;
                if (objData.fnGetValue("Set", "") == pstrSetNo)
                {
                    dicCredentials.Add("User", objData.fnGetValue("EmailAcc", ""));
                    dicCredentials.Add("Password", objData.fnGetValue("PassAcc", ""));
                }
            }
            return dicCredentials;
        }
    }
}
