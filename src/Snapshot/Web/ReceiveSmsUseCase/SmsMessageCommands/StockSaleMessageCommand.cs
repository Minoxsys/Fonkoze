using Core.Persistence;
using Domain;
using System.Linq;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;
using System.Collections.Generic;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class StockSaleMessageCommand : StockUpdateMessageCommandBase
    {
        private readonly IQueryService<Product> _queryProductService;
        private readonly ISaveOrUpdateCommand<ProductSale> _saveProductSale;

        public StockSaleMessageCommand(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                       ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IPreconfiguredEmailService emailSendingService,
                                       IQueryService<RawSmsReceived> rawSmsReceived, IQueryService<Product> queryProductService,
                                       ISaveOrUpdateCommand<ProductSale> saveProductSale, ISaveOrUpdateCommand<RawSmsReceived> updateRawSmsReceived)
            : base(updateStockService, sendSmsService, saveOrUpdateAlertCommand, emailSendingService, rawSmsReceived, updateRawSmsReceived)
        {
            _queryProductService = queryProductService;
            _saveProductSale = saveProductSale;

        }

        internal override void DoAfterStockUpdate(List<IParsedProduct> parsedProducts,List<IParsedProduct> failedProducts, Outpost outpost)
        {
            List<IParsedProduct> successfulProducts = new List<IParsedProduct>();
            if (failedProducts != null)
            {
                foreach (var p in parsedProducts)
                {
                    if (failedProducts.FirstOrDefault(it => it.ProductCode == p.ProductCode) == null)
                    {
                        successfulProducts.Add(p);
                    }
                }
            }
            else
                successfulProducts = parsedProducts;

            foreach (ParsedProduct s in successfulProducts)
            {
                var ps = new ProductSale
                    {
                        Outpost = outpost,
                        Product = _queryProductService.Query().FirstOrDefault(it => it.SMSReferenceCode == s.ProductCode),
                        Quantity = s.StockLevel,
                        ClientIdentifier = s.ClientIdentifier
                    };

                _saveProductSale.Execute(ps);
            }
        }

        internal override StockUpdateResult UpdateProductStocksForOutpost(ISmsParseResult parseResult, Outpost outpost)
        {
            return UpdateStockService.DecrementProductStocksForOutpost(parseResult, outpost.Id, StockUpdateMethod.SMS);
        }
    }
}