using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies;

namespace Web.ReceiveSmsUseCase.Services
{
    public class SmsTextParserService : ISmsTextParserService
    {
        private readonly MessageParsingHelpers _parsingHelper = new MessageParsingHelpers();
        private const int ValidMessageMinimumLength = 6;

        public SmsParseResult Parse(string message)
        {
            message = message.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return _parsingHelper.CreateInvalidMessageFormatResponse();
            }
            if (message.Length < ValidMessageMinimumLength)
            {
                return _parsingHelper.CreateInvalidMessageFormatResponse();
            }

            var activationMessageStrategy = new ParseActivationMessageStrategy();
            var parseResult = activationMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockCountMessageStrategy = new ParseStockCountMessageStrategy();
            parseResult = parseStockCountMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseReceivedMessageStrategy = new ParseReceivedStockMessageStrategy();
            parseResult = parseReceivedMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult; 

            var parseStockUpdateMessageStrategy = new ParseStockSaleMessageStrategy();
            parseResult = parseStockUpdateMessageStrategy.Parse(message);
            return parseResult;
        }
    }
}