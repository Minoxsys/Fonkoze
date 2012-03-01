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

        public virtual Guid ClientId { get; set; }
        public virtual Guid RoleId { get; set; }

		public override string ToString()
		{
			return "" +Id;
		}
	}
}
