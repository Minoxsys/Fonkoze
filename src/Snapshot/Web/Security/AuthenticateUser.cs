using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Services;
using Core.Domain;
using Core.Persistence;

namespace Web.Security
{
    public class AuthenticateUser : IMembershipService
    {
        private IQueryService<User> queryUsers;
        private ISecurePassword SecurePassword;
                
        public AuthenticateUser(IQueryService<User> queryUsers,ISecurePassword SecurePassword)
        {
            this.queryUsers = queryUsers;
            this.SecurePassword = SecurePassword;
 
        }
        public bool ValidateUser(string userName,string password)
        {
            var user = queryUsers.Query().Where(it => it.UserName == userName && it.Password == SecurePassword.EncryptPassword(password));

            if (user.ToList().Count != 0)
                return true;
            else
                return false;
        }
    }
}