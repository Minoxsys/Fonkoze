using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    interface ISmsParsingStrategy
    {
        SmsParseResult Parse(string message);
    }
}
