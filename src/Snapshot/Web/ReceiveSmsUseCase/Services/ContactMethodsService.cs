using Core.Persistence;
using Domain;
using Persistence.Queries;
using Persistence.Queries.Outposts;
using System.Linq;
using Web.Services;
using System;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ContactMethodsService : IContactMethodsService
    {
        private readonly IQueryService<Contact> _contactsQueryService;
        private IQueryOutposts _queryOutposts;
        private ISaveOrUpdateCommand<Contact> _saveCommandContact;
        private ISaveOrUpdateCommand<Outpost> _saveOrUpdateOutpost;
        private ISendSmsService _smsService;
        public ISaveOrUpdateCommand<SentSms> _saveOrUpdateSentSms;

        public ContactMethodsService(IQueryService<Contact> contactsQueryService, IQueryOutposts queryOutposts, ISaveOrUpdateCommand<Contact> saveCommandContact, ISaveOrUpdateCommand<Outpost> saveOrUpdateOutpost, ISendSmsService smsService, ISaveOrUpdateCommand<SentSms> saveOrUpdateSentSms)
        {
            _contactsQueryService = contactsQueryService;
            _queryOutposts = queryOutposts;
            _saveCommandContact = saveCommandContact;
            _saveOrUpdateOutpost = saveOrUpdateOutpost;
            _smsService = smsService;
            _saveOrUpdateSentSms = saveOrUpdateSentSms;
        }

        public void ActivatePhoneNumber(string sender, Outpost outpost)
        {
           //TODO: activate phone number and send confirmation back to sender
            Contact contact = _contactsQueryService.Query().Where(c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(sender)).FirstOrDefault();
            contact.IsMainContact = true;
                       
            foreach (var cont in outpost.Contacts)
            {
                if (cont.Id == contact.Id)
                    cont.IsMainContact = true;
                else
                    cont.IsMainContact = false;
                _saveCommandContact.Execute(cont);
            }

            _saveCommandContact.Execute(contact);

            outpost.DetailMethod = sender;
            _saveOrUpdateOutpost.Execute(outpost);
         
           string responseString =  _smsService.SendSms(sender,"Activated!");

            SaveMessage("+" + outpost.DetailMethod, "Activated!", responseString);

        }

        private void SaveMessage(string sentTo, string message, string responseString)
        {
            SentSms sentSms = new SentSms { PhoneNumber = sentTo, Message = message, Response = responseString, SentDate = DateTime.UtcNow };
            _saveOrUpdateSentSms.Execute(sentSms);
        }

    }
}