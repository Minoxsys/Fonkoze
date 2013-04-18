using Core.Persistence;
using Domain;
using Domain.Enums;
using System.Linq;
using System.Net.Mail;
using Web.LocalizationResources;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class UpdateStockMessageCommand : ISmsMessageCommand
    {
        private readonly IUpdateStockService _updateStockService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IPreconfiguredEmailService _emailSendingService;
        private readonly IQueryService<Alert> _alertQueryService;

        public UpdateStockMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                         ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IPreconfiguredEmailService emailSendingService, IQueryService<Alert> alertQueryService)
        {
            _alertQueryService = alertQueryService;
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
                    _sendSmsService.SendSmsMessage(Strings.PhoneNumberNotActive, smsData.Sender);
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
                    _emailSendingService.SendEmail(msg);
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

        private MailMessage CreateMailMessage(ReceivedSmsInputModel smsData, Outpost outpost)
        {
            var msg = _emailSendingService.CreatePartialMailMessageFromConfig();
            msg.Subject = Strings.IncorrectSMSAlertSubject;
            msg.Body = string.Format(Strings.TwoConsecutiveInvalidSMSEmailBody, outpost.Name,
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
