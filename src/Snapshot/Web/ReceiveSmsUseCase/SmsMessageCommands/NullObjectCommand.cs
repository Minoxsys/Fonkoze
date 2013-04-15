using Domain;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class NullObjectCommand: ISmsMessageCommand
    {
        public void Execute(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost)
        {
           
        }
    }
}