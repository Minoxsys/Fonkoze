using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.RoleManager
{
    public class RoleReferenceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
    }
}