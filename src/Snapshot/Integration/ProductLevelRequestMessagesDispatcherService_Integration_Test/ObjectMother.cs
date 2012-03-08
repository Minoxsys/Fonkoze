using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Web.Services;
using Core.Persistence;
using Rhino.Mocks;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using NHibernate;
using Persistence.Queries.Outposts;
using Web.Services.EmailService;
using Web.Services.UrlService;

namespace IntegrationTests.ProductLevelRequestMessagesDispatcherService_Integration_Test
{
    public class ObjectMother
    {
        public const string PRODUCT_GROUP_NAME = "Malaria";
        public const string PRODUCT_GROUP_REFERENCE_CODE = "Mal";
        public const string PRODUCT_NAME_1 = "Red";
        public const string PRODUCT_NAME_2 = "Green";
        public const string SMS_REFERENCE_CODE_1 = "R";
        public const string SMS_REFERENCE_CODE_2 = "G";

        public ProductLevelRequestMessagesDispatcherService service;

        public IQueryService<Contact> queryServiceContact;
        public IQueryService<OutpostStockLevel> queryServiceOutpostStockLevel;
        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryOutposts queryOutposts;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<SmsRequest> queryServiceSmsRequest;
        public IOutpostStockLevelService outpostStockLevelService;
        public ISaveOrUpdateCommand<OutpostStockLevel> saveOrUpdateOutpostStockLevel;
        public ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord;
        public IURLService urlService;

        public ProductLevelRequest productLevelRequest;

        public Outpost outpost1;
        public Outpost outpost2;
        public Contact contact1;
        public Contact contact2;
        public OutpostStockLevel outpostStockLevel1;
        public OutpostStockLevel outpostStockLevel2;
        public Product product1;
        public Product product2;
        public SmsRequest smsRequest;
        public EmailRequest emailRequest;
        public RequestRecord requestRecord;

        public ISession session;

        public void Setup_Stub_Data()
        {
            session = SessionFactory.Instance.CreateSession();

            contact1 = new Contact() { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = "1234567890", IsMainContact = true };
            contact2 = new Contact() { ContactType = Contact.EMAIL_CONTACT_TYPE, ContactDetail = "georgian.camarasan@evozon.com", IsMainContact = true };

            outpost1 = new Outpost();
            outpost1.Contacts = new Contact[] { contact1 }.ToList<Contact>();
            session.Save(outpost1);

            contact1.Outpost = outpost1;

            outpost2 = new Outpost();
            outpost2.Contacts = new Contact[] { contact2 }.ToList<Contact>();
            session.Save(outpost2);

            contact2.Outpost = outpost2;

            Campaign campaign = new Campaign();
            campaign.StoreOptions<OptionsModel>(new OptionsModel { Outposts = outpost1.Id + "," + outpost2.Id });

            product1 = new Product()
            {
                Name = PRODUCT_NAME_1,
                SMSReferenceCode = SMS_REFERENCE_CODE_1
            };
            session.Save(product1);

            product2 = new Product()
            {
                Name = PRODUCT_NAME_2,
                SMSReferenceCode = SMS_REFERENCE_CODE_2
            };
            session.Save(product2);

            outpostStockLevel1 = new OutpostStockLevel
            {
                Outpost = outpost1,
                Product = product1
            };
            session.Save(outpostStockLevel1);

            outpostStockLevel2 = new OutpostStockLevel
            {
                Outpost = outpost2,
                Product = product2
            };
            session.Save(outpostStockLevel2);

            Client client = new Client();

            ProductGroup productGroup = new ProductGroup 
            {
                Name = PRODUCT_GROUP_NAME,
                ReferenceCode = PRODUCT_GROUP_REFERENCE_CODE,
                Client = client
            };

            productLevelRequest = new ProductLevelRequest()
            {
                Campaign = campaign,
                Client = client,
                ProductGroup = productGroup
            };

            ProductModel[] modelWithOneProduct = new ProductModel[] { 
                new ProductModel { ProductItem = PRODUCT_NAME_1 , Selected = true, SmsCode = SMS_REFERENCE_CODE_1 }, 
                new ProductModel { ProductItem = PRODUCT_NAME_2 , Selected = true, SmsCode = SMS_REFERENCE_CODE_2 }
            };

            productLevelRequest.StoreProducts<ProductModel[]>(modelWithOneProduct);
        }

        public void Setup_DispatcherService_And_Required_Services()
        {
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            queryServiceOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceSmsRequest = MockRepository.GenerateMock<IQueryService<SmsRequest>>();
            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            saveOrUpdateRequestRecord = MockRepository.GenerateMock<ISaveOrUpdateCommand<RequestRecord>>();

            IProductLevelRequestMessageSenderService smsRequestService = new SmsRequestService(queryServiceOutpost, queryServiceProductGroup, queryServiceOutpostStockLevel,
                queryServiceSmsRequest, new SaveCommandSmsRequest(this), outpostStockLevelService, saveOrUpdateOutpostStockLevel,
                new SmsGatewayService(new SmsGatewaySettingsService(), new HttpService(), queryOutposts, queryServiceContact));

            urlService = MockRepository.GenerateMock<IURLService>();

            EmailRequestService emailRequestService = new EmailRequestService(new SaveCommandEmailRequest(this), urlService, new EmailService());

            service = new ProductLevelRequestMessagesDispatcherService(queryServiceOutpost, queryServiceContact, queryServiceOutpostStockLevel,
                saveOrUpdateRequestRecord, new List<IProductLevelRequestMessageSenderService> { smsRequestService, emailRequestService });
        }

        public void Delete_StubData()
        {
            session.Delete(outpost1);
            session.Delete(outpost2);
            session.Delete(product1);
            session.Delete(product2);
            session.Delete(outpostStockLevel1);
            session.Delete(outpostStockLevel2);
            
            if (smsRequest != null)
            {
                session.Delete(smsRequest);
            }

            if (emailRequest != null)
            {
                session.Delete(emailRequest);
            }

            if (requestRecord != null)
            {
                session.Delete(requestRecord);
            }
        }

        class SaveCommandSmsRequest : ISaveOrUpdateCommand<SmsRequest>
        {
            private ObjectMother _;

            public SaveCommandSmsRequest(ObjectMother objectMother)
            {
                _ = objectMother;
            }

            public void Execute(SmsRequest entity)
            {
                _.session.Save(entity);
                _.smsRequest = entity;
            }
        }

        class SaveCommandEmailRequest : ISaveOrUpdateCommand<EmailRequest>
        {
            private ObjectMother _;

            public SaveCommandEmailRequest(ObjectMother objectMother)
            {
                _ = objectMother;
            }

            public void Execute(EmailRequest entity)
            {
                _.session.Save(entity);
                _.emailRequest = entity;
            }
        }

        class SaveCommandRequestRecord : ISaveOrUpdateCommand<RequestRecord>
        {
            private ObjectMother _;

            public SaveCommandRequestRecord(ObjectMother objectMother)
            {
                _ = objectMother;
            }

            public void Execute(RequestRecord entity)
            {
                _.session.Save(entity);
                _.requestRecord = entity;
            }
        }
    }
}
