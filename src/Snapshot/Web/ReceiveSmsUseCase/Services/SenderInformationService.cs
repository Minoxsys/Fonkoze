using Core.Persistence;
using Domain;
using System.Linq;

namespace Web.ReceiveSmsUseCase.Services
{
    public class SenderInformationService : ISenderInformationService
    {
        private readonly IQueryService<Outpost> _outpostsQueryService;
        private readonly IQueryService<Contact> _contactsQueryService;

        public SenderInformationService(IQueryService<Contact> contactsQueryService, IQueryService<Outpost> outpostsQueryService)
        {
            _contactsQueryService = contactsQueryService;
            _outpostsQueryService = outpostsQueryService;
        }

        public Outpost GetOutpostWithActiveSender(string senderPhoneNumber)
        {
            return FindMatchingOutpostForSender(senderPhoneNumber, true);
        }

        public Outpost GetOutpostWithInactiveSender(string senderPhoneNumber)
        {
            return FindMatchingOutpostForSender(senderPhoneNumber, false);
        }

        private Outpost FindMatchingOutpostForSender(string senderPhoneNumber, bool activeSender)
        {
            Contact contact = _contactsQueryService.Query().FirstOrDefault(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(senderPhoneNumber) && c.IsMainContact == activeSender);
            Outpost outpost = _outpostsQueryService.Query().FirstOrDefault(o => o.Contacts.Contains(contact));
            return outpost;
        }
    }
}