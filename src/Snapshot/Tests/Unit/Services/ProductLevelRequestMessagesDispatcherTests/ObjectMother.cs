using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Services;
using Domain;
using Web.Helpers;
using Web.Areas.CampaignManagement.Models.Campaign;
using Rhino.Mocks;
using Core.Persistence;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Core.Domain;

namespace Tests.Unit.Services.ProductLevelRequestMessagesDispatcherTests
{
    public class ObjectMother
    {
        public const string CAMPAIGN_NAME = "Campaign Name";
        public const string PRODUCT_NAME = "Product Name";
        public const string PRODUCT_GROUP_NAME = "Malaria";
        public const string SMS_REFERENCE_CODE = "A";
        public const string OUTPOST_NAME = "Outpost Name";

        public ProductLevelRequestMessagesDispatcherService dispatcherService;

        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<Contact> queryServiceContact;
        public IQueryService<OutpostStockLevel> queryServiceStockLevel;
        public IProductLevelRequestMessageSenderService senderService;
        public ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord;

        public Guid outpostId;
        public Outpost outpost;
        public ProductLevelRequest productLevelRequestWithOneProduct;
        public Campaign campaign;
        public ProductGroup productGroup;
        public Contact contact;
        public OutpostStockLevel outpostStockLevel1;
        public OutpostStockLevel outpostStockLevel2;
        public Client client;
        public User byUser;

        public void Setup_Dispatcher_Service_And_QueryServices()
        {
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            queryServiceStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            senderService = MockRepository.GenerateMock<IProductLevelRequestMessageSenderService>();
            saveOrUpdateRequestRecord = MockRepository.GenerateMock<ISaveOrUpdateCommand<RequestRecord>>();

            dispatcherService = new ProductLevelRequestMessagesDispatcherService(queryServiceOutpost, queryServiceContact, queryServiceStockLevel, 
                saveOrUpdateRequestRecord, new List<IProductLevelRequestMessageSenderService> { senderService });
        }

        public void Setup_Stub_Data()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(o => o.Id).Return(outpostId);
            outpost.Name = OUTPOST_NAME;

            client = new Client();
            byUser = new User();

            contact = new Contact { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, IsMainContact = true, ContactDetail = "1234567890", Outpost = outpost };

            campaign = MockRepository.GeneratePartialMock<Campaign>();
            campaign.Stub(c => c.Id).Return(Guid.NewGuid());
            campaign.Name = CAMPAIGN_NAME;
            campaign.StoreOptions<OptionsModel>(new OptionsModel { Outposts = outpostId.ToString() });

            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(p => p.Id).Return(Guid.NewGuid());
            productGroup.Name = PRODUCT_GROUP_NAME;

            productLevelRequestWithOneProduct = MockRepository.GeneratePartialMock<ProductLevelRequest>();
            productLevelRequestWithOneProduct.Stub(p => p.Id).Return(Guid.NewGuid());
            productLevelRequestWithOneProduct.Campaign = campaign;
            productLevelRequestWithOneProduct.ProductGroup = productGroup;
            productLevelRequestWithOneProduct.Client = client;
            productLevelRequestWithOneProduct.ByUser = byUser;

            outpostStockLevel1 = new OutpostStockLevel 
            {
                Product = new Product { Name = PRODUCT_NAME },
                Outpost = outpost
            };

            outpostStockLevel2 = new OutpostStockLevel
            {
                Product = new Product { Name = PRODUCT_NAME + "2" },
                Outpost = outpost
            };

            ProductModel[] modelWithOneProduct = new ProductModel[] { 
                new ProductModel { ProductItem = PRODUCT_NAME , Selected = true, SmsCode = SMS_REFERENCE_CODE }};
            productLevelRequestWithOneProduct.StoreProducts<ProductModel[]>(modelWithOneProduct);
        }
    }
}
