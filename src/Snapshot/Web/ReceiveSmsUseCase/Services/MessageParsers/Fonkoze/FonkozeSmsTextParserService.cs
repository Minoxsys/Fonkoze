using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze;

namespace Web.ReceiveSmsUseCase.Services.MessageParsers.Fonkoze
{
    public class FonkozeSmsTextParserService : ISmsTextParserService
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

            var parseStockCountMessageStrategyWithGroup =
                new ParseStockCountMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsAndTwoLetterProductCodeStrategy());
            parseResult = parseStockCountMessageStrategyWithGroup.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseReceivedMessageStrategyWithGroup =
                new ParseReceivedStockMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsAndTwoLetterProductCodeStrategy());
            parseResult = parseReceivedMessageStrategyWithGroup.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockUpdateMessageStrategyWithGroup =
                new ParseStockSaleMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsAndTwoLetterProductCodeStrategy());
            parseResult = parseStockUpdateMessageStrategyWithGroup.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockCountMessageStrategy = new ParseStockCountMessageStrategy(new ParseFonkozeMainMessageContentsStrategy());
            parseResult = parseStockCountMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseReceivedMessageStrategy = new ParseReceivedStockMessageStrategy(new ParseFonkozeMainMessageContentsStrategy());
            parseResult = parseReceivedMessageStrategy.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockUpdateMessageStrategy = new ParseStockSaleMessageStrategy(new ParseFonkozeMainMessageContentsStrategy());
            parseResult = parseStockUpdateMessageStrategy.Parse(message);
            return parseResult;
        }
    }
}