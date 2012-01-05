using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Contact;
using AutoMapper;
using Core.Persistence;
using Domain;


namespace Web.Areas.OutpostManagement.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /OutpostManagement/MobilePhone/

           public IQueryService<Contact> QueryService { get; set; }
           public IQueryService<Contact> QueryContact { get; set; }
           public IQueryService<Outpost> QueryOutposts { get; set; }
           public IQueryService<Country> QueryCountries { get; set; }
           public IQueryService<Region> QueryRegions { get; set; }
           public IQueryService<Outpost> QueryDistricts { get; set; }
           public IQueryable<Country> countries;
           public IQueryable<Region> regions;
           public IQueryable<District> districts;
           public IQueryService<Client> QueryClients { get; set; }

           public ISaveOrUpdateCommand<Contact> SaveOrUpdateCommand { get; set; }

           public IDeleteCommand<Contact> DeleteCommand { get; set; }
        //[Requires(Permissions = "Country.Overview")]




        public ActionResult Overview(Guid outpostId)
        {
            ContactsOverviewModel model = new ContactsOverviewModel();

            model.Items = new List<ContactModel>();
            model.OutpostId = outpostId;
            //Outpost outpost;
            
            var queryResult = QueryOutposts.Query().Where(mm => mm.Id == outpostId);

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
            
            countries =  QueryCountries.Query();
            regions = QueryRegions.Query();
            //districts = QueryDistricts.Query();

            return View(model);
        }

        [HttpGet]
        //[Requires(Permissions = "Country.CRUD")]
        public ActionResult Create(Guid outpostId)
        {
            var model = new ContactModel();
            model.OutpostId = outpostId;
            model.ContactType = "Mobile Number";
            return View("Create", model);
        }


        [HttpPost]
        [ValidateInput(false)]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public ActionResult Create(ContactModel contactModel)
        {
            var model = new ContactModel();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CreateMappings();
            var contact = new Contact();

            Outpost outpost = QueryOutposts.Load(contactModel.OutpostId);
            
            contact.Client = QueryClients.Load(Client.DEFAULT_ID); ;

            Mapper.Map(contactModel, contact);
            if (outpost != null)
                outpost.Contacts.Add(contact);

            SaveOrUpdateCommand.Execute(contact);

            return RedirectToAction("Overview", "Contact", new { outpostId = contactModel.OutpostId });
         }
        
 
        // GET: /OutpostManagement/MobilePhone/Details/5
        //[HttpGet]
        ////[Requires(Permissions = "Country.CRUD")]
        //public ViewResult Edit(Guid mobilePhoneId)
        //{
        //    var mobilPhone = QueryService.Load(mobilePhoneId);
        //    var MobilePhoneModel = new MobilePhoneModel();

        //    CreateMappings();
        //    Mapper.Map(mobilPhone, MobilePhoneModel);

        //    return View(MobilePhoneModel);
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        ////[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        //public ActionResult Edit(MobilePhoneModel mobilePhoneModel)
        //{
        //    var model = new MobilePhoneModel();

        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
            
        //    CreateMappings();
        //    var mobilePhone = new MobilePhone();
        //    Mapper.Map(mobilePhoneModel, mobilePhone);
            
        //     //mobilePhone.Outpost = QueryOutposts.Load(mobilePhoneModel.OutpostId);
        //     mobilePhone.Client = QueryClients.Load(Client.DEFAULT_ID); ;

        //    SaveOrUpdateCommand.Execute(mobilePhone);

        //    return RedirectToAction("Overview", "MobilePhone", new { outpostId = mobilePhoneModel.OutpostId });
        //}

        private static void CreateMappings(Contact entity = null)
        {
            Mapper.CreateMap<Contact, ContactModel>();          
            var mapMobilePhone = Mapper.CreateMap<ContactModel, Contact>();
         }


        [HttpPost]
        //[Requires(Permissions = "OnBoarding.Candidate.CRUD")]
        public RedirectToRouteResult Delete(Guid contactId, Guid contactOutpostId)
        {
            var contact = QueryService.Load(contactId);

            if (contact != null)
                DeleteCommand.Execute(contact);

                return RedirectToAction("Overview", "Contact", new { outpostId = contactOutpostId });
        }

        [HttpGet]
        public JsonResult SetCheckContactMainMethod(Guid phoneId)        
        {
            var contact = QueryContact.Load(phoneId);
            var ContactModel = new ContactModel();

             CreateMappings();
             Mapper.Map(contact, ContactModel);
             Guid contactOutpost = contact.Outpost.Id;
            //----------------------------------------------------------
             ContactsOverviewModel model = new ContactsOverviewModel();

             model.Items = new List<ContactModel>();
             //Outpost outpost;

             var queryResult = QueryOutposts.Query().Where(mm => mm.Id == contactOutpost);

             if (queryResult.Count() > 0)
             {
                 foreach (var outpost in queryResult)
                 {

                     IList<Contact> contacts = outpost.Contacts;
                     foreach (Contact contact2 in contacts)
                     {
                         ContactModel viewModelItem = new ContactModel();
                         viewModelItem.IsMainContact = contact2.IsMainContact;
                         CreateMappings();
                         Mapper.Map(contact, viewModelItem);
                         contact2.IsMainContact = false;
                         contact2.Client = QueryClients.Load(Client.DEFAULT_ID);
                         SaveOrUpdateCommand.Execute(contact2);
                     }

                 }
             };

             countries = QueryCountries.Query();
             regions = QueryRegions.Query();

             contact.IsMainContact = true;
             contact.Client = QueryClients.Load(Client.DEFAULT_ID); ;
             contact.Outpost.DetailMethod = contact.ContactDetail;
             SaveOrUpdateCommand.Execute(contact);


            return null;
        }

        [HttpGet]
        public JsonResult SetUncheckContactMainMethod(Guid phoneId)
        {
            var contact = QueryContact.Load(phoneId);
            var ContactModel = new ContactModel();

            CreateMappings();
            Mapper.Map(contact, ContactModel);
            Guid contactOutpost = contact.Outpost.Id;
            //----------------------------------------------------------
            ContactsOverviewModel model = new ContactsOverviewModel();

            model.Items = new List<ContactModel>();
            //Outpost outpost;

            var queryResult = QueryOutposts.Query().Where(mm => mm.Id == contactOutpost);

            if (queryResult.Count() > 0)
            {
                foreach (var outpost in queryResult)
                {

                    IList<Contact> contacts = outpost.Contacts;
                    foreach (Contact contact2 in contacts)
                    {
                        ContactModel viewModelItem = new ContactModel();
                        viewModelItem.IsMainContact = contact2.IsMainContact;
                        CreateMappings();
                        Mapper.Map(contact, viewModelItem);
                        contact2.IsMainContact = false;
                        contact2.Client = QueryClients.Load(Client.DEFAULT_ID);
                        SaveOrUpdateCommand.Execute(contact2);
                    }

                }
            };

            countries = QueryCountries.Query();
            regions = QueryRegions.Query();

            contact.IsMainContact = false;
            contact.Client = QueryClients.Load(Client.DEFAULT_ID); ;
            contact.Outpost.DetailMethod = null;
            SaveOrUpdateCommand.Execute(contact);

            return null;
        }
    }
}
