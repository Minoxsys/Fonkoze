﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Models.Alerts;
using Web.Security;

namespace Web.Controllers
{
    public class AlertsController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Alert> QueryAlerts { get; set; }

        private Client _client;
        private User _user;

        [Requires(Permissions = "Alert.View")]
        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAlerts(AlertsIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var alertsDataQuery = this.QueryAlerts.Query().Where(it => it.Client.Id == this._client.Id);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Alert>>>()
            {
                { "OutpostName-ASC", () => alertsDataQuery.OrderBy(c => c.OutpostName) },
                { "OutpostName-DESC", () => alertsDataQuery.OrderByDescending(c => c.OutpostName) },
                { "Contact-ASC", () => alertsDataQuery.OrderBy(c => c.Contact) },
                { "Contact-DESC", () => alertsDataQuery.OrderByDescending(c => c.Contact) },
                { "ProductGroupName-ASC", () => alertsDataQuery.OrderBy(c => c.ProductGroupName) },
                { "ProductGroupName-DESC", () => alertsDataQuery.OrderByDescending(c => c.ProductGroupName) },
                { "LowLevelStock-ASC", () => alertsDataQuery.OrderBy(c => c.LowLevelStock) },
                { "LowLevelStock-DESC", () => alertsDataQuery.OrderByDescending(c => c.LowLevelStock) },
                { "LastUpdate-ASC", () => alertsDataQuery.OrderBy(c => c.LastUpdate) },
                { "LastUpdate-DESC", () => alertsDataQuery.OrderByDescending(c => c.LastUpdate) }
            };

            alertsDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
            {
                alertsDataQuery = alertsDataQuery
                    .Where(it => it.OutpostName.Contains(indexModel.searchValue));
            }

            var totalItems = alertsDataQuery.Count();

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
                                                 LastUpdate = alert.LastUpdate.Value.ToString("dd-MMM-yyyy"),
                                                 Contact = alert.Contact
                                             }).ToArray();

            return Json(new AlertsIndexOutputModel
            {
                Alerts = alertsModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
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