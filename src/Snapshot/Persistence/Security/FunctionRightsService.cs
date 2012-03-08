using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Persistence;
using System.Web;
using Core.Security;
using Persistence.Queries.Functions;
using Persistence.Queries.Employees;

namespace Persistence.Security
{
	/// <summary>
	/// Based on the current logged in user, will determine
	/// function right access
	/// </summary>
	public class FunctionRightsService : IPermissionsService
	{
		IQueryService<Permission> queryFunctions;
		IQueryService<User> queryEmployees;

        public FunctionRightsService() { }	
		public FunctionRightsService( 
				IQueryService<Permission> queryFunctions,
				IQueryService<User> queryEmployees)
		{
			this.queryFunctions = queryFunctions;
			this.queryEmployees = queryEmployees;

		}
		/// <summary>
		/// Checks wether or not this function has been assigned
		/// to the current employee
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public bool HasPermissionAssigned( string function, string userName )
		{

			var functionEntity = queryFunctions.Query(new FunctionByName(function)).FirstOrDefault();
			if (functionEntity == null) // no such function found in the system
				return false;

            var empl = queryEmployees.Query(new UserByUserName(userName)).FirstOrDefault();
			if (empl == null) // no employee logged in
				return false;

			var rolesAssignedToThisFunction = functionEntity.Roles.ToList();
            var roleIdAssignedToThisEmployee = empl.RoleId;


			foreach (var functionRight in rolesAssignedToThisFunction)
			{
                if (roleIdAssignedToThisEmployee == functionRight.Id)
					return true;
			}
			return false;
		}
	}

	// TODO REFACTOR TO OWN FILES 

	
	
}
