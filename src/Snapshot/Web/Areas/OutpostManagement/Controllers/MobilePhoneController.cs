using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.MobilePhone;
using AutoMapper;
using Core.Persistence;
using Core.Domain;
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
           public IQueryService<Client> QueryClients { get; set; }

           public ISaveOrUpdateCommand<MobilePhone> SaveOrUpdateCommand { get; set; }

           public IDeleteCommand<MobilePhone> DeleteCommand { get; set; }
        //[Requires(Permissions = "Country.Overview")]




        public ActionResult Overview(Guid outpostId)
        {
            MobilePhonesOverviewModel model = new MobilePhonesOverviewModel();

            model.Items = new List<MobilePhoneModel>();
            model.Outpost_FK = outpostId;
            
            var queryResult = QueryService.Query().Where(mm => mm.Outpost_FK == outpostId);

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
            model.Outpost_FK = outpostId;
            return View(model);
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
             
            mobilePhone.Outpost = QueryOutposts.Load(mobilePhoneModel.Outpost_FK);
            mobilePhone.Client = QueryClients.Load(Client.DEFAULT_ID); ;
            
            Mapper.Map(mobilePhoneModel, mobilePhone);

            SaveOrUpdateCommand.Execute(mobilePhone);

            return RedirectToAction("Overview", "MobilePhone", new { outpostId = mobilePhoneModel.Outpost_FK });
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
            
             mobilePhone.Outpost = QueryOutposts.Load(mobilePhoneModel.Outpost_FK);
             mobilePhone.Client = QueryClients.Load(Client.DEFAULT_ID); ;

            SaveOrUpdateCommand.Execute(mobilePhone);

            return RedirectToAction("Overview", "MobilePhone", new { outpostId = mobilePhoneModel.Outpost_FK });
        }

        private static void CreateMappings(MobilePhone entity = null)
        {
            Mapper.CreateMap<MobilePhone, MobilePhoneModel>();
                    //.ForMember("Outpost", m => m.Ignore())
                    //.ForMember("Client", m => m.Ignore());
            

            var mapMobilePhone = Mapper.CreateMap<MobilePhoneModel, MobilePhone>();

           // if (entity != null)
           // {
            //    mapMobilePhone.ForMember(m => m.Id, options => options.Ignore());
            //    mapMobilePhone.ForMember(m => m.Outpost_FK, options => options.Ignore());
            //}
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
