using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.SmsMessageCommands;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ReceiveSmsWorkflowService : IReceiveSmsWorkflowService
    {
        private readonly ISaveOrUpdateCommand<RawSmsReceived> _saveRawSmsCommand;
        private readonly ISendSmsService _sendSmsService;
        private readonly IQueryService<Outpost> _outpostsQueryService;
        private readonly IQueryService<Contact> _contactsQueryService;
        private readonly ISmsTextParserService _smsTextParserService;
        private readonly ISmsCommandFactory _smsCommandFactory;

        public ReceiveSmsWorkflowService(ISaveOrUpdateCommand<RawSmsReceived> saveRawSmsCommand, ISendSmsService sendSmsService,
                                         IQueryService<Outpost> outpostsQueryService, IQueryService<Contact> contactsQueryService,
                                         ISmsTextParserService smsTextParserService, ISmsCommandFactory smsMessageCommandFactory)
        {
            _smsCommandFactory = smsMessageCommandFactory;
            //new SmsCommandFactory(contactMethodsService, saveOrUpdateAlertCommand, updateStockService, sendSmsService, null);
            _smsTextParserService = smsTextParserService;
            _contactsQueryService = contactsQueryService;
            _outpostsQueryService = outpostsQueryService;
            _sendSmsService = sendSmsService;
            _saveRawSmsCommand = saveRawSmsCommand;
        }

        public void ProcessSms(ReceivedSmsInputModel smsData)
        {
            //try and identify the sender phone number with an existing phone number assigned to an outpost
            Outpost outpost = GetOutpostWithActiveSender(smsData.Sender);
            if (outpost == null)
            {
                outpost = GetOutpostWithInactiveSender(smsData.Sender);
                if (outpost == null)
                {
                    SaveRawSmsEntry(smsData, new SmsParseResult {Success = false, Message = "Sender is unknown."}, null);
                    _sendSmsService.SendSmsMessage("Phone number not recognized. Please register your phone number to send messages.", smsData.Sender);
                    return;
                }
            }

            ISmsParseResult parseResult = _smsTextParserService.Parse(smsData.Content);

            SaveRawSmsEntry(smsData, parseResult, outpost);

            ISmsMessageCommand smsCommand = _smsCommandFactory.CreateSmsMessageCommand(parseResult.MessageType);
            smsCommand.Execute(smsData, parseResult, outpost);
        }

        private void SaveRawSmsEntry(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost)
        {
            var rawSms = new RawSmsReceived
                {
                    Sender = smsData.Sender,
                    Content = smsData.Content,
                    Credits = smsData.Credits,
                    ReceivedDate = DateTime.UtcNow,
                    ParseSucceeded = parseResult.Success,
                    ParseErrorMessage = parseResult.Message,
                    OutpostType = outpost != null ? (outpost.IsWarehouse ? OutpostType.Warehouse : OutpostType.Seller) : (OutpostType?) null
                };

            _saveRawSmsCommand.Execute(rawSms);
        }

        private Outpost GetOutpostWithActiveSender(string senderPhoneNumber)
        {
            return FindMatchingOutpostForSender(senderPhoneNumber, true);
        }

        private Outpost GetOutpostWithInactiveSender(string senderPhoneNumber)
        {
            return FindMatchingOutpostForSender(senderPhoneNumber, false);
        }

        private Outpost FindMatchingOutpostForSender(string senderPhoneNumber, bool activeSender)
        {
            Contact contact = _contactsQueryService.Query().FirstOrDefault(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(senderPhoneNumber) && c.IsMainContact == activeSender);
            Outpost outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Contacts.Contains(contact));
            return outpost;
        }
    }
}