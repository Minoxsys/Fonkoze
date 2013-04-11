using System;
using System.Net;
using System.Net.Mail;
using Web.Bootstrap;

namespace Web.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly string _host = AppSettings.SendEmailHost;
        private readonly int _port = Int32.Parse(AppSettings.SendEmailPort);
        private readonly string _fromAddress = AppSettings.SendEmailFrom;
        private readonly string _fromPassword = AppSettings.SendEmailPassword;

        public string SendMail(MailMessage message)
        {
            try
            {
                var client = new SmtpClient
                    {
                        Host = _host,
                        Port = _port,
                        Credentials = new NetworkCredential(_fromAddress, _fromPassword)
                    };
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