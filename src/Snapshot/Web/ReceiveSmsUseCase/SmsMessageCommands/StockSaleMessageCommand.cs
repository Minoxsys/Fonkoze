using Core.Persistence;
using Domain;
using System.Linq;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class StockSaleMessageCommand: StockUpdateMessageCommandBase
    {
        private IQueryService<Product> _queryProductService;
        private ISaveOrUpdateCommand<ProductSale> _saveProductSale;
        
        public StockSaleMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                        ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IPreconfiguredEmailService emailSendingService,
                                        IQueryService<RawSmsReceived> rawSmsReceived, IQueryService<Product> queryProductService, ISaveOrUpdateCommand<ProductSale> saveProductSale)
            : base(updateStockService, sendSmsService, saveOrUpdateAlertCommand, emailSendingService, rawSmsReceived)
        {
            _queryProductService = queryProductService;
            _saveProductSale = saveProductSale;
            
        }

        internal override void DoAfterStockUpdate(ISmsParseResult parseResult,Outpost outpost)
        {
            foreach (ParsedProduct s in parseResult.ParsedProducts)
            {
                var ps = new ProductSale() { Outpost = outpost,
                    Product = _queryProductService.Query().FirstOrDefault(it => it.SMSReferenceCode == s.ProductCode),
                    Quantity = s.StockLevel,
                    ClientIdentifier = s.ClientIdentifier };

                _saveProductSale.Execute(ps);
            }
        }

        internal override StockUpdateResult UpdateProductStocksForOutpost(ISmsParseResult parseResult, Outpost outpost)
        {
            return UpdateStockService.DecrementProductStocksForOutpost(parseResult, outpost.Id, StockUpdateMethod.SMS);
        }
    }
}