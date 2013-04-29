using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseStockCountMessageStrategy : ParseMessageWithIdentifierAbstractStrategy
    {
        private const string StockCountMessageIdentifier = "SC";

        protected override string MessageIdentifier
        {
            get { return StockCountMessageIdentifier; }
        }

        protected override MessageType MessageTypeReturned
        {
            get { return MessageType.StockCount; }
        }
    }
}