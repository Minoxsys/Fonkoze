using System.Collections.Generic;

namespace Web.ReceiveSmsUseCase.Models
{
    public interface ISmsParseResult
    {
        MessageType MessageType { get; set; }
        bool Success { get; set; }
        string Message { get; set; }
        List<IParsedProduct> ParsedProducts { get; set; }
    }
}