using System.Collections.Generic;

namespace Web.ReceiveSmsUseCase.Models
{
    public class SmsParseResult : ISmsParseResult
    {
        public SmsParseResult()
        {
            ParsedProducts = new List<IParsedProduct>();
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public List<IParsedProduct> ParsedProducts { get; set; }
    }
}
