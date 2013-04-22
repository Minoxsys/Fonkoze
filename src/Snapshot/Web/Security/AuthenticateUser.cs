using Core.Domain;
using Core.Persistence;
using Core.Services;
using System.Linq;

namespace Web.Security
{
    public class AuthenticateUser : IMembershipService
    {
        private readonly IQueryService<User> _queryUsers;
        private readonly ISecurePassword _securePassword;

        public AuthenticateUser(IQueryService<User> queryUsers, ISecurePassword securePassword)
        {
            _queryUsers = queryUsers;
            _securePassword = securePassword;
        }

        public bool ValidateUser(string userName, string password)
        {
            string securedPassword = _securePassword.EncryptPassword(password);
            var user = _queryUsers.Query().Where(it => it.UserName == userName && it.Password == securedPassword);

            return user.Count() != 0;
        }
    }
}