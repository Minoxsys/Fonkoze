using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseReceivedStockMessageStrategy : ParseMessageWithIdentifierAbstractStrategy
    {
        private const string ReceivedMessageIdentifier = "RD";

        public ParseReceivedStockMessageStrategy(ISmsParsingStrategy mainMessageStrategy)
            : base(mainMessageStrategy)
        {

        }

        protected override string MessageIdentifier
        {
            get { return ReceivedMessageIdentifier; }
        }

        protected override MessageType MessageTypeReturned
        {
            get { return MessageType.ReceivedStock; }
        }
    }
}