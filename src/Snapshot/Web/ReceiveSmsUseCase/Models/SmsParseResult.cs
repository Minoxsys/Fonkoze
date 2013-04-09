using System.Collections.Generic;

namespace Web.ReceiveSmsUseCase.Models
{
    public class SmsParseResult
    {
        public SmsParseResult()
        {
            ParsedProducts = new List<ParsedProduct>();
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ParsedProduct> ParsedProducts { get; set; }
    }
}
