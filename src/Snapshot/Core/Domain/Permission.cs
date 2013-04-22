using System.Collections.Generic;

namespace Core.Domain
{
    public class Permission : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual IList<Role> Roles { get; set; }

        public Permission()
        {
            Roles = new List<Role>();
        }

        public virtual void AddRole(Role role)
        {
            Roles.Add(role);
        }
    }
}
