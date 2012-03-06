using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Services;
using Domain;
using Web.Bootstrap;
using Rhino.Mocks;
using System.Web;
using Core.Persistence;
using Persistence.Commands;
using Persistence;
using NHibernate;
using Persistence.Queries.Outposts;
using Web.Helpers;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace IntegrationTests.SmsGateway_Integration_Test
{
    public class ObjectMother
    {
        private const string OUTPOST_NAME = "Spitalul Judetean";
        private const string PRODUCT_GROUP_NAME = "Malaria";
        private const string PRODUCT_GROUP_REFERENCE_CODE = "MAL";
        private const string MESSAGE = "MAL R0J0";
        private const string NUMBERS = "1234567890";
        private const string SMS_REFERENCE_CODE = "R";
        public const string CAMPAIGN_NAME = "Campaign Name";
        public const string PRODUCT_NAME = "Product";

        public string url;
        public string postRequest;

        public HttpService httpService;
        public SmsGatewaySettingsService smsGatewaySettingsService;
        public SmsGatewayService smsGatewayService;
        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<OutpostStockLevel> queryServiceStockLevel;
        public IQueryService<SmsRequest> queryServiceSmsRequest;
        public IQueryService<Contact> queryServiceContact;
        public ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest;
        private ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel;
        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel;
        public IQueryOutposts queryOutposts;

        public Guid clientId;
        public Client client;
        public Outpost outpost;
        public ProductGroup productGroup;
        public Product product;
        public OutpostStockLevel stockLevel;
        public Campaign campaign;
        public ProductLevelRequest productLevelRequest;
        public List<OutpostStockLevel> stockLevels;

        public IOutpostStockLevelService outpostStockLevelService;
        public SmsRequestService smsRequestService;

        public ISession session;


        public void SetupSmsGatewayService_and_HttpServices()
        {
            smsGatewaySettingsService = new SmsGatewaySettingsService();
            httpService = new HttpService();
            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            
            smsGatewayService = new SmsGatewayService(smsGatewaySettingsService, httpService, queryOutposts, queryServiceContact);

            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryServiceSmsRequest = MockRepository.GenerateMock<IQueryService<SmsRequest>>();
            saveCommandOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();

            saveCommandSmsRequest = new SaveCommandSmsRequest() { ObjectMother = this };
            smsRequestService = new SmsRequestService(queryServiceOutpost, queryServiceProductGroup, queryServiceStockLevel, queryServiceSmsRequest,
                saveCommandSmsRequest, outpostStockLevelService, saveCommandOutpostStockLevel, smsGatewayService);
        }

        public void SetUp_StubData()
        {
            clientId = Guid.NewGuid();

            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);

            session = SessionFactory.Instance.CreateSession();

            outpost = new Outpost();
            outpost.Name = OUTPOST_NAME;
            outpost.Contacts = new Contact[] { new Contact() { ContactType = "Mobile Number", ContactDetail = "1234567890", IsMainContact = true } }.ToList<Contact>();
            session.Save(outpost);

            productGroup = new ProductGroup();
            productGroup.Name = PRODUCT_GROUP_NAME;
            productGroup.ReferenceCode = PRODUCT_GROUP_REFERENCE_CODE;
            session.Save(productGroup);

            product = new Product { SMSReferenceCode = SMS_REFERENCE_CODE, Name = PRODUCT_NAME + " 0" };
            session.Save(product);

            stockLevel = new OutpostStockLevel { Product = product, StockLevel = 0, ProductGroup = productGroup, Outpost = outpost };

            session.Save(stockLevel);

            stockLevels = new OutpostStockLevel[] { stockLevel }.ToList<OutpostStockLevel>();
        }

        public void Setup_ProductLevelRequest()
        {
            campaign = new Campaign
            {
                Name = CAMPAIGN_NAME,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Opened = true,
                Options = ConvertHelper.StrToByteArray((ConvertHelper.ConvertToJSON(new OptionsModel { Outposts = outpost.Id.ToString() })))
            };
            session.Save(campaign);

            productLevelRequest = new ProductLevelRequest
            {
                Campaign = campaign,
                ProductGroup = productGroup
            };

            ProductModel[] model = new ProductModel[] { 
                new ProductModel { ProductItem = PRODUCT_NAME + " 0", Selected = true, SmsCode = SMS_REFERENCE_CODE },
                new ProductModel { ProductItem = PRODUCT_NAME + " 1", Selected = false, SmsCode = "P" } };
            productLevelRequest.StoreProducts<ProductModel[]>(model);
            session.Save(productLevelRequest);
        }

        public void Delete_StubData()
        {
            session.Delete(stockLevel);
            session.Delete(productGroup);
            session.Delete(stockLevel);
        }

        class SaveCommandSmsRequest : ISaveOrUpdateCommand<SmsRequest>
        {
            public ObjectMother ObjectMother { get; set; }

            public void Execute(SmsRequest entity)
            {
                ObjectMother.session.Save(entity);
            }
        }

    }
}
