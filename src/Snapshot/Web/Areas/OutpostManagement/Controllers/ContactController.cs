using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Contact;
using AutoMapper;
using Core.Persistence;
using Domain;
using Core.Domain;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class ContactController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;
        public IQueryService<Contact> QueryService { get; set; }
        public IQueryService<Contact> QueryContact { get; set; }
        public IQueryService<Outpost> QueryOutposts { get; set; }
        public IQueryService<Country> QueryCountries { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<Outpost> QueryDistricts { get; set; }

        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public ISaveOrUpdateCommand<Contact> SaveOrUpdateCommand { get; set; }
        public ISaveOrUpdateCommand<Outpost> UpdateOutpost { get; set; }

        public IDeleteCommand<Contact> DeleteCommand { get; set; }

        public class GetContactIndex
        {
            public Guid? OutpostId { get; set; }
        }

        [HttpGet]
        public JsonResult Index(GetContactIndex input)
        {
            LoadUserAndClient();
            var outpost = LoadOutpost(input.OutpostId);

            var contacts = QueryContact.Query().Where(contact => contact.Outpost == outpost).ToList();

            var contactsModelArray = (from c in contacts
                                      select new ContactModel
                                      {
                                          OutpostId = outpost.Id,
                                          IsMainContact = c.IsMainContact,
                                          ContactDetail = c.ContactDetail,
                                          ContactType = c.ContactType,
                                          Id = c.Id
                                      });
            return Json(contactsModelArray, JsonRequestBehavior.AllowGet);
        }

        private Outpost LoadOutpost(Guid? outpostId)
        {
            var outpost = QueryOutposts.Load(outpostId.Value);
            return outpost;
        }

        public class PostContactIndex : ContactModel
        {
        }

        [HttpPost]
        public JsonResult Index(PostContactIndex input)
        {
            LoadUserAndClient();
            var outpost = LoadOutpost(input.OutpostId);

            var contact = new Contact
            {
                Client = _client,
                ByUser = _user,
                Outpost = outpost,
                ContactType = input.ContactType,
                ContactDetail = input.ContactDetail,
                IsMainContact = input.IsMainContact
            };

            if (input.ContactDetail != null)
            {
                SaveOrUpdateCommand.Execute(contact);

                ChangeDetailMethodOnOutpostWhenIsMainContact(outpost, contact);
            }
            
            return Json(new { success = true, message = "New contact has been created", data = ToContactModel(contact) });
        }

        private void ChangeDetailMethodOnOutpostWhenIsMainContact(Outpost outpost, Contact contact)
        {
            if (!contact.IsMainContact)
                return;
            outpost.DetailMethod = contact.ContactDetail;
            UpdateOutpost.Execute(outpost);
        }

        private object ToContactModel(Contact c)
        {
            return new ContactModel
            {
                Id = c.Id,
                OutpostId = c.Outpost.Id,
                IsMainContact = c.IsMainContact,
                ContactDetail = c.ContactDetail,
                ContactType = c.ContactType
            };
        }

        public class PutContactIndex : ContactModel
        {
        }

        [HttpPut]
        public JsonResult Index(PutContactIndex input)
        {
            LoadUserAndClient();
            var outpost = LoadOutpost(input.OutpostId);

            var contact = QueryContact.Load(input.Id.Value);
          
            contact.Client = _client;
            contact.ByUser = _user;
            contact.Outpost = outpost;

            contact.ContactType = input.ContactType;
            contact.ContactDetail = input.ContactDetail;
            contact.IsMainContact = input.IsMainContact;

            SaveOrUpdateCommand.Execute(contact);
            ChangeDetailMethodOnOutpostWhenIsMainContact(outpost, contact);

            return Json(new { success = true, message = "Contact has been updated", data = ToContactModel(contact) });
        }

        public class DeleteContactIndex : ContactModel
        {
        }

        [HttpDelete]
        public JsonResult Index(DeleteContactIndex input)
        {
            LoadUserAndClient();

            var outpost = LoadOutpost(input.OutpostId);

            var contact = QueryContact.Load(input.Id.Value);

            if (contact != null)
                DeleteCommand.Execute(contact);

            return Json(new { success = true, message = "Contact has been removed" });
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

            this._client = LoadClient.Load(clientId);
        }
    }
}