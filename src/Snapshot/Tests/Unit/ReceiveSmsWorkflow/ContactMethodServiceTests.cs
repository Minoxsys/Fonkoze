using Core.Persistence;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.ReceiveSmsUseCase.Services;
using Web.Services;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class ContactMethodServiceTests
    {
        private IQueryService<Contact> _contactsQueryService;
        private ISaveOrUpdateCommand<Contact> _saveCommandContact;
        private ISaveOrUpdateCommand<Outpost> _saveOrUpdateOutpost;
        private ISendSmsService _smsService;
        private Contact _contact;
        private Outpost _outpost;
        private ContactMethodsService _sut;
        private string _phoneNo;

        [SetUp]
        public void PerTestSetup()
        {
            _contactsQueryService = MockRepository.GenerateMock<IQueryService<Contact>>();
            _saveCommandContact = MockRepository.GenerateMock<ISaveOrUpdateCommand<Contact>>();
            _saveOrUpdateOutpost = MockRepository.GenerateMock<ISaveOrUpdateCommand<Outpost>>();
            _smsService = MockRepository.GenerateMock<ISendSmsService>();

            _phoneNo = "1234567";

            var contactId = Guid.NewGuid();
            _contact = MockRepository.GeneratePartialMock<Contact>();
            _contact.ContactDetail = _phoneNo;
            _contact.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            _contact.IsMainContact = false;
            _contact.Stub(c => c.Id).Return(contactId);

            var contactId2 = Guid.NewGuid();
            var contact2 = MockRepository.GeneratePartialMock<Contact>();
            contact2.ContactDetail = "67890";
            contact2.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            contact2.IsMainContact = true;
            contact2.Stub(c => c.Id).Return(contactId2);

            var contactLst = new List<Contact> {_contact, contact2};
            _outpost = MockRepository.GeneratePartialMock<Outpost>();
            _outpost.Contacts = contactLst;

            _sut = new ContactMethodsService(_contactsQueryService, _saveCommandContact, _saveOrUpdateOutpost, _smsService);
        }

        [Test]
        public void ActivatePhoneNumber_UpdatesContact()
        {

            _contactsQueryService.Expect(call => call.Query()).Return(new[] {_contact}.AsQueryable());

            _saveCommandContact.Expect(call => call.Execute(Arg<Contact>.Matches(c =>
                                                                                 c.ContactDetail == _phoneNo &&
                                                                                 c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE &&
                                                                                 c.IsMainContact)
                                                   ));
            _saveCommandContact.Expect(call => call.Execute(Arg<Contact>.Matches(c =>
                                                                                 c.ContactDetail == "67890" &&
                                                                                 c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE &&
                                                                                 c.IsMainContact == false)
                                                   ));
            _sut.ActivatePhoneNumber(_phoneNo, _outpost);

            _contactsQueryService.VerifyAllExpectations();
            _saveCommandContact.VerifyAllExpectations();

        }

        [Test]
        public void ActivatePhoneNumber_UpdatesOutpost()
        {
            _contactsQueryService.Expect(call => call.Query()).Return(new[] {_contact}.AsQueryable());
            _saveOrUpdateOutpost.Expect(call => call.Execute(Arg<Outpost>.Matches(o => o.DetailMethod == _phoneNo)));

            _sut.ActivatePhoneNumber(_phoneNo, _outpost);

            _contactsQueryService.VerifyAllExpectations();
            _saveOrUpdateOutpost.VerifyAllExpectations();
        }

        [Test]
        public void ActivatePhoneNumber_SendsSmsConfirmationMessageBackToSender()
        {
            _contactsQueryService.Expect(call => call.Query()).Return(new[] {_contact}.AsQueryable());
            _smsService.Expect(call => call.SendSms(_phoneNo, "Activated!", true)).Return(null);

            _sut.ActivatePhoneNumber(_phoneNo, _outpost);

            _smsService.VerifyAllExpectations();
        }
    }
}
