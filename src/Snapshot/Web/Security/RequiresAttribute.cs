using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Core.Security;
using Web.Bootstrap.Container;
using System.Web.Routing;
using Web.Controllers;
using Autofac;

namespace Web.Security
{
	public class RequiresAttribute: FilterAttribute, IAuthorizationFilter
	{
		
		public virtual void OnAuthorization( AuthorizationContext context )
		{
			var unityAccessor = context.HttpContext.ApplicationInstance as IContainerAccessor;

			var permissionsService = unityAccessor.Container.Resolve<IPermissionsService>();

			if (permissionsService == null)
			{
				throw new InvalidOperationException("IoC could not find FunctionRightsService implementation ");

			}

			var requiresPermissions = SplitString(Permissions);

            bool isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (!isAuthenticated)
            {
                context.Result = new HttpUnauthorizedResult();
                return;
            }

            bool letItPass = isAuthenticated;

			if (requiresPermissions.Length > 0)
				letItPass = letItPass && requiresPermissions.Any(right => permissionsService.HasPermissionAssigned(right,context.HttpContext.User.Identity.Name));

            if (!letItPass)
            {
                
                var redirectToUnAuthorizedAccess = new RedirectToRouteResult(
                    new RouteValueDictionary{
                        {"controller", "UnauthorizedAccess"},
                        {"area", null}
                    }
                );
                context.Result = redirectToUnAuthorizedAccess;
            }
	
		}

		private string[] SplitString( string @string )
		{
			if (string.IsNullOrWhiteSpace(@string))
				return new string[] { };

			var splits = @string.Split(',');

			return splits;
		}

		public string Permissions
		{
			get;
			set;
		}
	}
}
