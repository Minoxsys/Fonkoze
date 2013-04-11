using Core.Persistence;
using Domain;
using Domain.Enums;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class UpdateStockMessageCommand : ISmsMessageCommand
    {
        private readonly IUpdateStockService _updateStockService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;

        public UpdateStockMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                         ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand)
        {
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
        }

        private bool IsActiveSender(Outpost outpost, string senderPhoneNumber)
        {
            return
                outpost.Contacts.FirstOrDefault(
                    c => c.ContactDetail.Contains(senderPhoneNumber) && c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE && c.IsMainContact) != null;
        }
    }
}