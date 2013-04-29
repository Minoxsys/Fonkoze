using Web.LocalizationResources;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class MessageParsingHelpers
    {
        public SmsParseResult CreateErrorMessage(string msg)
        {
            return new SmsParseResult {Message = msg};
        }

        public SmsParseResult CreateInvalidMessageFormatResponse()
        {
            return CreateErrorMessage(Strings.InvalidMessageFormat);
        }
    }
}