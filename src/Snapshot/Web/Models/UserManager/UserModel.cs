using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.UserManager
{
    public class UserModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public Guid ClientId { get; set; }
        public IList<Role> Roles { get; set; }

        public UserModel()
        {
            Roles = new List<Role>();
        }

        
    }
}