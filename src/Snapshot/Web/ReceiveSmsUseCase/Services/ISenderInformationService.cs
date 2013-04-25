using Domain;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface ISenderInformationService
    {
        Outpost GetOutpostWithActiveSender(string senderPhoneNumber);
        Outpost GetOutpostWithInactiveSender(string senderPhoneNumber);
    }
}