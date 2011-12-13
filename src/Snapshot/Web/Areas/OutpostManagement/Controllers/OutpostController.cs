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

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {
        public IQueryService<Outpost> QueryService { get; set; }

        public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }

        public IDeleteCommand<Outpost> DeleteCommand { get; set; }

        //[Requires(Permissions = "Country.Overview")]
        public ActionResult Overview()
        {
            Web.Areas.OutpostManagement.Models.Outpost.Overview model = new Web.Areas.OutpostManagement.Models.Outpost.Overview();

            model.Items = new List<OutpostModel>();

            var queryResult = QueryService.Query();

            if (queryResult.ToList().Count() > 0)
                queryResult.ToList().ForEach(item =>
                {
                    OutpostModel viewModelItem = new OutpostModel();

                    CreateMappings();

                    Mapper.Map(item, viewModelItem);

                    model.Items.Add(viewModelItem);
                });

            return View(model);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create()
        {
            var model = new OutpostModelOutput();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(OutpostModelInput outpostModel)
        {
            var model = new OutpostModelInput();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var outpost = new Outpost();
            Mapper.Map(outpostModel, outpost);
        //  Mapper.Map(countryModel, country);

            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Overview", "Outpost");
        }


        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ViewResult Edit(Guid countryId)
        {
            var country = QueryService.Load(countryId);
            var OutpostModel = new OutpostModel();

            CreateMappings();
            Mapper.Map(country, OutpostModel);

            return View(OutpostModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Edit(OutpostModelInput OutpostModel)
        {
            var model = new OutpostModelInput();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var outpost = new Outpost();
            Mapper.Map(OutpostModel, outpost);

            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Overview", "Outpost");
        }

        private static void CreateMappings(Outpost entity = null)
        {
            Mapper.CreateMap<Outpost, OutpostModel>();

            var mapOutpost = Mapper.CreateMap<OutpostModelInput, Outpost>();

            if (entity != null)
                mapOutpost.ForMember(m => m.Id, options => options.Ignore());
        }


        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid countryId)
        {
            var country = QueryService.Load(countryId);

            if (country != null)
                DeleteCommand.Execute(country);

            return RedirectToAction("Overview", "Outpost");
        }

    }
}
