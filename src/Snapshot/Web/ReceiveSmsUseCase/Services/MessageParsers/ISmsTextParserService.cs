using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsers
{
    public interface ISmsTextParserService
    {
        SmsParseResult Parse(string message);
    }
}
