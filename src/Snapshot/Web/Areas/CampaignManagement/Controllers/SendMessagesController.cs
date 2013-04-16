﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Security;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class SendMessagesController : Controller
    {
        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Outpost> QueryOutposts { get; set; }

        private Client _client;
        private User _user;

        [Requires(Permissions = "Campaign.View")]
        public ActionResult Overview()
        {
            return View();
        }
        
    
        [HttpGet]
        public JsonResult GetOutposts()
        {
            LoadUserAndClient();

            var outpostsQuery = QueryOutposts.Query().Where(it => it.Client.Id == this._client.Id);

            var outpostModelListProjection = new List<ReferenceModel>();
            foreach (var outpost in outpostsQuery.ToList())
            {
                var refModel = new ReferenceModel
                                    {
                                         Id = outpost.Id,
                                         Name = outpost.Name,
                                         Selected = false
                                    };
                outpostModelListProjection.Add(refModel);
            }           

            return Json(new ReferenceModelOutput
            {
                Items = outpostModelListProjection.ToArray(),
                TotalItems = outpostModelListProjection.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = LoadClient.Load(clientId);
        }

    }
}