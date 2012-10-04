using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class AccountOptionsController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public ISaveOrUpdateCommand<User> SaveOrUpdateCommand { get; set; }

        public ISecurePassword SecurePassword { get; set; }

        private Client _client;
        private User _user;

        [HttpGet]
        public ActionResult Overview()
        {
            LoadUserAndClient();
            UserModel model = new UserModel { FirstName = _user.FirstName, LastName = _user.LastName, Email = _user.Email };
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveProfileInformation(UserModel model)
        {
            LoadUserAndClient();

            var user = QueryUsers.Load(_user.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            SaveOrUpdateCommand.Execute(user);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Profile information has been saved.")
               });
        }

        [HttpPost]
        public JsonResult SavePassword(PasswordModel model)
        {
            LoadUserAndClient();
            var user = QueryUsers.Load(_user.Id);

            string currentPassword = SecurePassword.EncryptPassword(model.CurrentPassword);
            if (user.Password != currentPassword)
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = String.Format("Current Password is not correct!")
                   });

            user.Password = SecurePassword.EncryptPassword(model.NewPassword);
            SaveOrUpdateCommand.Execute(user);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   Message = String.Format("Password has been saved.")
               });
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }

    }
}
