using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Core.Security
{
	public class CustomPrincipal : IPrincipal
	{
		IPermissionsService functionRight;
		IIdentity identity;
        string userName;

        public CustomPrincipal(IPermissionsService functionRight, IIdentity identity, string userName)
		{
			this.functionRight = functionRight;

			this.identity = identity;

            this.userName = userName;
		}

		public IIdentity Identity
		{
			get
			{
				return identity;
			}
		}

		public bool IsInRole( string role )
		{
            var isInRole = functionRight.HasPermissionAssigned(role, userName);
			return isInRole;
		}
	}
}
