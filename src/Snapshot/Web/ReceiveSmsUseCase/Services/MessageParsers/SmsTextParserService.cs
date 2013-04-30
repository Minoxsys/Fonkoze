using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies;

namespace Web.ReceiveSmsUseCase.Services.MessageParsers
{
    public class SmsTextParserService : ISmsTextParserService
    {
        private readonly MessageParsingHelpers _parsingHelper = new MessageParsingHelpers();

        public SmsParseResult Parse(string message)
        {
            message = message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return _parsingHelper.CreateInvalidMessageFormatResponse();
            }

            var activationMessageStrategy = new ParseActivationMessageStrategy();
            var parseResult = activationMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockCountMessageStrategy = new ParseStockCountMessageStrategy(new ParseMainMessageContentsStrategy());
            parseResult = parseStockCountMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseReceivedMessageStrategy = new ParseReceivedStockMessageStrategy(new ParseMainMessageContentsStrategy());
            parseResult = parseReceivedMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult; 

            var parseStockUpdateMessageStrategy = new ParseStockSaleMessageStrategy(new ParseMainMessageContentsStrategy());
            parseResult = parseStockUpdateMessageStrategy.Parse(message);
            return parseResult;
        }
    }
}