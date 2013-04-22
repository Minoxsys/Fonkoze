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
        [Test]
        public void Updates_ContactAndOutpostAndSentSms_And_NewRecordIn_SentSmss()
        {
            //Mock<IQueryService<Contact>> _contactsQueryService =new Mock<IQueryService<Contact>>();
            //Mock<IQueryOutposts> _queryOutposts = new Mock<IQueryOutposts>();
            //Mock<ISaveOrUpdateCommand<Contact>> _saveCommandContact = new Mock<ISaveOrUpdateCommand<Contact>>();
            //Mock<ISaveOrUpdateCommand<Outpost>> _saveOrUpdateOutpost = new Mock<ISaveOrUpdateCommand<Outpost>>();
            //Mock<ISendSmsService> _smsService = new Mock<ISendSmsService>();
            //Mock<ISaveOrUpdateCommand<SentSms>> _saveOrUpdateSentSms = new Mock<ISaveOrUpdateCommand<SentSms>>();

            //ContactMethodsService _sut = new ContactMethodsService(MockRepository.GenerateMock<IQueryService<Contact>>(),
            //    MockRepository.GenerateMock<IQueryOutposts>(), MockRepository.GenerateMock<ISaveOrUpdateCommand<Contact>>(),
            //    MockRepository.GenerateMock<ISaveOrUpdateCommand<Outpost>>(), MockRepository.GenerateMock<ISendSmsService>(), MockRepository.GenerateMock<ISaveOrUpdateCommand<SentSms>>());

            //Guid contactId = Guid.NewGuid();
            //Contact contact = MockRepository.GeneratePartialMock<Contact>();
            //contact.ContactDetail = "1234567";
            //contact.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            //contact.IsMainContact = false;
            //// contact.Stub(c => c.Id).Return(contactId);

            //_sut.ContactsQueryService.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());

            //_sut.SaveCommandContact.Expect(call => call.Execute(Arg<Contact>.Matches(c =>
            //                                                                c.ContactDetail == "1234567" &&
            //                                                                c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE &&
            //                                                                c.IsMainContact)
            //                                                                ));
        }

    }
}
