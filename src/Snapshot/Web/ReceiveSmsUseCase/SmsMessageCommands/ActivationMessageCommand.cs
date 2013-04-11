using Domain;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public class ActivationMessageCommand : ISmsMessageCommand
    {
        private readonly IContactMethodsService _contactMethodsService;

        public ActivationMessageCommand(IContactMethodsService contactMethodsService)
        {
            _contactMethodsService = contactMethodsService;
        }

        public void Execute(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost)
        {
            if (parseResult.Success && parseResult.MessageType == MessageType.Activation)
            {
                _contactMethodsService.ActivatePhoneNumber(smsData.Sender, outpost);
            }
        }
    }
}