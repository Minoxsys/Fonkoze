using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class SmsTextParserService : ISmsTextParserService
    {
        public SmsParseResult Parse(string message)
        {
            return new SmsParseResult();
        }
    }
}