using Core.Persistence;
using Domain;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services;
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
        private readonly IPreconfiguredEmailService _emailSendingService;
        private readonly IQueryService<RawSmsReceived> _rawSmsReceivedQueryService;
        private IQueryService<Product> _queryProductService;
        private ISaveOrUpdateCommand<ProductSale> _saveProductSale;

        public SmsCommandFactory(IContactMethodsService contactMethodsService, ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand,
                                 IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                 IPreconfiguredEmailService emailSendingService, IQueryService<RawSmsReceived> rawSmsReceivedQueryService, IQueryService<Product> queryProductService, ISaveOrUpdateCommand<ProductSale> saveProductSale)
        {
            _rawSmsReceivedQueryService = rawSmsReceivedQueryService;
            _emailSendingService = emailSendingService;
            _sendSmsService = sendSmsService;
            _contactMethodsService = contactMethodsService;
            _saveOrUpdateAlertCommand = saveOrUpdateAlertCommand;
            _updateStockService = updateStockService;
            _queryProductService = queryProductService;
            _saveProductSale = saveProductSale;
        }

        public ISmsMessageCommand CreateSmsMessageCommand(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Activation:
                    {
                        return new ActivationMessageCommand(_contactMethodsService);
                    }
                case MessageType.StockSale:
                    {
                        return new StockSaleMessageCommand(_updateStockService, _sendSmsService, _saveOrUpdateAlertCommand, _emailSendingService,
                                                           _rawSmsReceivedQueryService,_queryProductService,_saveProductSale);
                    }
                case MessageType.StockCount:
                    {
                        return new StockCountMessageCommand(_updateStockService, _sendSmsService, _saveOrUpdateAlertCommand, _emailSendingService,
                                                            _rawSmsReceivedQueryService);
                    }
                case MessageType.ReceivedStock:
                    {
                        return new StockReceivedMessageCommand(_updateStockService, _sendSmsService, _saveOrUpdateAlertCommand, _emailSendingService,
                                                               _rawSmsReceivedQueryService);
                    }
                default:
                    return new NullObjectCommand();
            }
        }
    }
}