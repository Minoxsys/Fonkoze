using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Core.Domain;
using Persistence;
using Core.Services;
using Persistence.Queries.Employees;
using Web.Models.Account;
using Core.Persistence;

namespace Web.Controllers
{

	[HandleError]
	public class AccountController :Controller	{
		
		public IAuthenticationService FormsService
		{
			get;
			set;
		}
		
		public IMembershipService AuthenticateUser
		{
			get;
			set;
		}

		
		public IQueryService<User> QueryUser
		{
			get;
			set;
		}

		

		public ISaveOrUpdateCommand<User> SaveUser
		{
			get;
			set;
		}

		
		public Models.Account.LogOnModel LogOnOutput
		{
			get;
			set;
		}

		// **************************************
		// URL: /Account/LogOn
		// **************************************

		public ActionResult LogOn()
		{
			return View(LogOnOutput);
		}

		[HttpPost]
        //[ValidateInput(false)]
		public ActionResult LogOn( LogOnModel model, string returnUrl )
		{
			if (ModelState.IsValid)
			{
				if (
                    AuthenticateUser.ValidateUser(model.UserName,model.Password))
				{
					FormsService.SignIn(model.UserName, model.RememberMe);

					//CreateUserIfNotFound(model.UserName);

					if (!String.IsNullOrEmpty(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}


		// **************************************
		// URL: /Account/LogOff
		// **************************************

		public ActionResult LogOff()
		{
			FormsService.SignOut();

            return RedirectToAction("LogOn");
		}

		


	}
}
