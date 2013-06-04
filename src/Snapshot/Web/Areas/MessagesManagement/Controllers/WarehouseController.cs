using Domain.Enums;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;
using Web.Security;
using Web.Services;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using System;
using System.Linq;
using Core.Persistence;
using Domain;
using Core.Domain;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class WarehouseController : Controller
    {
        public IRetrieveAllDistrictsService retrieveAllDistrictsService { get; set; }
        public IRawSmsMeesageQueryHelpersService SmsQueryService { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromWarehouse(IndexTableInputModel indexTableInputModel, OverviewInputModel input = null)
        {
            var districtId = input == null ? Guid.Empty : input.DistrictId;
            return Json(SmsQueryService.GetMessagesFromOutpost(indexTableInputModel, OutpostType.Warehouse, districtId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDistricts()
        {
            LoadUserAndClient();

            var districtList = retrieveAllDistrictsService.GetAllDistrictsForOneClient(_client.Id);

            return Json(new
            {
                districts = districtList,
                TotalItems = districtList.Count
            }, JsonRequestBehavior.AllowGet);
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

            _client = QueryClients.Load(clientId);
        }
    }
}
