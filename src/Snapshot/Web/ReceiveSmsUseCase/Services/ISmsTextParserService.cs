using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface ISmsTextParserService
    {
        SmsParseResult Parse(string message);
    }
}
