using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.RoleManager
{
    public class RoleManagerEditInput
    {
        public class RoleModel
        {
            public Guid Id
            {
                get;
                set;
            }

            [Required]
            [StringLength(40)]
            public string Name
            {
                get;
                set;
            }
            [Required]
            public string Description
            {
                get;
                set;
            }
        }

        public RoleModel Role
        {
            get;
            set;
        }
    }
}