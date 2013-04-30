using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseStockCountMessageStrategy : ParseMessageWithIdentifierAbstractStrategy
    {
        private const string StockCountMessageIdentifier = "SC";

        public ParseStockCountMessageStrategy(ISmsParsingStrategy mainMessageStrategy)
            : base(mainMessageStrategy)
        {

        }

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