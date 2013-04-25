using Domain;

namespace Web.Services.Helper
{
    public interface ISenderInformationService
    {
        Outpost GetOutpostWithActiveSender(string senderPhoneNumber);
        Outpost GetOutpostWithInactiveSender(string senderPhoneNumber);
    }
}