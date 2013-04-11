using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ReceiveSmsWorkflowService : IReceiveSmsWorkflowService
    {
        private readonly ISaveOrUpdateCommand<RawSmsReceived> _saveRawSmsCommand;
        private readonly ISendSmsService _sendSmsService;
        private readonly IQueryService<Outpost> _outpostsQueryService;
        private readonly IQueryService<Contact> _contactsQueryService;
        private readonly ISmsTextParserService _smsTextParserService;
        private readonly IUpdateStockService _updateStockService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IContactMethodsService _contactMethodsService;

        public ReceiveSmsWorkflowService(ISaveOrUpdateCommand<RawSmsReceived> saveRawSmsCommand, ISendSmsService sendSmsService,
                                         IQueryService<Outpost> outpostsQueryService, IQueryService<Contact> contactsQueryService,
                                         ISmsTextParserService smsTextParserService, IUpdateStockService updateStockService,
                                         ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IContactMethodsService contactMethodsService)
        {
            _contactMethodsService = contactMethodsService;
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

            //try and identify the sender phone number with an existing phone number assigned to an outpost
            bool isActiveSender = true;
            rawSms.OutpostId = GetOutpostWithActiveSender(rawSms.Sender);
            if (IsUnknownOutpost(rawSms.OutpostId))
            {
                isActiveSender = false;
                rawSms.OutpostId = GetOutpostWithInactiveSender(rawSms.Sender);
                if (IsUnknownOutpost(rawSms.OutpostId))
                {
                    rawSms.ParseSucceeded = false;
                    rawSms.ParseErrorMessage = "Sender is unknown.";
                    _saveRawSmsCommand.Execute(rawSms);
                    _sendSmsService.SendSmsMessage("Phone number not recognized. Please register your phone number to send messages.", rawSms.Sender);
                    return;
                }
            }

            ISmsParseResult parseResult = _smsTextParserService.Parse(rawSms.Content);

            rawSms.ParseSucceeded = parseResult.Success;
            rawSms.ParseErrorMessage = parseResult.Message;

            var outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Id == rawSms.OutpostId);
            Debug.Assert(outpost != null, "outpost != null");
            rawSms.OutpostType = outpost.IsWarehouse ? OutpostType.Warehouse : OutpostType.Seller;

            _saveRawSmsCommand.Execute(rawSms);

            if (parseResult.Success && parseResult.MessageType == MessageType.Activation)
            {
                _contactMethodsService.ActivatePhoneNumber(smsData.Sender, outpost);
                return;
            }

            if (rawSms.ParseSucceeded)
            {
                if (isActiveSender)
                {
                    _updateStockService.UpdateProductStocksForOutpost(parseResult, rawSms.OutpostId);
                }
                else
                {
                    _sendSmsService.SendSmsMessage("Phone number not active. Please activate your phone number to send update stock messages.", rawSms.Sender);
                }
            }
            else
            {
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

        private Guid GetOutpostWithActiveSender(string senderPhoneNumber)
        {
            return OutpostWithActiveSender(senderPhoneNumber, true);
        }

        private Guid GetOutpostWithInactiveSender(string senderPhoneNumber)
        {
            return OutpostWithActiveSender(senderPhoneNumber, false);
        }

        private Guid OutpostWithActiveSender(string senderPhoneNumber, bool activeSender)
        {
            Contact contact = _contactsQueryService.Query().FirstOrDefault(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(senderPhoneNumber) && c.IsMainContact == activeSender);
            Outpost outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Contacts.Contains(contact));

            return outpost != null ? outpost.Id : Guid.Empty;
        }
    }
}