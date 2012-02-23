using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.CampaignManagement.Models.Campaign;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Models.Shared;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class CampaignController : Controller
    {
        public IQueryService<Campaign> QueryCampaign { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<District> QueryDistricts { get; set; }
        public IQueryService<Outpost> QueryOutposts { get; set; }

        public ISaveOrUpdateCommand<Campaign> SaveOrUpdateCommand { get; set; }

        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        private const string STATUS_OPEN = "Opened";
        private const string STATUS_CLOSE = "Closed";

        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult Create(CampaignInputModel model)
        {
            LoadUserAndClient();

            Campaign campaign = new Campaign();
            campaign.Name = model.CampaignName;
            campaign.StartDate = DateTime.Parse(model.StartDate);
            campaign.EndDate = DateTime.Parse(model.EndDate);
            campaign.Client = this._client;
            campaign.CreationDate = DateTime.UtcNow;
            campaign.Opened = true;
            campaign.Options = StrToByteArray((ConvertToJSON(GetOptionsModel(model.CountriesIds, model.RegionsIds, model.DistrictsIds, model.OutpostsIds))));

            SaveOrUpdateCommand.Execute(campaign);

            return Json(
               new JsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Campaign {0} has been saved.", campaign.Name)
               });
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private OptionsModel GetOptionsModel(string countries, string regions, string districts, string outposts)
        {
            OptionsModel model = new OptionsModel();
            model.Countries = countries; //GetListOfIds(countries);
            model.Regions = regions; // GetListOfIds(regions);
            model.Districts = districts; // GetListOfIds(districts);
            model.Outposts = outposts; // GetListOfIds(outposts);

            return model;
        }

        private List<Guid> GetListOfIds(string listOfIds)
        {
            string[] ids = listOfIds.Split(',');
            var list = new List<Guid>();
            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                    list.Add(new Guid(id));
            }
            return list;
        }

        private string ConvertToJSON(OptionsModel model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(model);
        }


        public JsonResult GetCampaigns(CampaignOverviewInputModel overviewInputModel)
        {
            LoadUserAndClient();

            int pageSize = overviewInputModel.limit.Value;
            var campaigns = QueryCampaign.Query().Where(it => it.Client.Id == _client.Id);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Campaign>>>()
            {
                { "Name-ASC", () => campaigns.OrderBy(it => it.Name) },
                { "Name-DESC", () => campaigns.OrderByDescending(it => it.Name) },
                { "StartDate-ASC", () => campaigns.OrderBy(it => it.StartDate) },
                { "StartDate-DESC", () => campaigns.OrderByDescending(it => it.StartDate)},
                { "EndDate-ASC", () =>campaigns.OrderBy(it => it.EndDate) },
                { "EndDate-DESC", () => campaigns.OrderByDescending(it => it.EndDate) },
                { "CreationDate-ASC", () => campaigns.OrderBy(it => it.CreationDate) },
                { "CreationDate-DESC",() => campaigns.OrderByDescending(it => it.CreationDate) }
            };
            //campaigns = orderByColumnDirection[String.Format("{0}-{1}", overviewInputModel.sort, overviewInputModel.dir)].Invoke();

            if (overviewInputModel.searchValue != null)
                campaigns = campaigns.Where(it => it.Name.Contains(overviewInputModel.searchValue));

            int totalItems = campaigns.Count();

            campaigns = campaigns.Take(pageSize)
                                 .Skip(overviewInputModel.start.Value);

            if (totalItems > 0)
            {
                var campaignList = new List<CampaignOutputModel>();
                foreach (var campaign in campaigns.ToList())
                {
                    var campaignModel = new CampaignOutputModel();
                    campaignModel.Id = campaign.Id;
                    campaignModel.Name = campaign.Name;
                    campaignModel.StartDate = campaign.StartDate.Value.ToString("dd-MMM-yyyy");
                    campaignModel.EndDate = campaign.EndDate.Value.ToString("dd-MMM-yyyy");
                    campaignModel.CreationDate = campaign.CreationDate.Value.ToString("dd-MMM-yyyy");
                    campaignModel.Status = (campaign.Opened == true) ? STATUS_OPEN : STATUS_CLOSE;
                    campaignList.Add(campaignModel);
                }
                return Json(new CampaignOverviewOutputModel
                {
                    Campaigns = campaignList.ToArray(),
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new CampaignOverviewOutputModel
            {
                Campaigns = null,
                TotalItems = 0
            }, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult GetRegions(string countryIdList)
        {
            LoadUserAndClient();
            if (string.IsNullOrEmpty(countryIdList))
            {
                return Json(new ReferenceModelOutput
                {
                    Items = null,
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);
            }
            var regions = QueryRegions.Query().Where(it => it.Client.Id == this._client.Id);

            string[] countryIds = countryIdList.Split(',');
            var listOfRegions = new List<Region>();
            foreach (string countryId in countryIds)
            {
                if (!string.IsNullOrEmpty(countryId))
                    listOfRegions.AddRange(regions.Where(it => it.Country.Id == new Guid(countryId)).ToList());
            }

            var regionModelListProjection = (from region in listOfRegions
                                              select new ReferenceModel
                                              {
                                                  Id = region.Id,
                                                  Name = region.Name,
                                              }).ToArray();

            return Json(new ReferenceModelOutput
            {
                Items = regionModelListProjection,
                TotalItems = regionModelListProjection.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistricts(string regionIdList)
        {
            LoadUserAndClient();
            if (string.IsNullOrEmpty(regionIdList))
            {
                return Json(new ReferenceModelOutput
                {
                    Items = null,
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);
            }
            var districts = QueryDistricts.Query().Where(it => it.Client.Id == this._client.Id);

            string[] regionsIds = regionIdList.Split(',');
            var listOfDistricts = new List<District>();
            foreach (string regionId in regionsIds)
            {
                if (!string.IsNullOrEmpty(regionId))
                    listOfDistricts.AddRange(districts.Where(it => it.Region.Id == new Guid(regionId)).ToList());
            }

            var districtModelListProjection = (from region in listOfDistricts
                                              select new ReferenceModel
                                              {
                                                  Id = region.Id,
                                                  Name = region.Name,
                                              }).ToArray();

            return Json(new ReferenceModelOutput
            {
                Items = districtModelListProjection,
                TotalItems = districtModelListProjection.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOutposts(string districtIdList)
        {
            LoadUserAndClient();
            if (string.IsNullOrEmpty(districtIdList))
            {
                return Json(new ReferenceModelOutput
                {
                    Items = null,
                    TotalItems = 0
                }, JsonRequestBehavior.AllowGet);
            }
            var outposts = QueryOutposts.Query().Where(it => it.Client.Id == this._client.Id);

            string[] districtsIds = districtIdList.Split(',');
            var listOfOutposts = new List<Outpost>();
            foreach (string districtId in districtsIds)
            {
                if (!string.IsNullOrEmpty(districtId))
                    listOfOutposts.AddRange(outposts.Where(it => it.District.Id == new Guid(districtId)).ToList());
            }

            var outpostModelListProjection = (from region in listOfOutposts
                                               select new ReferenceModel
                                               {
                                                   Id = region.Id,
                                                   Name = region.Name,
                                               }).ToArray();

            return Json(new ReferenceModelOutput
            {
                Items = outpostModelListProjection,
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
