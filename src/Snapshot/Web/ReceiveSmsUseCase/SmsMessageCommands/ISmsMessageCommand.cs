using Domain;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public interface ISmsMessageCommand
    {
        void Execute(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost);
    }
}