using Core.Persistence;
using Domain;
using Web.ReceiveSmsUseCase.Models;
using Web.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class StockReceivedMessageCommand : StockUpdateMessageCommandBase
    {
        public StockReceivedMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                           ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IPreconfiguredEmailService emailSendingService,
                                           IQueryService<RawSmsReceived> rawSmsReceived)
            : base(updateStockService, sendSmsService, saveOrUpdateAlertCommand, emailSendingService, rawSmsReceived)
        {
        }

        internal override StockUpdateResult UpdateProductStocksForOutpost(ISmsParseResult parseResult, Outpost outpost)
        {
            return UpdateStockService.IncrementProductStocksForOutpost(parseResult, outpost.Id, StockUpdateMethod.SMS);
        }
    }
}