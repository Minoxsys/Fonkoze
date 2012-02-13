using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.UserManager
{
    public class RolesIndexOutputModel
    {
        public ReferenceModel[] Roles { get; set; }
        public int TotalItems { get; set; }
    }
}