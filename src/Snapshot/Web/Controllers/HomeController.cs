using System.Globalization;
using Core.Domain;
using Core.Persistence;
using Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Web.Security;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private User _user;
        private Client _client;

        public IQueryService<Client> LoadClient { get; set; }

        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Alert> QueryAlerts { get; set; }

        [Requires(Permissions = "Home.Index")]
        public ActionResult Index()
        {
            if (!ModelState.IsValid)
            {
                return new EmptyResult();
            }

            return View();
        }

        [HttpGet]
        public string GetNoOfAlertsInLast2Days()
        {
            var latestAlerts = QueryAlerts.Query().Where(it => it.Created >= DateTime.Now.AddDays(-2));
            return latestAlerts.Count().ToString(CultureInfo.InvariantCulture);
        }

        public ActionResult UserDetails()
        {
            LoadUserAndClient();
            return new ContentResult
                {
                    ContentType = "text/html",
                    Content = string.Format("{0} {1} ({2})", _user.FirstName, _user.LastName, _client.Name)
                };
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            _user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            _client = LoadClient.Load(clientId);
        }
    }
}