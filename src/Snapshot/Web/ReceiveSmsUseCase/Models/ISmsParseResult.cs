using System.Collections.Generic;

namespace Web.ReceiveSmsUseCase.Models
{
    public interface ISmsParseResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        List<IParsedProduct> ParsedProducts { get; set; }
    }
}