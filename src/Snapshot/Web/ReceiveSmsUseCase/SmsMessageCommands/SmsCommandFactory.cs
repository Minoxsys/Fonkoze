using Core.Persistence;
using Domain;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class SmsCommandFactory
    {
        private readonly IUpdateStockService _updateStockService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IContactMethodsService _contactMethodsService;
        private readonly ISendSmsService _sendSmsService;

        public SmsCommandFactory(IContactMethodsService contactMethodsService, ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand,
                                 IUpdateStockService updateStockService, ISendSmsService sendSmsService)
        {
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
                        return new UpdateStockMessageCommand(_updateStockService, _sendSmsService, _saveOrUpdateAlertCommand);
                    }
            }
            return null;
        }
    }
}