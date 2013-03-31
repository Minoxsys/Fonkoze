using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models.UserManager
{
    public class UserManagerCreateInputModel
    {
        public UserModel Employee
        {
            get;
            set;
        }
        public string InfoMessage
        {
            get;
            set;
        }       
        public string ConfirmedPassword { get; set; }

        public UserManagerCreateInputModel()
        {
            Employee = new UserModel();
        }
    }
}