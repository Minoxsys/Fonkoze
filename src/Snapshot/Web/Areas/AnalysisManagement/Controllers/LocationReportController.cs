using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.LocationReport;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class LocationReportController : Controller
    {
        public IQueryService<Outpost> QueryOutposts { get; set; }
        public IQueryService<District> QueryDistricts { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<Country> QueryCountries { get; set; }
        public IQueryService<OutpostStockLevel> QueryStockLevel { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        private Client _client;
        private User _user;

        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCountryMarkers(FilterModel filtermodel)
        {
            LoadUserAndClient();
            var countryQuery = QueryCountries.Query().Where(it => it.Client == _client);
            if (filtermodel.countryId != Guid.Empty)
                countryQuery = countryQuery.Where(it => it.Id == filtermodel.countryId);

            List<MarkerModel> countries = new List<MarkerModel>();

            foreach (var country in countryQuery.ToList())
            {
                if (HasOutposts(country))
                {
                    var model = new MarkerModel
                    {
                        Id = country.Id,
                        Name = country.Name,
                        Number = GetStockLevel(country, null, null, null),
                        Type = GetCssClassForMarker(country,null,null,null),
                        Coordonates = GetCenterCoordonates(country, null , null),
                    };
                    countries.Add(model);
                }
            }

            return Json(new MarkerIndexOutputModel
            {
                Markers = countries.ToArray(),
                TotalItems = countries.Count
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetCssClassForMarker(Country country, Region region, District district, Outpost outpost)
        {
            var result = QueryStockLevel.Query().Where(it => it.Client == _client);
            if (country != null)
                result = result.Where(it => it.Outpost.Country.Id == country.Id);
            if (region != null)
                result = result.Where(it => it.Outpost.Region.Id == region.Id);
            if (district != null)
                result = result.Where(it => it.Outpost.District.Id == district.Id);
            if (outpost != null)
                result = result.Where(it => it.Outpost.Id == outpost.Id);
           
            result = result.Where(it => it.StockLevel < it.Product.LowerLimit);

            if (result.Count<OutpostStockLevel>()>0)
                return "badStock";
            else
            {
                result = result.Where(it => it.StockLevel <= (it.Product.LowerLimit + it.Product.LowerLimit * 20 / 100) && it.StockLevel > it.Product.LowerLimit);
                if (result.Count<OutpostStockLevel>() > 0)
                    return "closeToBadStock";
                else
                    return "goodStock";
            }
           
        }

        private string GetStockLevel(Country country, Region region, District district, Outpost outpost)
        {
            int sum = 0;
            var stockLevels = QueryStockLevel.Query().Where(it => it.Client == _client);

            if (country != null)
                stockLevels = stockLevels.Where(it => it.Outpost.Country.Id == country.Id);
            if (region != null)
                stockLevels = stockLevels.Where(it => it.Outpost.Region.Id == region.Id);
            if (district != null)
                stockLevels = stockLevels.Where(it => it.Outpost.District.Id == district.Id);
            if (outpost != null)
                stockLevels = stockLevels.Where(it => it.Outpost.Id == outpost.Id);

            foreach (var stockLevel in stockLevels)
                sum += stockLevel.StockLevel;

            return sum.ToString();
        }

        private bool HasOutposts(Country country)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.Country.Id == country.Id).Count();
            return numberOfOutposts > 0;
        }

        private string GetCenterCoordonates(Country country, Region region, District district)
        {
            double lat = 0;
            double lng = 0;

            var outpostDataQuery = QueryOutposts.Query().Where(it => it.Client == _client);
            if (country != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.Country.Id == country.Id);
            if (region != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.Region.Id == region.Id);
            if (district != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == district.Id);

            foreach (var outpost in outpostDataQuery)
            {
                if (outpost.Latitude != null)
                {
                    lat += double.Parse(outpost.Latitude.Substring(1, outpost.Latitude.IndexOf(",") - 1));
                    lng += double.Parse(outpost.Latitude.Substring(outpost.Latitude.IndexOf(",") + 1, outpost.Latitude.Length - outpost.Latitude.IndexOf(",") - 2).Trim());
                }
            }

            return "(" + lat / outpostDataQuery.Count() + "," + lng / outpostDataQuery.Count() + ")";
        }

        [HttpGet]
        public JsonResult GetRegionMarkers(FilterModel filtermodal)
        {
            LoadUserAndClient();
            var regionQuery = QueryRegions.Query().Where(it => it.Client == _client);
            if (filtermodal.countryId != Guid.Empty)
                regionQuery = regionQuery.Where(it => it.Country.Id == filtermodal.countryId);
            if (filtermodal.regionId != Guid.Empty)
                regionQuery = regionQuery.Where(it => it.Id == filtermodal.regionId);

            List<MarkerModel> regions = new List<MarkerModel>();

            foreach (var region in regionQuery.ToList())
            {
                if (HasOutposts(region))
                {
                    var model = new MarkerModel
                    {
                        Id = region.Id,
                        Name = region.Name,
                        Number = GetStockLevel(null, region, null, null),
                        Type = GetCssClassForMarker(null,region,null,null),
                        Coordonates = GetCenterCoordonates(null, region, null),
                    };
                    regions.Add(model);
                }
            }

            return Json(new MarkerIndexOutputModel
            {
                Markers = regions.ToArray(),
                TotalItems = regions.Count
            }, JsonRequestBehavior.AllowGet);
        }

        private bool HasOutposts(Region region)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.Region.Id == region.Id).Count();
            return numberOfOutposts > 0;
        }

        [HttpGet]
        public JsonResult GetDistrictMarkers(FilterModel filtermodal)
        {
            LoadUserAndClient();
            var districtQuery = QueryDistricts.Query().Where(it => it.Client == _client);
            if (filtermodal.countryId != Guid.Empty)
                districtQuery = districtQuery.Where(it => it.Region.Country.Id == filtermodal.countryId);
            if (filtermodal.regionId != Guid.Empty)
                districtQuery = districtQuery.Where(it => it.Region.Id == filtermodal.regionId);
            if (filtermodal.districtId != Guid.Empty)
                districtQuery = districtQuery.Where(it => it.Id == filtermodal.districtId);

            List<MarkerModel> districts = new List<MarkerModel>();

            foreach (var district in districtQuery.ToList())
            {
                if (HasOutposts(district))
                {
                    var model = new MarkerModel
                    {
                        Id = district.Id,
                        Name = district.Name,
                        Number = GetStockLevel(null, null, district, null),
                        Type = GetCssClassForMarker(null,null,district,null),
                        Coordonates = GetCenterCoordonates(null, null, district),
                    };
                    districts.Add(model);
                }
            }

            return Json(new MarkerIndexOutputModel
            {
                Markers = districts.ToArray(),
                TotalItems = districts.Count
            }, JsonRequestBehavior.AllowGet);
        }

        private bool HasOutposts(District district)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.District.Id == district.Id).Count();
            return numberOfOutposts > 0;
        }

        [HttpGet]
        public JsonResult GetOutpostMarkers(FilterModel filtermodal)
        {
            LoadUserAndClient();
            var outpostQuery = QueryOutposts.Query().Where(it => it.Client == _client);
            if (filtermodal.countryId != Guid.Empty)
                outpostQuery = outpostQuery.Where(it => it.Country.Id == filtermodal.countryId);
            if (filtermodal.regionId != Guid.Empty)
                outpostQuery = outpostQuery.Where(it => it.Region.Id == filtermodal.regionId);
            if (filtermodal.districtId != Guid.Empty)
                outpostQuery = outpostQuery.Where(it => it.District.Id == filtermodal.districtId);

            List<MarkerModel> outposts = new List<MarkerModel>();

            foreach (var outpost in outpostQuery.ToList())
            {
                var model = new MarkerModel();
                model.Id = outpost.Id;
                model.Name = outpost.Name;
                model.Number = GetStockLevel(null, null, null, outpost);
                model.Type = GetCssClassForMarker(null,null,null,outpost);
                model.Coordonates = outpost.Latitude;

                outposts.Add(model);
            }

            return Json(new MarkerIndexOutputModel
            {
                Markers = outposts.ToArray(),
                TotalItems = outposts.Count
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
