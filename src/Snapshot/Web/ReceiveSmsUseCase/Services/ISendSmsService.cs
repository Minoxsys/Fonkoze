namespace Web.ReceiveSmsUseCase.Services
{
    public interface ISendSmsService
    {
        void SendSmsMessage(string message, string sender);
    }
}
