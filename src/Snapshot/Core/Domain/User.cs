using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Domain
{
	public class User : DomainEntity
	{
        public virtual string FirstName { get; set; }
        public virtual string LastName {get; set; }
        public virtual string UserName {get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string ClientName { get; set; }

        public virtual Guid ClientId { get; set; }
        public virtual Guid RoleId { get; set; }

		public virtual IList<Role> Roles
		{
			get;
			set;
		}
        
        

		public User()
		{
			Roles = new List<Role>();
            //Client = new Client();
		}

		public virtual void AddRole( Role role )
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole( Role role )
		{
			role.Employees.Remove(this);
			this.Roles.Remove(role);
		}

		public override string ToString()
		{
			return "" +Id;
		}
	}
}
