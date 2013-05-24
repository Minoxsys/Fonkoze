using Core.Domain;
using Core.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Web.Models.Alerts;
using Web.Security;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Web.Services;

namespace Web.Controllers
{
    public class AlertsController : Controller
    {
        public IRetrieveAllDistrictsService retrieveAllDistrictsService  { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Alert> QueryAlerts { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        private Client _client;
        private User _user;

        [Requires(Permissions = "Alert.View")]
        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAlerts(AlertsIndexModel indexModel, OverviewInputModel input = null)
        {
            LoadUserAndClient();

            Debug.Assert(indexModel.limit != null, "indexModel.limit != null");
            var pageSize = indexModel.limit.Value;
            var alertsDataQuery = QueryAlerts.Query().Where(it => it.Client.Id == _client.Id);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Alert>>>
                {
                    {"OutpostName-ASC", () => alertsDataQuery.OrderBy(c => c.OutpostName)},
                    {"OutpostName-DESC", () => alertsDataQuery.OrderByDescending(c => c.OutpostName)},
                    {"Contact-ASC", () => alertsDataQuery.OrderBy(c => c.Contact)},
                    {"Contact-DESC", () => alertsDataQuery.OrderByDescending(c => c.Contact)},
                    {"ProductGroupName-ASC", () => alertsDataQuery.OrderBy(c => c.ProductGroupName)},
                    {"ProductGroupName-DESC", () => alertsDataQuery.OrderByDescending(c => c.ProductGroupName)},
                    {"LowLevelStock-ASC", () => alertsDataQuery.OrderBy(c => c.LowLevelStock)},
                    {"LowLevelStock-DESC", () => alertsDataQuery.OrderByDescending(c => c.LowLevelStock)},
                    {"LastUpdate-ASC", () => alertsDataQuery.OrderBy(c => c.LastUpdate)},
                    {"LastUpdate-DESC", () => alertsDataQuery.OrderByDescending(c => c.LastUpdate)},
                    {"AlertType-ASC", () => alertsDataQuery.OrderBy(c => c.LastUpdate)},
                    {"AlertType-DESC", () => alertsDataQuery.OrderByDescending(c => c.LastUpdate)},
                    {"Date-ASC", () => alertsDataQuery.OrderBy(c => c.Created)},
                    {"Date-DESC", () => alertsDataQuery.OrderByDescending(c => c.Created)},
                };

            alertsDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                alertsDataQuery = alertsDataQuery
                    .Where(it => it.OutpostName.Contains(indexModel.searchValue));
            }
            if (input != null && input.DistrictId != Guid.Empty)
            {
                alertsDataQuery = alertsDataQuery
                    .Where(it => QueryOutpost.Query().Where(i => i.District.Id == input.DistrictId).Where(j => j.Id == it.OutpostId).Count() != 0);
            }

            var totalItems = alertsDataQuery.Count();

            Debug.Assert(indexModel.start != null, "indexModel.start != null");
            alertsDataQuery = alertsDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var alertsModelListProjection = (from alert in alertsDataQuery.ToList()
                                             select new AlertModel
                                                 {
                                                     Id = alert.Id,
                                                     OutpostId = alert.OutpostId,
                                                     OutpostName = alert.OutpostName,
                                                     ProductGroupId = alert.ProductGroupId,
                                                     ProductGroupName = alert.ProductGroupName,
                                                     LowLevelStock = alert.LowLevelStock,
                                                     LastUpdate = alert.LastUpdate.HasValue ? alert.LastUpdate.Value.ToString("dd-MMM-yyyy, HH:mm") : "-",
                                                     Contact = alert.Contact,
                                                     AlertType = alert.AlertType.ToString(),
                                                     Date = alert.Created.HasValue ? alert.Created.Value.ToString("dd-MMM-yyyy, HH:mm") : "-"
                                                 }).ToArray();

            return Json(new AlertsIndexOutputModel
                {
                    Alerts = alertsModelListProjection,
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
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
