using Domain;

namespace Web.ReceiveSmsUseCase.Services
{
    public interface IContactMethodsService
    {
        void ActivatePhoneNumber(string sender, Outpost outpost);
    }
}