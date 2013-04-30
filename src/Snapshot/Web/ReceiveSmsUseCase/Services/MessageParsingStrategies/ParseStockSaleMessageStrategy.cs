using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseStockSaleMessageStrategy : ISmsParsingStrategy
    {
        private readonly ISmsParsingStrategy _mainMessageContentsStrategy;

        public ParseStockSaleMessageStrategy(ISmsParsingStrategy mainMessageStrategy)
        {
            _mainMessageContentsStrategy = mainMessageStrategy;
        }

        public SmsParseResult Parse(string message)
        {
            var result = _mainMessageContentsStrategy.Parse(message);
            result.MessageType = MessageType.StockSale;
            return result;
        }
    }
}