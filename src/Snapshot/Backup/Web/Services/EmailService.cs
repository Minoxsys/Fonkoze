using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Domain;

namespace Web.Services
{
    public class EmailService : IEmailService
    {
        private string host = "mail.evozon.com";
        private int port = 25;

        public bool SendMail(MailMessage message)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = host;
                client.Port = port;
                client.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}