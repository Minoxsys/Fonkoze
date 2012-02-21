using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.CampaignManagement.Models.Campaign;
using Domain;
using Core.Persistence;
using Core.Domain;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class CampaignController : Controller
    {
        public IQueryService<Campaign> QueryCampaign { get; set; }
        public ISaveOrUpdateCommand<Campaign> SaveOrUpdateCampaign { get; set; }

        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        private Client _client;
        private User _user;

        private const string STATUS_OPEN = "Opened";
        private const string STATUS_CLOSE = "Closed";
        public ActionResult Overview()
        {
            return View();
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

        public JsonResult GetCampaigns(CampaignOverviewInputModel overviewInputModel)
        {
            LoadUserAndClient();
            int pageSize = 0;

            if (overviewInputModel.Limit != null)
                pageSize = overviewInputModel.Limit.Value;

            var campaigns = QueryCampaign.Query().Where(it => it.Client.Id == _client.Id);


            if (overviewInputModel.SearchName != null)
                campaigns = campaigns.Where(it => it.Name.Contains(overviewInputModel.SearchName));

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Campaign>>>()
            {
                { "Name-ASC", () => campaigns.OrderBy(it=>it.Name) },
                { "Name-DESC", () => campaigns.OrderByDescending(c => c.Name) },
                { "StartDate-ASC", () => campaigns.OrderBy(it=>it.StartDate) },
                { "StartDate-DESC", () => campaigns.OrderByDescending(it=>it.StartDate)},
                { "EndDate-ASC", () =>campaigns.OrderBy(it=>it.EndDate) },
                { "EndDate-DESC", () => campaigns.OrderByDescending(it=>it.EndDate) },
                { "CreationDate-ASC", () => campaigns.OrderBy(it=>it.CreationDate) },
                { "CreationDate-DESC",() => campaigns.OrderByDescending(it=>it.CreationDate) }
            };

            int totalItems = 0;

            campaigns = orderByColumnDirection[String.Format("{0}-{1}", overviewInputModel.sort, overviewInputModel.dir)].Invoke();
            totalItems = campaigns.Count();
            campaigns = campaigns.Take(pageSize)
                                 .Skip(overviewInputModel.Start.Value);

            var campaignList = new List<CampaignOutputModel>();

            foreach (var campaign in campaigns)
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
                Campaigns = campaignList,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountries()
        {
            LoadUserAndClient();
            var countries = QueryCountry.Query().Where(it => it.Client.Id == _client.Id).ToList();

            var countriesList = new List<LocationEntityModel>();

            foreach (var country in countries)
            {
                var countryModel = new LocationEntityModel();
                countryModel.Id = country.Id;
                countryModel.Name = country.Name;
                countriesList.Add(countryModel);
            }
            return Json(new
            {
                countries = countriesList,
                TotalItems = countriesList.Count
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRegions(LocationEntitiesInput entitiesInput)
        {
            LoadUserAndClient();

            var regions = new List<Region>();

            if ((entitiesInput.EntitiesIds != null)&&(entitiesInput.EntitiesIds.Count > 0))
            {
                regions = QueryRegion.Query().Where(it => entitiesInput.EntitiesIds.Contains(it.Country.Id) && it.Client.Id == _client.Id).ToList();
            }
            else
            {
                regions = QueryRegion.Query().Where(it => it.Client.Id == _client.Id).ToList();
            }

            var regionList = new List<LocationEntityModel>();

            foreach (var region in regions)
            {
                var regionModel = new LocationEntityModel();
                regionModel.Id = region.Id;
                regionModel.Name = region.Name;
                regionModel.BelongsTo_LocationEntityName = region.Country.Name;
                regionList.Add(regionModel);
            }
            return Json(new
            {
                regions = regionList,
                TotalItems = regionList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistricts(LocationEntitiesInput entitiesInput)
        {
            LoadUserAndClient();
            var districts = new List<District>();

            if ((entitiesInput.EntitiesIds != null) &&(entitiesInput.EntitiesIds.Count > 0))
            {
                districts = QueryDistrict.Query().Where(it => entitiesInput.EntitiesIds.Contains(it.Region.Id) && it.Client.Id == _client.Id).ToList();
            }
            else
            {
                districts = QueryDistrict.Query().Where(it => it.Client.Id == _client.Id).ToList();
            }

            var districtList = new List<LocationEntityModel>();

            foreach (var district in districts)
            {
                var districtModel = new LocationEntityModel();
                districtModel.Id = district.Id;
                districtModel.Name = district.Name;
                districtModel.BelongsTo_LocationEntityName = district.Region.Name;
                districtList.Add(districtModel);
            }

            return Json(new
            {
                districts = districtList,
                TotalItems = districtList.Count
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOutposts(LocationEntitiesInput entitiesInput)
        {
            var outposts = new List<Outpost>();
            LoadUserAndClient();

            if ((entitiesInput.EntitiesIds != null) && (entitiesInput.EntitiesIds.Count > 0))
            {
                outposts = QueryOutpost.Query().Where(it => entitiesInput.EntitiesIds.Contains(it.District.Id) && it.Client.Id == _client.Id).ToList();
            }
            else
            {
                outposts = QueryOutpost.Query().Where(it => it.Client.Id == _client.Id).ToList();
            }

            var outpostList = new List<LocationEntityModel>();

            foreach (var outpost in outposts)
            {
                var outpostModel = new LocationEntityModel();
                outpostModel.Id = outpost.Id;
                outpostModel.Name = outpost.Name;
                outpostModel.BelongsTo_LocationEntityName = outpost.District.Name;
                outpostList.Add(outpostModel);
            }

            return Json(new
            {
                outposts = outpostList,
                TotalItems = outpostList.Count
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
