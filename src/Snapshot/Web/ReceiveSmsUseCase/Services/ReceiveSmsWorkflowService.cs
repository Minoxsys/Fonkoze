using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ReceiveSmsWorkflowService: IReceiveSmsWorkflowService
    {
        private readonly ISaveOrUpdateCommand<RawSmsReceived> _saveRawSmsCommand;
        private readonly ISendSmsService _sendSmsService;
        private readonly IQueryService<Outpost> _outpostsQueryService;
        private readonly IQueryService<Contact> _contactsQueryService;
        private readonly ISmsTextParserService _smsTextParserService;
        private readonly IUpdateStockService _updateStockService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;

        public ReceiveSmsWorkflowService(ISaveOrUpdateCommand<RawSmsReceived> saveRawSmsCommand, ISendSmsService sendSmsService,
                                 IQueryService<Outpost> outpostsQueryService, IQueryService<Contact> contactsQueryService,
                                 ISmsTextParserService smsTextParserService, IUpdateStockService updateStockService,
                                 ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand)
        {
            _saveOrUpdateAlertCommand = saveOrUpdateAlertCommand;
            _updateStockService = updateStockService;
            _smsTextParserService = smsTextParserService;
            _contactsQueryService = contactsQueryService;
            _outpostsQueryService = outpostsQueryService;
            _sendSmsService = sendSmsService;
            _saveRawSmsCommand = saveRawSmsCommand;
        }

        public void ProcessSms(ReceivedSmsInputModel smsData)
        {
            var rawSms = new RawSmsReceived
                {
                    Sender = smsData.Sender,
                    Content = smsData.Content,
                    Credits = smsData.Credits,
                    ReceivedDate = DateTime.UtcNow
                };

            rawSms.OutpostId = GetOutpostForSender(rawSms.Sender);
            if (IsUnknownOutpost(rawSms.OutpostId))
            {
                _saveRawSmsCommand.Execute(rawSms);
                _sendSmsService.SendSmsMessage("Phone number not recognized. Please activate your phone number to send messages.", rawSms.Sender);
                return;
            }

            SmsParseResult parseResult = _smsTextParserService.Parse(rawSms.Content);

            rawSms.ParseSucceeded = parseResult.Success;
            rawSms.ParseErrorMessage = parseResult.Message;
            _saveRawSmsCommand.Execute(rawSms);

            if (rawSms.ParseSucceeded)
            {
                _updateStockService.UpdateProductStocks(parseResult);
            }
            else
            {
                var outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Id == rawSms.OutpostId);
                Debug.Assert(outpost != null, "outpost != null");
                var alert = new Alert
                    {
                        AlertType = AlertType.Error,
                        Client = outpost.Client,
                        OutpostId = rawSms.OutpostId,
                        Contact = rawSms.Sender,
                        OutpostName = outpost.Name,
                        ProductGroupName = "-",
                        LowLevelStock = "-",
                        LastUpdate = null
                    };
                _saveOrUpdateAlertCommand.Execute(alert);
            }
        }

        private bool IsUnknownOutpost(Guid outpostId)
        {
            return outpostId == Guid.Empty;
        }

        private Guid GetOutpostForSender(string senderPhoneNumber)
        {
            Contact contact = _contactsQueryService.Query().FirstOrDefault(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(senderPhoneNumber));
            Outpost outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Contacts.Contains(contact));

            return outpost != null ? outpost.Id : Guid.Empty;
        }
    }
}