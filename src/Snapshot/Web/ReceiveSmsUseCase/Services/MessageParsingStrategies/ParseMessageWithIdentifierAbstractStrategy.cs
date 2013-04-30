using System;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public abstract class ParseMessageWithIdentifierAbstractStrategy : ISmsParsingStrategy
    {
        protected abstract string MessageIdentifier { get; }
        protected abstract MessageType MessageTypeReturned { get; }

        protected readonly ISmsParsingStrategy MainMessageContentsStrategy;
        private readonly MessageParsingHelpers _parsingHelper = new MessageParsingHelpers();

        protected ParseMessageWithIdentifierAbstractStrategy(ISmsParsingStrategy mainMessageContentsStrategy)
        {
            MainMessageContentsStrategy = mainMessageContentsStrategy;
        }

        public SmsParseResult Parse(string message)
        {
            var tokens = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (string.Compare(tokens[0], MessageIdentifier, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parseResult = MainMessageContentsStrategy.Parse(RemoveMessageIdentifier(message));
                parseResult.MessageType = MessageTypeReturned;
                return parseResult;
            }
            return _parsingHelper.CreateInvalidMessageFormatResponse();
        }

        private string RemoveMessageIdentifier(string message)
        {
            return message.Substring(2);
        }
    }
}