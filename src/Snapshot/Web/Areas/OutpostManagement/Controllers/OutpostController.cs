using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Persistence.Queries.Outposts;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Contact;
using AutoMapper;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Client;
using Core.Domain;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class OutpostController : Controller
    {

        public OutpostModel OutpostModel { get; set; }
        public OutpostOutputModel OutpostModelOutput { get; set; }

        public IQueryService<Outpost> QueryWarehouse { get; set; }
        public IQueryService<Outpost> QueryService { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUser { get; set; }
        public IQueryService<Product> QueryProduct { get; set; }
        public IQueryService<Contact> QueryContact { get; set; }

        public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }
        public ISaveOrUpdateCommand<Contact> SaveOrUpdateCommandContact { get; set; }

        public IDeleteCommand<Outpost> DeleteCommand { get; set; }
        public IDeleteCommand<Contact> DeleteContactCommand { get; set; }

        public OutpostOutputModel OutpostOutputModel { get; set; }

        public OutpostInputModel OutpostInputModel { get; set; }

        public OutpostOutputModel CreateOutpost { get; set; }

        private const string TEMPDATA_ERROR_KEY = "error";


        [HttpGet]
        public ActionResult Overview()
        {
            OutpostOverviewModel model = new OutpostOverviewModel();
          

            return View(model);

        }

        [HttpGet]
        public PartialViewResult OverviewOutpost(Guid? districtId)
        {
            var outpostsList = new List<OutpostModel>();
            if (!districtId.HasValue)
                return PartialView(outpostsList);

            var outposts = QueryService.Query().Where<Outpost>(it => it.District.Id == districtId);

            foreach (Outpost item in outposts)
            {
                CreateMappings();
                var outpostModel = new OutpostModel();
                Mapper.Map(item, outpostModel);
                outpostsList.Add(outpostModel);

            }
            return PartialView(outpostsList);
        }

        public PartialViewResult SearchForOutpostWithName(string outpostName, Guid? districtId)
        {
            var outpostList = new List<OutpostModel>();

            if (districtId.Value == Guid.Empty)
            {
                var outposts = QueryService.Query().Where(it => it.Name.Contains(outpostName)).ToList();

                if (outposts.Count > 0)
                {
                    foreach (Outpost outpost in outposts)
                    {
                        CreateMappings();
                        var model = new OutpostModel();
                        Mapper.Map(outpost, model);
                        outpostList.Add(model);
                    }
                }
            }
            else
            {
                var outposts = QueryService.Query().Where(it => it.District.Id == districtId && it.Name.Contains(outpostName)).ToList();

                if (outposts.Count > 0)
                {
                    foreach (Outpost outpost in outposts)
                    {
                        CreateMappings();
                        var model = new OutpostModel();
                        Mapper.Map(outpost, model);
                        outpostList.Add(model);
                    }

                }

            }

            return PartialView("OverviewOutpost", outpostList);

        }

        public ActionResult OverviewContacts(Guid outpostId)
        {
            ContactsOverviewModel model = new ContactsOverviewModel();

            model.Items = new List<ContactModel>();
            model.OutpostId = outpostId;
            //Outpost outpost;

            var queryResult = QueryService.Query().Where(mm => mm.Id == outpostId);

            if (queryResult.Count() > 0)
            {
                foreach (var outpost in queryResult)
                {

                    IList<Contact> contacts = outpost.Contacts;
                    foreach (Contact contact in contacts)
                    {
                        ContactModel viewModelItem = new ContactModel();
                        viewModelItem.IsMainContact = contact.IsMainContact;
                        CreateMappings();
                        Mapper.Map(contact, viewModelItem);
                        model.Items.Add(viewModelItem);
                    }

                }
            };

            return View(model);
        }


        public ActionResult Create(Guid districtId)
        {

            OutpostOutputModel.District = new DistrictModel();
            OutpostOutputModel.Region = new RegionModel();

            //var district = new Domain.District(); ;

            if (districtId != null)
            {
                var district = QueryDistrict.Load(districtId);
                var countryId = district.Region.Country.Id;
                var regionId = district.Region.Id;
                OutpostOutputModel = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict, QueryService, countryId, regionId, districtId);
            }
            else
            {
                var countryId = new Guid();
                var regionId = new Guid();

                OutpostOutputModel = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict, QueryService, countryId, regionId, districtId);
            }

            OutpostOutputModel.Warehouses = new List<SelectListItem>();

            var resultWarehouse = QueryService.Query().Where(m => m.IsWarehouse);
            if (resultWarehouse != null)
            {
                if (resultWarehouse.FirstOrDefault() != null)
                {

                    var selectListItem = new SelectListItem();
                    selectListItem.Value = new Guid().ToString();
                    selectListItem.Text = "Please select a Warehouse";
                    OutpostOutputModel.Warehouses.Add(selectListItem);

                    foreach (Domain.Outpost item3 in resultWarehouse)
                    {
                        selectListItem = new SelectListItem();

                        selectListItem.Value = item3.Id.ToString();
                        selectListItem.Text = item3.Name;
                        OutpostOutputModel.Warehouses.Add(selectListItem);
                    }
                }
            }

            return View(OutpostOutputModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostInputModel();
            if (outpostInputModel.Warehouse.Id.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                outpostInputModel.Warehouse = null;
            }

            if (!ModelState.IsValid)
            {
                var outpostsOutputModel = new OutpostOutputModel();
                outpostsOutputModel.Region = new RegionModel();
                outpostsOutputModel.District = new DistrictModel();
                outpostsOutputModel.Regions = new List<SelectListItem>();
                outpostsOutputModel.Districts = new List<SelectListItem>();
                outpostsOutputModel = MapDatFromInputModelToOutputModel(outpostInputModel);
                return View("Create", outpostsOutputModel);
            }

            CreateMappings();
            var outpost = new Outpost();
            var client = QueryClients.Load(Client.DEFAULT_ID);
            Mapper.Map(outpostInputModel, outpost);

            outpost.Client = client;

            outpost.Country = QueryCountry.Load(outpostInputModel.Region.CountryId);
            outpost.Region = QueryRegion.Load(outpostInputModel.Region.Id);
            outpost.District = QueryDistrict.Load(outpostInputModel.District.Id);

            if (outpostInputModel.IsWarehouse == true)
            {
                outpostInputModel.Warehouse = null;
            }


            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Overview", "Outpost", new
            {
                countryId = outpostInputModel.Region.CountryId,
                regionId = outpostInputModel.Region.Id,
                districtId = outpostInputModel.District.Id
            });
        }



        [HttpGet]
        public ViewResult Edit(Guid outpostId, Guid countryId, Guid regionId, Guid districtId)
        {
            var _outpost = QueryService.Load(outpostId);
            var _contacts = QueryContact.Query().Where(oo => oo.Outpost.Id == outpostId);

            var warehouses = QueryService.Query().Where<Outpost>(it => it.IsWarehouse);
            var outpostModelView = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict, QueryService, countryId, regionId, districtId);

            outpostModelView.Warehouses = new List<SelectListItem>();

            CreateMappings();
            Mapper.Map(_outpost, outpostModelView);

            var selectedCountry = outpostModelView.Countries.Where<SelectListItem>(it => it.Value == _outpost.District.Region.Country.Id.ToString()).ToList();
            if (selectedCountry.Count > 0)
            {
                selectedCountry[0].Selected = true;
                Guid selectedCountryId = Guid.Parse(selectedCountry[0].Value);
                var regions = QueryRegion.Query().Where(it => it.Country.Id == selectedCountryId);

                if (regions.ToList().Count > 0)
                {
                    foreach (Region item in regions)
                    {
                        outpostModelView.Regions.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    }
                }
            }

            var selectedRegion = outpostModelView.Regions.Where<SelectListItem>(it => it.Value == _outpost.District.Region.Id.ToString()).ToList();
            if (selectedRegion.Count > 0)
                outpostModelView.Regions.First<SelectListItem>(it => it.Value == _outpost.District.Region.Id.ToString()).Selected = true;

            var selectedDistrict = outpostModelView.Regions.Where<SelectListItem>(it => it.Value == _outpost.District.Id.ToString()).ToList();
            if (selectedDistrict.Count > 0)
                outpostModelView.Districts.First<SelectListItem>(it => it.Value == _outpost.District.Id.ToString()).Selected = true;

            foreach (Outpost item in warehouses)
            {
                var selectListItem = new SelectListItem();

                selectListItem.Value = item.Id.ToString();
                selectListItem.Text = item.Name;
                outpostModelView.Warehouses.Add(selectListItem);
            }

            return View(outpostModelView);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(OutpostInputModel outpostInputModel)
        {
            var model = new OutpostInputModel();

            if (!ModelState.IsValid)
            {
                var outpostOutputModel = MapDatFromInputModelToOutputModel(outpostInputModel);
                return View("Edit", outpostOutputModel);
            }

            CreateMappings();
            var outpost = QueryService.Load(outpostInputModel.Id);
            Mapper.Map(outpostInputModel, outpost);

            outpost.Contacts = QueryContact.Query().Where(m => m.Outpost.Id == outpost.Id).ToList();

            if (outpost.IsWarehouse == true)
            {
                outpost.Warehouse = null;
            }

            if (outpostInputModel.Region != null)
            {
                outpost.Region = QueryRegion.Load(outpostInputModel.Region.Id);
            }

            if (outpostInputModel.District != null)
            {

                outpost.District = QueryDistrict.Load(outpostInputModel.District.Id);
            }

            foreach (Domain.Contact contact in outpost.Contacts)
            {
                var contactUpdate = QueryContact.Load(contact.Id);

                if (contactUpdate != null)
                {
                    if (contactUpdate.IsMainContact)
                    {
                        outpost.DetailMethod = contactUpdate.ContactDetail;
                    }

                    SaveOrUpdateCommandContact.Execute(contactUpdate);
                }
            }

            if (outpost.Warehouse != null)
            {
                var warehouse = QueryService.Load(outpost.Warehouse.Id);
                if (warehouse.Warehouse != null)
                {
                    outpost.Warehouse.Name = warehouse.Warehouse.Name;
                }
            }

            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Overview",
                new
                {
                    countryId = outpost.Country.Id,
                    regionId = outpost.Region.Id,
                    districtId = outpost.District.Id
                });

        }

        private static void CreateMappings()
        {
            Mapper.CreateMap<OutpostModel, Outpost>();
            Mapper.CreateMap<Outpost, OutpostModel>();

            Mapper.CreateMap<Outpost, OutpostInputModel>();
            Mapper.CreateMap<Outpost, OutpostOutputModel>();


            Mapper.CreateMap<OutpostInputModel, Outpost>()
                    .ForMember("Region", m => m.Ignore())
                    .ForMember("District", m => m.Ignore())
                    .ForMember("Country", m => m.Ignore());

            Mapper.CreateMap<OutpostOutputModel, Outpost>();

            Mapper.CreateMap<OutpostInputModel, Outpost>();
            Mapper.CreateMap<OutpostInputModel, Outpost>();


            Mapper.CreateMap<Country, CountryModel>();
            Mapper.CreateMap<CountryModel, Country>();

            Mapper.CreateMap<Region, RegionModel>();
            Mapper.CreateMap<RegionModel, Region>();

            Mapper.CreateMap<District, DistrictModel>();
            Mapper.CreateMap<DistrictModel, District>();

            Mapper.CreateMap<Client, ClientModel>();
            Mapper.CreateMap<ClientModel, Client>();

            Mapper.CreateMap<DistrictInputModel.ClientInputModel, Client>();
            Mapper.CreateMap<DistrictInputModel.RegionInputModel, Region>();
        }

        [HttpPost]
        public RedirectToRouteResult Delete(Guid outpostId)
        {
            var outpost = QueryService.Load(outpostId);


            if (outpost != null)
            {
                foreach (Contact contact in outpost.Contacts)
                {
                    DeleteContact(outpostId, contact.Id);
                }
                DeleteCommand.Execute(outpost);
            }

            return RedirectToAction("Overview", "Outpost", new
            {
                countryId = outpost.District.Region.Country.Id,
                regionId = outpost.District.Region.Id,
                districtId = outpost.District.Id
            });
        }

        [HttpPost]
        public RedirectToRouteResult DeleteContact(Guid outpostID, Guid contactId)
        {
            var outpost = QueryService.Load(outpostID);
            var contact = QueryContact.Load(contactId);

            if (contact != null)
            {
                DeleteContactCommand.Execute(contact);
            }

            return RedirectToAction("Edit", "Outpost", new
            {
                outpostId = outpostID
            });

        }

        [HttpGet]
        public JsonResult GetDistrictsForRegion(Guid? regionId)
        {
            List<SelectListItem> Districts = new List<SelectListItem>();

            var districts = QueryDistrict.Query().Where(it => it.Region.Id == regionId.Value);

            if (districts.ToList().Count > 0)
            {
                foreach (var item in districts)
                {
                    Districts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Districts;

            return Json(Districts, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetOutpostsForDistrict(Guid? districtId)
        {
            List<SelectListItem> Outposts = new List<SelectListItem>();

            var outposts = QueryService.Query().Where(it => it.District.Id == districtId.Value);

            if (outposts.ToList().Count > 0)
            {
                foreach (var item in outposts)
                {
                    Outposts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Outposts;

            return Json(Outposts, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetProductsList(Guid? productId)
        {
            List<SelectListItem> Outposts = new List<SelectListItem>();

            var outposts = QueryService.Query().Where(it => it.District.Id == productId.Value);

            if (outposts.ToList().Count > 0)
            {
                foreach (var item in outposts)
                {
                    Outposts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            var jsonResult = new JsonResult();
            jsonResult.Data = Outposts;

            return Json(Outposts, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CreateContact(Guid outpostId)
        {
            var model = new ContactModel();
            model.OutpostId = outpostId;
            model.ContactType = "Mobile Number";
            return View("CreateContact", model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateContact(ContactModel contactModel)
        {
            var model = new ContactModel();


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var contact = new Contact();

            Outpost outpost = QueryService.Load(contactModel.OutpostId);
            var contacts = QueryContact.Query().Where(m => m.Outpost.Id == outpost.Id);

            contact.Client = QueryClients.Load(Client.DEFAULT_ID); ;

            Mapper.Map(contactModel, contact);
            if (outpost != null)
            {
                if (contacts.Count() == 0)
                {
                    contact.IsMainContact = true;
                }
                outpost.Contacts.Add(contact);
            }

            SaveOrUpdateCommand.Execute(outpost);

            return RedirectToAction("Edit", "Outpost", new { outpostId = contactModel.OutpostId });
        }

        private OutpostOutputModel MapDatFromInputModelToOutputModel(OutpostInputModel outpostInputModel)
        {
            var countryId = outpostInputModel.Region.CountryId;
            var regionId = outpostInputModel.Region.Id;
            var districtId = outpostInputModel.District.Id;

            var outpostOutputModel = new OutpostOutputModel(QueryCountry, QueryRegion, QueryDistrict, QueryService, countryId, regionId, districtId);
            var countries = QueryCountry.Query().Where(it => it.Id == countryId);

            if (countries.ToList().Count > 0)
            {
                foreach (Country item in countries)
                {
                    outpostOutputModel.Countries.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
                }
            }
            var selectedCountry = outpostOutputModel.Countries.Where(it => it.Value == outpostInputModel.Region.CountryId.ToString()).ToList();
            if (selectedCountry.Count > 0)
                outpostOutputModel.Countries.First(it => it.Value == outpostInputModel.Region.CountryId.ToString()).Selected = true;

            if (QueryRegion.Query() != null)
            {
                var regions = QueryRegion.Query().Where(it => it.Country.Id == outpostInputModel.Region.CountryId);
                if (regions.ToList().Count > 0)
                {
                    foreach (Region item in regions)
                    {
                        outpostOutputModel.Regions.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
                    }
                }
                var selectedRegion = outpostOutputModel.Regions.Where(it => it.Value == outpostInputModel.Region.Id.ToString()).ToList();
                var selectedDistrict = outpostOutputModel.Districts.Where(it => it.Value == outpostInputModel.District.Id.ToString()).ToList();


                if (selectedRegion.Count > 0)
                    outpostOutputModel.Regions.First(it => it.Value == outpostInputModel.Region.Id.ToString()).Selected = true;

                if (selectedDistrict.Count > 0)
                    outpostOutputModel.Districts.First(it => it.Value == outpostInputModel.District.Id.ToString()).Selected = true;
            }

            outpostOutputModel.Client = new ClientModel
            {
                Id = Client.DEFAULT_ID
            };


            outpostOutputModel.Contacts = new List<Contact>();
            outpostOutputModel.Region = new RegionModel();
            outpostOutputModel.District = new DistrictModel();
            outpostOutputModel.Id = outpostInputModel.Id;
            outpostOutputModel.Name = outpostInputModel.Name;
            outpostOutputModel.Region.CountryId = outpostInputModel.Region.CountryId;
            outpostOutputModel.Region.Id = outpostInputModel.Region.Id;
            outpostOutputModel.District.Id = outpostInputModel.District.Id;


            return outpostOutputModel;
        }
    }
}
