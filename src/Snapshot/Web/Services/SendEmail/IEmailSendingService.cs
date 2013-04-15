using System.Net.Mail;

namespace Web.Services.SendEmail
{
    public interface IEmailSendingService
    {
        bool SendEmail(MailMessage message, SmtpServerDetails serverDeteails);
    }
}