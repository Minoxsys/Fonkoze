using System.Net.Mail;
using Core.Persistence;
using Domain;
using Web.Services.Configuration;

namespace Web.Services.SendEmail
{
    public class PreconfiguredEmailService : IPreconfiguredEmailService
    {
        private readonly IEmailSendingService _emailService;
        private readonly IConfigurationService _configurationService;
        private readonly ISaveOrUpdateCommand<ApplicationActivity> _appActivitySaveCmd;

        public PreconfiguredEmailService(IEmailSendingService emailService, IConfigurationService configurationService,
                                         ISaveOrUpdateCommand<ApplicationActivity> appActivitySaveCmd)
        {
            _appActivitySaveCmd = appActivitySaveCmd;
            _configurationService = configurationService;
            _emailService = emailService;
        }

        public  bool SendEmail(MailMessage message)
        {
            if (_emailService.SendEmail(message, PrepareServerConfig()))
            {
                _appActivitySaveCmd.Execute(new ApplicationActivity { Message = string.Format("M")});
                return true;
            }
            return false;
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