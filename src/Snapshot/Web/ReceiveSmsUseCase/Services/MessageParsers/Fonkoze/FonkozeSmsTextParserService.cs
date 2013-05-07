using System;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies;
using Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze;

namespace Web.ReceiveSmsUseCase.Services.MessageParsers.Fonkoze
{
    public class FonkozeSmsTextParserService : ISmsTextParserService
    {
        private readonly MessageParsingHelpers _parsingHelper = new MessageParsingHelpers();
        private const string StockCountMessageIdentifier = "SC";
        private const string ReceivedStockMessageIdentifier = "RD";

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

            var tokens = message.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (string.Compare(tokens[0], ReceivedStockMessageIdentifier, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parseReceivedMessageStrategyWithGroup =
                    new ParseReceivedStockMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsNoClientIdentifierTwoLetterProductCodeStrategy());
                parseResult = parseReceivedMessageStrategyWithGroup.Parse(message);
                if (parseResult.Success)
                    return parseResult;

                var parseReceivedMessageStrategy =
                    new ParseReceivedStockMessageStrategy(new ParseFonkozeMainMessageNoGroupsNoClientIdentifierTwoLetterProductCodeStrategy());
                parseResult = parseReceivedMessageStrategy.Parse(message);
                return parseResult;
            }

            if (string.Compare(tokens[0], StockCountMessageIdentifier, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parseStockCountMessageStrategyWithGroup =
                    new ParseStockCountMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsNoClientIdentifierTwoLetterProductCodeStrategy());
                parseResult = parseStockCountMessageStrategyWithGroup.Parse(message);
                if (parseResult.Success)
                    return parseResult;

                var parseStockCountMessageStrategy =
                    new ParseStockCountMessageStrategy(new ParseFonkozeMainMessageNoGroupsNoClientIdentifierTwoLetterProductCodeStrategy());
                parseResult = parseStockCountMessageStrategy.Parse(message);
                return parseResult;
            }

            var parseStockUpdateMessageStrategyWithGroup =
                new ParseStockSaleMessageStrategy(new ParseFonkozeMainMessageContentsWithGroupsAndTwoLetterProductCodeStrategy());
            parseResult = parseStockUpdateMessageStrategyWithGroup.Parse(message);
            if (parseResult.Success)
                return parseResult;

            var parseStockUpdateMessageStrategy = new ParseStockSaleMessageStrategy(new ParseFonkozeMainMessageContentsStrategy());
            parseResult = parseStockUpdateMessageStrategy.Parse(message);
            return parseResult;
        }
    }
}