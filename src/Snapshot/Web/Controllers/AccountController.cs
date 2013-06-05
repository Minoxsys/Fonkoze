using Core.Domain;
using Core.Persistence;
using Core.Services;
using System;
using System.Web.Mvc;
using Web.LocalizationResources;
using Web.Models.Account;

namespace Web.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        public IAuthenticationService FormsService { get; set; }
        public IMembershipService AuthenticateUser { get; set; }
        public IQueryService<User> QueryUser { get; set; }
        public ISaveOrUpdateCommand<User> SaveUser { get; set; }
        public LogOnModel LogOnOutput { get; set; }

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        public ActionResult LogOn()
        {
            return View(LogOnOutput);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (
                    AuthenticateUser.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);

                    //CreateUserIfNotFound(model.UserName);

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", Strings.AccountController_LogOn_The_user_name_or_password_provided_is_incorrect_);
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
