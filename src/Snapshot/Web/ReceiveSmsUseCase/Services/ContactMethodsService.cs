using Core.Persistence;
using Domain;
using System.Linq;
using Web.Services;

namespace Web.ReceiveSmsUseCase.Services
{
    public class ContactMethodsService : IContactMethodsService
    {
        private readonly IQueryService<Contact> _contactsQueryService;
        private readonly ISaveOrUpdateCommand<Contact> _saveCommandContact;
        private readonly ISaveOrUpdateCommand<Outpost> _saveOrUpdateOutpost;
        private readonly ISendSmsService _smsService;

        public ContactMethodsService(IQueryService<Contact> contactsQueryService, ISaveOrUpdateCommand<Contact> saveCommandContact,
                                     ISaveOrUpdateCommand<Outpost> saveOrUpdateOutpost, ISendSmsService smsService)
        {
            _contactsQueryService = contactsQueryService;
            _saveCommandContact = saveCommandContact;
            _saveOrUpdateOutpost = saveOrUpdateOutpost;
            _smsService = smsService;
        }

        public void ActivatePhoneNumber(string sender, Outpost outpost)
        {
            Contact contact =
                _contactsQueryService.Query().FirstOrDefault(c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(sender));
            if (contact != null)
            {

                foreach (var cont in outpost.Contacts)
                {
                    cont.IsMainContact = cont.Id == contact.Id;
                    _saveCommandContact.Execute(cont);
                }
                
                outpost.DetailMethod = sender;
                _saveOrUpdateOutpost.Execute(outpost);

                _smsService.SendSms(sender, "Activated!", true);
            }
        }
    }
}