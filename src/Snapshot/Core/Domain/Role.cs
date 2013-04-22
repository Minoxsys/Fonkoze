﻿using System.Collections.Generic;

namespace Core.Domain
{
    public class Role : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<Permission> Functions { get; set; }

        public Role()
        {
            Functions = new List<Permission>();
        }

        public virtual void AddFunction(Permission function)
        {
            Functions.Add(function);
        }

        public virtual void RemoveFunction(Permission function)
        {
            function.Roles.Remove(this);
            Functions.Remove(function);
        }
    }
}