using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Web.Models.Home;
using Core.Persistence;
using AutoMapper;
using Web.Bootstrap.Converters;
using Web.Models.Shared;
using Web.Security;
using System.ComponentModel.DataAnnotations;
using Core.Domain;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
		private Core.Domain.User _user;
		private Client _client;

		public IQueryService<Client> LoadClient { get; set; }

		public IQueryService<User> QueryUsers { get; set; }

        [Requires(Permissions = "Home.Index")]
        public ActionResult Index()
        {
            if (!this.ModelState.IsValid)
            {
                return new EmptyResult();
            }
            Web.Models.Home.IndexModel listModel = new Web.Models.Home.IndexModel();


            return View(listModel);
        }

		public ActionResult UserDetails()
		{	
			LoadUserAndClient();
			return new ContentResult
			{
				ContentType="text/html",
				Content= string.Format("{0} {1} ({2})",_user.FirstName, _user.LastName, _client.Name)
			};
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

			this._client = LoadClient.Load(clientId);
		}


    }
}