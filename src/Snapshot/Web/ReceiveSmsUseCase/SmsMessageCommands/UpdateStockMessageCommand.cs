using Core.Persistence;
using Domain;
using Domain.Enums;
using System.Linq;
using System.Net.Mail;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services.Configuration;
using Web.Services.SendEmail;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class UpdateStockMessageCommand : ISmsMessageCommand
    {
        private readonly IUpdateStockService _updateStockService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IEmailSendingService _emailSendingService;
        private readonly IConfigurationService _configurationService;
        private readonly IQueryService<Alert> _alertQueryService;

        public UpdateStockMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                         ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IEmailSendingService emailSendingService,
                                         IConfigurationService configurationService, IQueryService<Alert> alertQueryService)
        {
            _alertQueryService = alertQueryService;
            _configurationService = configurationService;
            _emailSendingService = emailSendingService;
            _updateStockService = updateStockService;
            _sendSmsService = sendSmsService;
            _saveOrUpdateAlertCommand = saveOrUpdateAlertCommand;
        }

        public void Execute(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost)
        {
            if (parseResult.Success)
            {
                if (IsActiveSender(outpost, smsData.Sender))
                {
                    _updateStockService.UpdateProductStocksForOutpost(parseResult, outpost.Id);
                }
                else
                {
                    _sendSmsService.SendSmsMessage("Phone number not active. Please activate your phone number to send update stock messages.", smsData.Sender);
                }
            }
            else
            {
                // get the latest error alert from the same sender if any
                var previousWrongAlert =
                    _alertQueryService.Query()
                                      .OrderByDescending(a => a.Created)
                                      .FirstOrDefault(a => a.AlertType == AlertType.Error && a.OutpostName == outpost.Name && a.Contact == smsData.Sender);

                if (previousWrongAlert != null)
                {
                    var msg = CreateMailMessage(smsData, outpost);
                    var serverConfig = PrepareServerConfig();

                    _emailSendingService.SendEmail(msg, serverConfig);
                }
            }

            var alert = new Alert
                {
                    AlertType = AlertType.Error,
                    Client = outpost.Client,
                    OutpostId = outpost.Id,
                    Contact = smsData.Sender,
                    OutpostName = outpost.Name,
                    ProductGroupName = "-",
                    LowLevelStock = "-",
                    LastUpdate = null
                };
            _saveOrUpdateAlertCommand.Execute(alert);
        }

        private SmtpServerDetails PrepareServerConfig()
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

        private MailMessage CreateMailMessage(ReceivedSmsInputModel smsData, Outpost outpost)
        {
            var msg = new MailMessage();
            msg.To.Add(new MailAddress(_configurationService[_configurationService.Keys.SendEmailTo]));
            msg.CC.Add(new MailAddress(_configurationService[_configurationService.Keys.SendEmailCc]));
            msg.From = new MailAddress(_configurationService[_configurationService.Keys.SendEmailFrom]);
            msg.Subject = "Incorrect SMS alert";
            msg.IsBodyHtml = false;
            msg.Body = string.Format("Seller {0} with phone number {1} has send the second consecutive invalid SMS. Please assist.", outpost.Name,
                                     smsData.Sender);
            return msg;
        }

        private bool IsActiveSender(Outpost outpost, string senderPhoneNumber)
        {
            return
                outpost.Contacts.FirstOrDefault(
                    c => c.ContactDetail.Contains(senderPhoneNumber) && c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE && c.IsMainContact) != null;
        }
    }
}
