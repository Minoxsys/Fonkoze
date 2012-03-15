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

namespace Tests.Unit.Services.EmailRequestServiceTests
{
    public class ObjectMother
    {
        public const string PRODUCT_NAME = "Product";
        public const string E_MAIL = "a@a.com";

        public Guid outpostId;
        public Guid productGroupId;
        public Guid contactId;
        public Guid emailRequestId;

        public Outpost outpost;
        public ProductGroup productGroup;
        public Product product;
        public Contact contact;
        public EmailRequest emailRequest;
        public ProductLevelRequestMessageInput input;
        public Client client;
        public Guid productId;
        public ProductLevelRequestMessageInput productLevelRequestMessageInput;
        public ProductLevelRequestMessageInput productLevelRequestMessageInputWithoutProducts;
        public ProductLevelRequestMessageInput productLevelRequestMessageInputWithoutEmail;

        public IQueryService<EmailRequest> queryServiceEmailRequest;
        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<Contact> queryServiceContact;
        public ISaveOrUpdateCommand<EmailRequest> saveOrUpdateCommandEmailRequest;

        public IURLService urlService;
        public IEmailService emailService;

        public EmailRequestService emailRequestService;

        public void Setup_EmailRequestService_and_MockServices()
        {
            queryServiceEmailRequest = MockRepository.GenerateMock<IQueryService<EmailRequest>>();
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            saveOrUpdateCommandEmailRequest = MockRepository.GenerateMock<ISaveOrUpdateCommand<EmailRequest>>();

            urlService = MockRepository.GenerateMock<IURLService>();
            emailService = MockRepository.GenerateMock<IEmailService>();


            emailRequestService = new EmailRequestService(saveOrUpdateCommandEmailRequest, urlService, emailService);
        }

        public void SetUp_StubData()
        {
            outpostId = Guid.NewGuid();
            productId = Guid.NewGuid();
            productGroupId = Guid.NewGuid();

            client = new Client();

            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(o => o.Id).Return(outpostId);

            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(p => p.Id).Return(productGroupId);

            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(p => p.Id).Return(productId);
            product.Name = PRODUCT_NAME;
        }

        public void Setup_ProductLevelRequestMessageInput()
        {

            productLevelRequestMessageInput = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactType = Contact.EMAIL_CONTACT_TYPE, ContactDetail = E_MAIL },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { product }.ToList()
            };

            productLevelRequestMessageInputWithoutProducts = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactDetail = E_MAIL },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { }.ToList()
            };

            productLevelRequestMessageInputWithoutEmail = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = "" },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { product }.ToList()
            };
        }

        public ProductLevelRequestMessageInput ProductLevelRequestMessageInputWithNullContact()
        {
            return new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = null,
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { product }.ToList()
            };

        }
    }
}
