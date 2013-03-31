using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.OutpostManagement.Models.Country;
using System.Web.Mvc;
using Core.Persistence;
using Core.Domain;
using Domain;
using Web.Areas.OutpostManagement.Models.Client;

namespace Web.Areas.OutpostManagement.Models.Region
{
    public class RegionOutputModel
    {
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public Guid CountryId { get; set; }
        public ClientModel Client { get; set; }
        public Guid Id { get; set; }
        public List<SelectListItem> Countries { get; set; }

        public IQueryService<Domain.Country> queryCountry { get; set; }

        public RegionOutputModel() { }
        public RegionOutputModel(IQueryService<Domain.Country> queryCountry)
        {

            this.queryCountry = queryCountry;

            Countries = new List<SelectListItem>();
            

            var result = queryCountry.Query();

            if (result.ToList().Count > 0)
            {
                foreach (Domain.Country item in result)
                {
                    var selectListItem = new SelectListItem();

                    selectListItem.Value = item.Id.ToString();
                    selectListItem.Text = item.Name;
                    Countries.Add(selectListItem);
                }
            }
        }

    }
}