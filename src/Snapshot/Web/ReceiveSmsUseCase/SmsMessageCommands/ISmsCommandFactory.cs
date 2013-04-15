using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public interface ISmsCommandFactory
    {
        ISmsMessageCommand CreateSmsMessageCommand(MessageType messageType);
    }
}