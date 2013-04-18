using Web.Models.Parsing;

namespace Web.ReceiveSmsUseCase.Models
{
    public interface ISmsParseResult : IParseResult
    {
        MessageType MessageType { get; set; }
        string Message { get; set; }
    }
}