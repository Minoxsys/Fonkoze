using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Web.Controllers;
using Web.Areas.OutpostManagement.Models;
using Web.Areas.OutpostManagement.Models.Outpost;
using AutoMapper;
using Web.Bootstrap.Converters;
using Core.Persistence;
using Persistence.Queries.Employees;
using System.Net.Mail;
using Web.Helpers;
using Web.Security;
using Web.Validation.ValidDate;
using System.Globalization;
using Domain;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class MobilePhoneController : Controller
    {
        //
        // GET: /OutpostManagement/MobilePhone/

           public IQueryService<MobilePhone> QueryService { get; set; }
           public IQueryService<Outpost> QueryOutposts { get; set; }
           public IQueryService<Country> QueryCountries { get; set; }
           public IQueryService<Region> QueryRegions { get; set; }
           public IQueryService<Outpost> QueryDistricts { get; set; }
           public IQueryable<Country> countries;
           public IQueryable<Region> regions;
           public IQueryable<District> districts;

           public ISaveOrUpdateCommand<MobilePhone> SaveOrUpdateCommand { get; set; }

           public IDeleteCommand<MobilePhone> DeleteCommand { get; set; }
        //[Requires(Permissions = "Country.Overview")]




        public ActionResult Overview(Guid outpostId)
        {
            Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones model = new Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones();

            model.Items = new List<MobilePhoneModel>();

            var queryResult = QueryService.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    MobilePhoneModel viewModelItem = new MobilePhoneModel();
                    CreateMappings();
                    Mapper.Map(item, viewModelItem);
                    model.Items.Add(viewModelItem);
                });

            countries =  QueryCountries.Query();
            regions = QueryRegions.Query();
            //districts = QueryDistricts.Query();

            return View(model);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create(Guid outpostId)
        {
            var model = new MobilePhoneModel();
            model.OutpostId = outpostId;
            return View(model);
        }

        //
 
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(MobilePhoneModel mobilePhoneModel)
        {
            var model = new MobilePhoneModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var mobilePhone = new MobilePhone();
                
            mobilePhone.Outpost = QueryOutposts.Load(mobilePhoneModel.OutpostId);
            Mapper.Map(mobilePhoneModel, mobilePhone);

            SaveOrUpdateCommand.Execute(mobilePhone);

            return RedirectToAction("PhoneListInput", "MobilePhone", new { outpostId = mobilePhoneModel.OutpostId });
         }
        
  
        [HttpGet]
        public ActionResult PhoneListInput(Guid outpostId)
        {
            Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones model = new Web.Areas.OutpostManagement.Models.Outpost.OverviewPhones();

            model.Items = new List<MobilePhoneModel>();

            var queryResult = QueryService.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    MobilePhoneModel viewModelItem = new MobilePhoneModel();
                    CreateMappings();
                    Mapper.Map(item, viewModelItem);
                    model.Items.Add(viewModelItem);
                });

            return View(model);
        }


        // GET: /OutpostManagement/MobilePhone/Details/5
        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid mobilePhoneId)
        {
            var mobilPhone = QueryService.Load(mobilePhoneId);
            var MobilePhoneModel = new MobilePhoneModel();

            CreateMappings();
            Mapper.Map(mobilPhone, MobilePhoneModel);

            return View(MobilePhoneModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(MobilePhoneModel mobilePhoneModel)
        {
            var model = new MobilePhoneModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var mobilePhone = new MobilePhone();
            Mapper.Map(mobilePhoneModel, mobilePhone);
            mobilePhone.Outpost = QueryOutposts.Load(mobilePhoneModel.OutpostId);

            SaveOrUpdateCommand.Execute(mobilePhone);

            return RedirectToAction("PhoneListInput", "MobilePhone", new { outpostId = mobilePhoneModel.OutpostId });
        }

        private static void CreateMappings(MobilePhone entity = null)
        {
            Mapper.CreateMap<MobilePhone, MobilePhoneModel>();

            var mapMobilePhone = Mapper.CreateMap<MobilePhoneModel, MobilePhone>();

            if (entity != null)
                mapMobilePhone.ForMember(m => m.Id, options => options.Ignore());
        }


        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid mobilePhoneId)
        {
            var mobilePhone = QueryService.Load(mobilePhoneId);

            if (mobilePhoneId != null)
                DeleteCommand.Execute(mobilePhone);

            return RedirectToAction("EditPhoneList", "MobilePhone");
        }
    }
}
