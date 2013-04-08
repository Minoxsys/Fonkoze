using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface IReceiveSmsWorkflowService
    {
        void ProcessSms(ReceivedSmsInputModel smsData);
    }
}
