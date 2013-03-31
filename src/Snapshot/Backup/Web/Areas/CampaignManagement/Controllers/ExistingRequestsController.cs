using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Web.Areas.CampaignManagement.Models.ExistingRequests;
using Web.Security;
using System.Collections.Generic;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Web.Areas.CampaignManagement.Controllers
{
    public partial class ExistingRequestsController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;

        public IQueryService<Client> LoadClient { get; set; }

        public IQueryService<Core.Domain.User> QueryUsers { get; set; }

        public IQueryService<Outpost> QueryOutposts { get; set; }
        public IQueryService<Campaign> QueryCampaign { get; set; }

        public IQueryService<RequestRecord> QueryRequests { get; set; }

        [Requires(Permissions = "ExistingRequest.View")]
        public ActionResult Overview()
        {

            return View();
        }

        public JsonResult GetOutposts(GetOutpostsInput input)
        {
            LoadUserAndClient();

            List<GetOutpostsOutput> outpostResult = new List<GetOutpostsOutput>();
            GetOutpostsOutput allModel = new GetOutpostsOutput { Id = Guid.Empty, Name = " All" };
            outpostResult.Add(allModel);

            if (input.CampaignId.HasValue && input.CampaignId != Guid.Empty)
            {
                var outposts = QueryOutposts.Query().Where(c => c.Client == _client);
                var campaign = QueryCampaign.Load(input.CampaignId.Value);
                var options = campaign.RestoreOptions<Web.Areas.CampaignManagement.Models.Campaign.OptionsModel>();

                Guid[] outpostIds = new Guid[] { };
                if (options != null)
                {
                    outpostIds = options.Outposts.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(it => new Guid(it)).ToArray();
                }

                if (outpostIds.Count() == 0)
                    return Json(null, JsonRequestBehavior.AllowGet);

                outposts = outposts.Where(c => outpostIds.Contains(c.Id));

                foreach (var outpost in outposts.ToList())
                {
                    var model = new GetOutpostsOutput
                    {
                        Id = outpost.Id,
                        Name = outpost.Name
                    };
                    outpostResult.Add(model);
                }
            }
            return Json(outpostResult.ToArray(), JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult GetCampaigns()
        {
            LoadUserAndClient();
            var campaignsDataQry = QueryCampaign.Query().Where(p => p.Client == _client);
            List<CampaignModel> campaigns = new List<CampaignModel>();
            CampaignModel allModel = new CampaignModel { Id = Guid.Empty.ToString(), Name = " All" };
            campaigns.Add(allModel);

            foreach (var campaign in campaignsDataQry)
            {
                var model = new CampaignModel
                {
                    Id = campaign.Id.ToString(),
                    Name = campaign.Name
                };
                campaigns.Add(model);
            }


            return Json(campaigns.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExistingRequests(GetExistingRequestsInput input)
        {
            LoadUserAndClient();

            var requestsData = QueryRequests.Query().Where(req => req.Client == _client);


            if (input.CampaignId.HasValue && input.CampaignId != Guid.Empty)
            {
                requestsData = requestsData.Where(req => req.CampaignId == input.CampaignId.Value);
            }

            if (input.OutpostId.HasValue && input.OutpostId != Guid.Empty)
            {
                requestsData = requestsData.Where(req => req.OutpostId == input.OutpostId.Value);
            }

            if (input.From.HasValue)
            {
                requestsData = requestsData.Where(req => req.Created >= input.From.Value);
            }

            if (input.To.HasValue)
            {
                requestsData = requestsData.Where(req => req.Created <= input.To.Value);
            }

            var totalItems = requestsData.Count();

            requestsData = requestsData.Take(input.limit.Value).Skip(input.start.Value);

            var requestModels = requestsData.ToList().Select( req=>
                    new GetExistingRequestModel
                    {
                        Campaign=req.CampaignName,
                        Outpost = req.OutpostName,
                        ProductGroup = req.ProductGroupName,
                        ProductsNo = req.ProductsNo,
                        Date = req.Created.HasValue? req.Created.Value.ToString("dd-MMM-yyyy"): string.Empty
                    }
                ).ToArray();

            var output = new GetExistingRequestsOutput
            {
                TotalItems = totalItems,
                ExitingRequests = requestModels
            };

            return Json(output, JsonRequestBehavior.AllowGet);
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