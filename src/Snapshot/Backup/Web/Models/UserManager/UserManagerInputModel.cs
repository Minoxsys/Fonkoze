using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.UserManager
{
    public class UserManagerInputModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Guid RoleId { get; set; }
        public Guid ClientId { get; set; }


    }
}