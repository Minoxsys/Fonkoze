using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.LocationReport;
using Web.Services.StockUpdates;
using Web.Models.Shared;

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

            int noOfOutposts = 0;
            foreach (var country in countryQuery.ToList())
            {
                noOfOutposts = NoOfOutposts(country);
                if (noOfOutposts>0)
                {
                    var model = new MarkerModel
                    {
                        Id = country.Id,
                        Name = country.Name,
                        Number = GetStockLevel(country, null, null, null),
                        Type = GetCssClassAndInfoWindowContentForMarker(country,null,null,null,_client).CssClass,
                        Coordonates = GetCenterCoordonates(country, null , null),
                        InfoWindowContent = "Existing Sellers = "+noOfOutposts
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

        [HttpGet]
        public JsonResult GetDistrictsGridContent(FilterModel filter)
        {
            LoadUserAndClient();

            var oslQueryable = QueryStockLevel.Query().Where(it => it.Client == _client);
            if (filter.countryId != Guid.Empty)
                oslQueryable = oslQueryable.Where(it => it.Outpost.Country.Id == filter.countryId);
            if (filter.regionId != Guid.Empty)
                oslQueryable = oslQueryable.Where(it => it.Outpost.Region.Id == filter.regionId);
            if (filter.districtId != Guid.Empty)
                oslQueryable = oslQueryable.Where(it => it.Outpost.District.Id == filter.districtId);
           List<DistrictsGridModel> gridContentLst = new List<DistrictsGridModel>();
          
            List<OutpostStockLevel> oslLst = oslQueryable.ToList();
            IEnumerable<IGrouping<District, OutpostStockLevel>> oslGroupByDistrict = oslLst.GroupBy(it => it.Outpost.District);
            foreach (IGrouping<District, OutpostStockLevel> grDistrict in oslGroupByDistrict)
            {
                DistrictsGridModel gridItem = new DistrictsGridModel() { DistrictName=grDistrict.Key.Name,GreenOutposts=0,RedOutposts=0,TotalOutposts=0};

                IEnumerable<IGrouping<Outpost, OutpostStockLevel>> oslGroupByOutpost = grDistrict.GroupBy(it => it.Outpost);
                
                gridItem.TotalOutposts = NoOfOutposts(grDistrict.Key);//* the grDistrict doesn't contain the outposts with no products assigned

                foreach(IGrouping<Outpost, OutpostStockLevel> gr in oslGroupByOutpost)
                {
                    BuildDistrictGridItem(gr, ref gridItem);
                    
                }
                gridItem.RedOutposts = gridItem.TotalOutposts - gridItem.GreenOutposts;
                gridContentLst.Add(gridItem);
            }


            return Json(new StoreOutputModel<DistrictsGridModel>
            {
                Items = gridContentLst.ToArray(),
                TotalItems = gridContentLst.Count
            }, JsonRequestBehavior.AllowGet);
        }
        private void BuildDistrictGridItem(IGrouping<Outpost, OutpostStockLevel> group, ref DistrictsGridModel gridItem)
        {
            if (group.Count<OutpostStockLevel>() == 0)
            {
                return;
            }
            var resultRed = group.Where(it => it.StockLevel <= it.Product.LowerLimit);
            if (resultRed.Count() > 0)
            {
                gridItem.RedOutposts++;
            }
            else
            {
                gridItem.GreenOutposts++;
            }
        }

        internal CssClassAndInfoWinContent GetCssClassAndInfoWindowContentForMarker(Country country, Region region, District district, Outpost outpost, Client client)
        {
           CssClassAndInfoWinContent returnValue = new CssClassAndInfoWinContent();

            var result = QueryStockLevel.Query().Where(it => it.Client == client);
            if (country != null)
                result = result.Where(it => it.Outpost.Country.Id == country.Id);
            if (region != null)
                result = result.Where(it => it.Outpost.Region.Id == region.Id);
            if (district != null)
                result = result.Where(it => it.Outpost.District.Id == district.Id);
            string lastUpdate="";
            if (outpost != null)
            {
                result = result.Where(it => it.Outpost.Id == outpost.Id);
                if (result.Count() > 0)
                {
                    var updatedBySmsQuery = result.Where(it => it.UpdateMethod == StockUpdateMethod.SMS.ToString());
                    if (updatedBySmsQuery.Count() > 0)
                    {
                        lastUpdate = "<br/>Last SMS: " + updatedBySmsQuery.Max(it => it.Updated).Value.ToString("d-MMM-yyyy");
                    }
                    else
                        lastUpdate = "<br/>Last SMS: Never";
                }
            }
                       

            if (result.Count<OutpostStockLevel>() == 0)
            {
                returnValue.CssClass = "badStock";
                if (outpost != null)
                {
                    returnValue.InfoWinContent = "Seller has no products assigned.";
                }
                return returnValue;
            }

            var resultRed = result.Where(it => it.StockLevel <= it.Product.LowerLimit);

            if (resultRed.Count<OutpostStockLevel>() > 0)
            {
                returnValue.CssClass = "badStock";
                returnValue.InfoWinContent = GetInfoWindowContentForOutpost(outpost, resultRed);
                
            }
            else
            {
                result = result.Where(it => it.StockLevel <= (it.Product.LowerLimit + it.Product.LowerLimit * 20 / 100) && it.StockLevel > it.Product.LowerLimit);
                if (result.Count<OutpostStockLevel>() > 0)
                {
                    returnValue.CssClass = "closeToBadStock";
                    returnValue.InfoWinContent = GetInfoWindowContentForOutpost(outpost, result);
                }
                else
                {
                    returnValue.CssClass = "goodStock";
                    if (outpost != null)
                    {
                        returnValue.InfoWinContent = "All Stock Levels Are Good.<br/>";
                    }
                    
                }
                
            }
            if (outpost != null)
            {
                returnValue.InfoWinContent += lastUpdate;
            }

            return returnValue;
           
        }

        private string GetInfoWindowContentForOutpost(Outpost outpost, IQueryable<OutpostStockLevel> result)
        {
            string infoWindowContent = "";
            if (outpost != null)
            {
                foreach (OutpostStockLevel osl in result)
                {
                    infoWindowContent += osl.Product.Name + " = " + osl.StockLevel + "<br/>";
                }
                return infoWindowContent;
            }
            return string.Empty;
        
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

        private int NoOfOutposts(Country country)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.Country.Id == country.Id).Count();
            return numberOfOutposts;
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

            int noOfOutposts = 0;
            foreach (var region in regionQuery.ToList())
            {
                noOfOutposts = NoOfOutposts(region);
                if (noOfOutposts > 0)
                {
                    var model = new MarkerModel
                    {
                        Id = region.Id,
                        Name = region.Name,
                        Number = GetStockLevel(null, region, null, null),
                        Type = GetCssClassAndInfoWindowContentForMarker(null, region, null, null, _client).CssClass,
                        Coordonates = GetCenterCoordonates(null, region, null),
                        InfoWindowContent = "Existing Sellers = " + noOfOutposts
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

        private int NoOfOutposts(Region region)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.Region.Id == region.Id).Count();
            return numberOfOutposts;
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

            int noOfOutposts = 0;
            foreach (var district in districtQuery.ToList())
            {
                noOfOutposts = NoOfOutposts(district);
                if (noOfOutposts > 0)
                {
                    var model = new MarkerModel
                    {
                        Id = district.Id,
                        Name = district.Name,
                        Number = GetStockLevel(null, null, district, null),
                        Type = GetCssClassAndInfoWindowContentForMarker(null, null, district, null, _client).CssClass,
                        Coordonates = GetCenterCoordonates(null, null, district),
                        InfoWindowContent = "Existing Sellers = " + noOfOutposts
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

        private int NoOfOutposts(District district)
        {
            int numberOfOutposts = QueryOutposts.Query().Where(it => it.Client == _client && it.District.Id == district.Id).Count();
            return numberOfOutposts;
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
                CssClassAndInfoWinContent cssClassAndinfoWinContent = GetCssClassAndInfoWindowContentForMarker(null, null, null, outpost, _client);
                var model = new MarkerModel();
                model.Id = outpost.Id;
                model.Name = outpost.Name;
                model.Number = GetStockLevel(null, null, null, outpost);
                model.Type = cssClassAndinfoWinContent.CssClass;
                model.Coordonates = outpost.Latitude;
                model.InfoWindowContent = cssClassAndinfoWinContent.InfoWinContent;

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

        internal struct CssClassAndInfoWinContent
        {
            internal string CssClass;
            internal string InfoWinContent;

        }

    }
}
