using System;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Security;
namespace Web.Areas.OutpostManagement.Services
{
    public interface IOutpostsUpdateService
    {
        OutpostsUpdateResult ManageParseOutposts(UserAndClientIdentity loggedUser, IOutpostsParseResult parseResult);
    }
}
