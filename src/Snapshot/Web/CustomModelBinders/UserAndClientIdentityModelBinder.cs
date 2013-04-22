using Core.Domain;
using Core.Persistence;
using Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Web.Security;

namespace Web.CustomModelBinders
{
    public class UserAndClientIdentityModelBinder : IModelBinder
    {
        private readonly IQueryService<Client> _clientsQueryService;
        private readonly IQueryService<User> _usersQueryService;

        public UserAndClientIdentityModelBinder(IQueryService<User> usersQueryService, IQueryService<Client> clientsQueryService )
        {
            _usersQueryService = usersQueryService;
            _clientsQueryService = clientsQueryService;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var loggedUser = controllerContext.HttpContext.User.Identity.Name;
            var user = _usersQueryService.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (user.ClientId != Guid.Empty)
                clientId = user.ClientId;

            var client = _clientsQueryService.Load(clientId);
            return new UserAndClientIdentity {Client = client, User = user};
        }
    }
}