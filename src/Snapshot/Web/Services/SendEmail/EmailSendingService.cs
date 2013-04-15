using System;
using System.Net;
using System.Net.Mail;

namespace Web.Services.SendEmail
{
    public class EmailSendingService : IEmailSendingService
    {
        public bool SendEmail(MailMessage message, SmtpServerDetails serverDeteails)
        {
            try
            {
                var client = new SmtpClient
                    {
                        Host = serverDeteails.Host,
                        Port = serverDeteails.Port,
                        Credentials = new NetworkCredential(serverDeteails.FromAddress, serverDeteails.FromPassword)
                    };
                client.Send(message);
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
    }
}