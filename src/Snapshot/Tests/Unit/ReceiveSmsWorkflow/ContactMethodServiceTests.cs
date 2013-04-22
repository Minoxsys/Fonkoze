using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Persistence.Queries.Outposts;
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
        private Contact contact;
        private Contact contact2;
        private Outpost outpost;
        private ContactMethodsService _sut;
        private string phoneNo;
        private Guid contactId;
        private Guid contactId2;

        [SetUp]
        public void PerTestSetup()
        {
            _contactsQueryService = MockRepository.GenerateMock<IQueryService<Contact>>();
            _saveCommandContact = MockRepository.GenerateMock<ISaveOrUpdateCommand<Contact>>();
            _saveOrUpdateOutpost = MockRepository.GenerateMock<ISaveOrUpdateCommand<Outpost>>();
            _smsService = MockRepository.GenerateMock<ISendSmsService>();

            phoneNo="1234567";

            Guid contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.ContactDetail = phoneNo;
            contact.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            contact.IsMainContact = false;
            contact.Stub(c => c.Id).Return(contactId);
            
            Guid contactId2 = Guid.NewGuid();
            contact2 = MockRepository.GeneratePartialMock<Contact>();
            contact2.ContactDetail = "67890";
            contact2.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            contact2.IsMainContact = true;
            contact2.Stub(c => c.Id).Return(contactId2);

            List<Contact> contactLst = new List<Contact>();
            contactLst.Add(contact);
            contactLst.Add(contact2);
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Contacts = contactLst;
            
            _sut = new ContactMethodsService(_contactsQueryService, _saveCommandContact, _saveOrUpdateOutpost, _smsService);

            
        }


        [Test]
        public void ActivatePhoneNumber_UpdatesContact()
        {

           _contactsQueryService.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());

           _saveCommandContact.Expect(call => call.Execute(Arg<Contact>.Matches(c =>
                                                                           c.ContactDetail == phoneNo &&
                                                                           c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE &&
                                                                           c.IsMainContact)
                                                                           ));
           _saveCommandContact.Expect(call => call.Execute(Arg<Contact>.Matches(c =>
                                                                          c.ContactDetail == "67890" &&
                                                                          c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE &&
                                                                          c.IsMainContact==false)
                                                                          ));
           _sut.ActivatePhoneNumber(phoneNo, outpost);
         
            _contactsQueryService.VerifyAllExpectations();
           _saveCommandContact.VerifyAllExpectations();

        }

        [Test]
        public void ActivatePhoneNumber_UpdatesOutpost()
        {

            _contactsQueryService.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());
            _saveOrUpdateOutpost.Expect(call => call.Execute(Arg<Outpost>.Matches(o => o.DetailMethod == phoneNo)));

            _sut.ActivatePhoneNumber(phoneNo,outpost);

            _contactsQueryService.VerifyAllExpectations();
            _saveOrUpdateOutpost.VerifyAllExpectations();

        }


        [Test]
        public void ActivatePhoneNumber_CallsSendSms()
        {
            _contactsQueryService.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());
            _smsService.Expect(call => call.SendSms(phoneNo, "Activated!", true)).Return(null);
            _sut.ActivatePhoneNumber(phoneNo, outpost);
            _smsService.VerifyAllExpectations();
        }

    }
}
