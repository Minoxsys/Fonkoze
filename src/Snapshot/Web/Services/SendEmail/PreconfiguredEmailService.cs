using System.Net.Mail;
using Web.Services.Configuration;

namespace Web.Services.SendEmail
{
    public class PreconfiguredEmailService : IPreconfiguredEmailService
    {
        private readonly IEmailSendingService _emailService;
        private readonly IConfigurationService _configurationService;

        public PreconfiguredEmailService(IEmailSendingService emailService, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _emailService = emailService;
        }

        public bool SendEmail(MailMessage message, SmtpServerDetails serverDeteails)
        {
            return _emailService.SendEmail(message, serverDeteails);
        }

        public  bool SendEmail(MailMessage message)
        {
            return _emailService.SendEmail(message, PrepareServerConfig());
        }

        public SmtpServerDetails PrepareServerConfig()
        {
            const int defaultPort = 25;
            var serverConfig = new SmtpServerDetails
                {
                    FromAddress = _configurationService[_configurationService.Keys.SendEmailFrom],
                    FromPassword = _configurationService[_configurationService.Keys.SendEmailPassword],
                    Host = _configurationService[_configurationService.Keys.SendEmailHost]
                };
            int portNo;
            serverConfig.Port = int.TryParse(_configurationService[_configurationService.Keys.SendEmailPort], out portNo) ? portNo : defaultPort;
            return serverConfig;
        }

        public MailMessage CreatePartialMailMessageFromConfig()
        {
            var msg = new MailMessage();
            msg.To.Add(new MailAddress(_configurationService[_configurationService.Keys.SendEmailTo]));
            msg.CC.Add(new MailAddress(_configurationService[_configurationService.Keys.SendEmailCc]));
            msg.From = new MailAddress(_configurationService[_configurationService.Keys.SendEmailFrom]);
            msg.IsBodyHtml = false;
            return msg;
        }
    }
}