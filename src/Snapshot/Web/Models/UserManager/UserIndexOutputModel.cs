using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.UserManager
{
    public class UserIndexOutputModel
    {
        public UserOutputModel[] Users { get; set; }
        public int TotalItems { get; set; }
    }
}