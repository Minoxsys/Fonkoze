using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{

    public interface ISmsParsingStrategy
    {
        SmsParseResult Parse(string message);
    }
}
