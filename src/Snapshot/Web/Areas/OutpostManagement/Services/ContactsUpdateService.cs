using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using Domain;
using Web.Models.Parsing.Outpost;
using Web.Security;

namespace Web.Areas.OutpostManagement.Services
{
    public class ContactsUpdateService : IContactsUpdateService
    {
        private IQueryService<Contact> _queryContact;
        private ISaveOrUpdateCommand<Contact> _saveOrUpdateCommandContact;

        public ContactsUpdateService(IQueryService<Contact> queryContact, ISaveOrUpdateCommand<Contact> saveOrUpdateCommandContact)
        {
            _queryContact = queryContact;
            _saveOrUpdateCommandContact = saveOrUpdateCommandContact;
        }

        public void AddContact(UserAndClientIdentity loggedUser, Outpost outpost, IParsedOutpost parsedOutpost)
        {
            var contact = new Contact
            {
                Client = loggedUser.Client,
                ByUser = loggedUser.User,
                Outpost = outpost,
                IsMainContact = true,
                ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE,
            };

            if (!string.IsNullOrWhiteSpace(parsedOutpost.ContactDetail))
            {
                contact.ContactDetail = parsedOutpost.ContactDetail;
            }

            _saveOrUpdateCommandContact.Execute(contact);
        }

        private void UpdateContact(Contact existentContact, IParsedOutpost parsedOutpost)
        {
            var contactToUpdate = _queryContact.Query().Where(it => it == existentContact).FirstOrDefault();

            if (contactToUpdate != null)
            {
                contactToUpdate.ContactDetail = parsedOutpost.ContactDetail;
                _saveOrUpdateCommandContact.Execute(contactToUpdate);
            }
        }

        public void ManageOutpostContact(UserAndClientIdentity loggedUser, Outpost existentOutpost, IParsedOutpost parsedOutpost)
        {
            var existentContact = existentOutpost.Contacts.Where(c => c.ContactDetail == parsedOutpost.ContactDetail).FirstOrDefault();

            if (existentContact == null)
            {
                DeactivateExistentContacts(existentOutpost);
                AddContact(loggedUser, existentOutpost, parsedOutpost);
            }
        }

        private void DeactivateExistentContacts(Outpost existentOutpost)
        {
            var contactsForOutpost = _queryContact.Query().Where(c => c.Outpost == existentOutpost);

            foreach(var contact in contactsForOutpost)
            {
                contact.IsMainContact = false;
            }
        }
    }
}