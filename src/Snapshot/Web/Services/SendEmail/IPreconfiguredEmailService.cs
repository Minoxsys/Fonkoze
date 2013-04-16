using System.Net.Mail;

namespace Web.Services.SendEmail
{
    public interface IPreconfiguredEmailService
    {
        bool SendEmail(MailMessage message);
        SmtpServerDetails PrepareServerConfig();
        MailMessage CreatePartialMailMessageFromConfig();
    }
}