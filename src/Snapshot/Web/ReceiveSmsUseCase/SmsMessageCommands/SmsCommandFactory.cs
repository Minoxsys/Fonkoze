using Core.Persistence;
using Domain;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class SmsCommandFactory : ISmsCommandFactory
    {
        private readonly IUpdateStockService _updateStockService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IContactMethodsService _contactMethodsService;
        private readonly ISendSmsService _sendSmsService;
        private readonly IQueryService<Alert> _alertQueryService;
        private readonly IPreconfiguredEmailService _emailSendingService;

        public SmsCommandFactory(IContactMethodsService contactMethodsService, ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand,
                                 IUpdateStockService updateStockService, ISendSmsService sendSmsService, IQueryService<Alert> alertQueryService,
                                 IPreconfiguredEmailService emailSendingService)
        {
            _emailSendingService = emailSendingService;
            _alertQueryService = alertQueryService;
            _sendSmsService = sendSmsService;
            _contactMethodsService = contactMethodsService;
            _saveOrUpdateAlertCommand = saveOrUpdateAlertCommand;
            _updateStockService = updateStockService;
        }

        public ISmsMessageCommand CreateSmsMessageCommand(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Activation:
                    {
                        return new ActivationMessageCommand(_contactMethodsService);
                    }
                case MessageType.StockUpdate:
                    {
                        return new UpdateStockMessageCommand(_updateStockService, _sendSmsService, _saveOrUpdateAlertCommand, _emailSendingService, _alertQueryService);
                    }
                default:
                    return new NullObjectCommand();
            }
        }
    }
}