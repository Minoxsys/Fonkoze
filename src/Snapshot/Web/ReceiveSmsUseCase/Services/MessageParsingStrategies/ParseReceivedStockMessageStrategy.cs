using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseReceivedStockMessageStrategy : ParseMessageWithIdentifierAbstractStrategy
    {
        private const string ReceivedMessageIdentifier = "RD";

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