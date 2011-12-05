using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Security
{
	public interface IPermissionsService
	{
        bool HasPermissionAssigned(string function, string userName);
	}
}
