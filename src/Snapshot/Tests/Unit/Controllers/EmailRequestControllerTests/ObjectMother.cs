using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using Rhino.Mocks;
using System.Net.Mail;
using Web.Controllers;
using Web.Services;

namespace Tests.Unit.Controllers.EmailRequestControllerTests
{
    public class ObjectMother
    {
        public const string DEFAULT_VIEW_NAME = "";

        public Guid outpostId;
        public Guid productGroupId;
        public Guid contactId;
        public Guid emailRequestId;

        public Outpost outpost;
        public ProductGroup productGroup;
        public Contact contact;
        public EmailRequest emailRequest;
        
        public EmailRequestController controller;

        public ISaveOrUpdateCommand<EmailRequest> saveCommandEmailRequest;
        public IQueryService<EmailRequest> queryServiceEmailRequest;
        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<Contact> queryServiceContact;

        public IURLService urlService;
        public FakeEmailService emailService;

        public void Setup_Controller_and_MockServices()
        {
            controller = new EmailRequestController();

            saveCommandEmailRequest = MockRepository.GenerateMock<ISaveOrUpdateCommand<EmailRequest>>();
            queryServiceEmailRequest = MockRepository.GenerateMock<IQueryService<EmailRequest>>();
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();

            urlService = MockRepository.GenerateMock<IURLService>();
            emailService = new FakeEmailService();

            controller.SaveOrUpdateCommand = saveCommandEmailRequest;
            controller.QueryEmailRequest = queryServiceEmailRequest;
            controller.QueryOutpost = queryServiceOutpost;
            controller.QueryProductGroup = queryServiceProductGroup;
            controller.QueryContact = queryServiceContact;

            controller.UrlService = urlService;
            controller.EmailService = emailService;
        }

        public void SetUp_StubData()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "Spitalul Judetean";
            outpost.Country = new Country();
            outpost.Region = new Region();
            outpost.District = new District();

            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = "AIDS";

            contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.Stub(c => c.Id).Return(contactId);
            contact.IsMainContact = true;
            contact.Outpost = outpost;
            contact.ContactType = Contact.EMAIL_CONTACT_TYPE;
            contact.ContactDetail = "eu@yahoo.com";

            emailRequestId = Guid.NewGuid();
            emailRequest = MockRepository.GeneratePartialMock<EmailRequest>();
            emailRequest.Date = DateTime.Today;
            emailRequest.OutpostId = outpostId;
            emailRequest.ProductGroupId = productGroupId;
        }
    }

    public class FakeEmailService : IEmailService
    {
        public bool _sendMailMessageCalled = false;

        public bool SendMail(MailMessage message)
        {
            _sendMailMessageCalled = true;
            return true;
        }
    }

}
