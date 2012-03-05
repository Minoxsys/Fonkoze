using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;

namespace Web.Models.UserManager
{
    public class UserOutputModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? RoleId { get; set; }
        public string ClientName { get; set; }
        public string RoleName { get; set; }

        public IList<Role> Roles { get; set; }

        public UserOutputModel()
        {
            Roles = new List<Role>();
        }
    }
}