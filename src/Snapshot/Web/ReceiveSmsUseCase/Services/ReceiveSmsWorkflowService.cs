using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using Web.LocalizationResources;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.SmsMessageCommands;
using Web.Services;
using Web.Services.Helper;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ReceiveSmsWorkflowService : IReceiveSmsWorkflowService
    {
        private readonly ISaveOrUpdateCommand<RawSmsReceived> _saveRawSmsCommand;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISmsTextParserService _smsTextParserService;
        private readonly ISmsCommandFactory _smsCommandFactory;
        private readonly ISenderInformationService _senderInformationService;

        public ReceiveSmsWorkflowService(ISaveOrUpdateCommand<RawSmsReceived> saveRawSmsCommand, ISendSmsService sendSmsService,
                                         ISmsTextParserService smsTextParserService, ISmsCommandFactory smsMessageCommandFactory,
                                         ISenderInformationService senderInformationService)
        {
            _smsCommandFactory = smsMessageCommandFactory;
            _smsTextParserService = smsTextParserService;
            _senderInformationService = senderInformationService;
            _sendSmsService = sendSmsService;
            _saveRawSmsCommand = saveRawSmsCommand;
        }

        public void ProcessSms(ReceivedSmsInputModel smsData)
        {
            //try and identify the sender phone number with an existing phone number assigned to an outpost
            Outpost outpost = _senderInformationService.GetOutpostWithActiveSender(smsData.Sender);
            if (outpost == null)
            {
                outpost = _senderInformationService.GetOutpostWithInactiveSender(smsData.Sender);
                if (outpost == null)
                {
                    SaveRawSmsEntry(smsData, new SmsParseResult {Success = false, Message = Strings.SenderIsUnknown}, null);
                    _sendSmsService.SendSms(smsData.Sender, Strings.PhoneNumberNotRecognized, false);
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
                    OutpostType = outpost != null ? (outpost.IsWarehouse ? OutpostType.Warehouse : OutpostType.Seller) : (OutpostType?) null,
                    OutpostId = outpost != null ? outpost.Id : Guid.Empty
                };

            _saveRawSmsCommand.Execute(rawSms);
        }
    }
}