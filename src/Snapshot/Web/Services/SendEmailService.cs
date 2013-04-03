using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Web.Bootstrap;
using System.Net;

namespace Web.Services
{
    public class SendEmailService : ISendEmailService
    {
        private string host = AppSettings.SendEmailHost;
        private int port = Int32.Parse(AppSettings.SendEmailPort);
        private string fromAddress = AppSettings.SendEmailFrom;
        private string fromPassword = AppSettings.SendEmailPassword;

        public string SendMail(System.Net.Mail.MailMessage message)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = host;
                client.Port = port;
                client.Credentials = new NetworkCredential(fromAddress, fromPassword);
                client.Send(message);

                return "Email has been sent.";
            }
            catch (Exception ext)
            {
                return ext.Message;
            }
        }
    }
}