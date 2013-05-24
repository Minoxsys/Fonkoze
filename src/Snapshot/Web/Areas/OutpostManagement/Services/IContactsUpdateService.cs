using System;
using Domain;
using Web.Models.Parsing.Outpost;
using Web.Security;
namespace Web.Areas.OutpostManagement.Services
{
    public interface IContactsUpdateService
    {
        void AddContact(UserAndClientIdentity loggedUser, Outpost outpost, IParsedOutpost parsedOutpost);
        void ManageOutpostContact(UserAndClientIdentity loggedUser, Outpost existentOutpost, IParsedOutpost parsedOutpost);
    }
}
