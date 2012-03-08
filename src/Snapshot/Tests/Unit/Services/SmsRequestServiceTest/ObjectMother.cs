using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using Core.Persistence;
using Web.Services;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Helpers;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    public class ObjectMother
    {
        public const string OUTPOST_NAME = "Spitalul Judetean";
        public const string PRODUCT_GROUP_NAME = "Malaria";
        public const string PRODUCT_GROUP_REFERENCE_CODE = "MAL";
        public const string SMS_REFERENCE_CODE = "A";
        public const string PRODUCT_NAME = "Product";
        public const string MANUAL_UPDATED_METHOD = "Manual";
        public const string SMS_UPDATED_METHOD = "SMS";
        public const int STOCK_LEVEL = 1;
        public const int RECEIVED_STOCK_LEVEL = 2;
        public const string PHONE_NUMBER = "1234567890";

        public const string CAMPAIGN_NAME = "Campaign Name";
        public const string MESSAGE_NOT_DELIVERED = "Message not delivered";

        public Guid productLevelRequestId;
        public Guid clientId;
        public Guid outpostId;
        public Guid productGroupId;
        public Guid productId;
        public Guid outpostStockLevelId;

        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<Product> queryServiceProduct;
        public IQueryService<OutpostStockLevel> queryServiceStockLevel;
        public IQueryService<SmsRequest> queryServiceSmsRequest;
        public ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest;
        public ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel;
        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel;

        public ProductLevelRequest productLevelRequest;
        public Campaign campaign;
        public Client client;
        public Outpost outpost;
        public Outpost outpostWithNoMainContact;
        public Outpost outpostWithNoNumberContact;
        public ProductGroup productGroup;
        public List<OutpostStockLevel> stockLevels;
        public SmsReceived smsReceived;
        public SmsRequest smsRequest;
        public SmsRequest smsRequestWithDifferentPhoneNumber;
        public ProductLevelRequestMessageInput productLevelRequestMessageInput;
        public ProductLevelRequestMessageInput productLevelRequestMessageInputWithoutProducts;
        public ProductLevelRequestMessageInput productLevelRequestMessageInputWithoutMobileNumber;

        public IOutpostStockLevelService outpostStockLevelService;
        public SmsRequestService smsRequestService;
        public ISmsGatewayService smsGatewayService;

        public void Setup_SmsRequestService_And_MockServices()
        {
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceProduct = MockRepository.GenerateMock<IQueryService<Product>>();
            queryServiceStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryServiceSmsRequest = MockRepository.GenerateMock<IQueryService<SmsRequest>>();
            saveCommandSmsRequest = MockRepository.GenerateMock<ISaveOrUpdateCommand<SmsRequest>>();
            saveCommandOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            saveCommandOutpostHistoricalStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostHistoricalStockLevel>>();

            smsGatewayService = MockRepository.GenerateMock<ISmsGatewayService>();

            outpostStockLevelService = new OutpostStockLevelService(saveCommandOutpostHistoricalStockLevel);

            smsRequestService = new SmsRequestService(queryServiceOutpost, queryServiceProductGroup, queryServiceStockLevel, queryServiceSmsRequest,
                saveCommandSmsRequest, outpostStockLevelService, saveCommandOutpostStockLevel, smsGatewayService);
        }

        public void Setup_Stub_Data()
        {
            clientId = Guid.NewGuid();

            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);

            outpostId = Guid.NewGuid();
            productGroupId = Guid.NewGuid();
            productId = Guid.NewGuid();

            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = OUTPOST_NAME;
            outpost.Contacts = new Contact[] { new Contact() { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = PHONE_NUMBER, IsMainContact = true } }.ToList<Contact>();

            outpostWithNoMainContact = MockRepository.GeneratePartialMock<Outpost>();
            outpostWithNoMainContact.Stub(c => c.Id).Return(outpostId);
            outpostWithNoMainContact.Name = OUTPOST_NAME;
            outpostWithNoMainContact.Contacts = new Contact[] { new Contact() { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = PHONE_NUMBER, IsMainContact = false } }.ToList<Contact>();

            outpostWithNoNumberContact = MockRepository.GeneratePartialMock<Outpost>();
            outpostWithNoNumberContact.Stub(c => c.Id).Return(outpostId);
            outpostWithNoNumberContact.Name = OUTPOST_NAME;
            outpostWithNoNumberContact.Contacts = new Contact[] { new Contact() { ContactType = Contact.EMAIL_CONTACT_TYPE, ContactDetail = "a@a.ro", IsMainContact = true } }.ToList<Contact>();

            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = PRODUCT_GROUP_NAME;
            productGroup.ReferenceCode = PRODUCT_GROUP_REFERENCE_CODE;

            stockLevels = GenerateListOfOutpostStockLevels();

            smsReceived = new SmsReceived() {
                ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE,
                Number = PHONE_NUMBER,
                ReceivedStockLevels = new ReceivedStockLevel[] { 
                    new ReceivedStockLevel() { ProductSmsReference = stockLevels[0].Product.SMSReferenceCode, StockLevel = RECEIVED_STOCK_LEVEL } }.ToList()
                };

            smsRequest = new SmsRequest() { OutpostId = outpostId, ProductGroupId = productGroupId, Number = PHONE_NUMBER, Created = DateTime.Now, ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE };
            smsRequestWithDifferentPhoneNumber = new SmsRequest() { OutpostId = outpostId, ProductGroupId = productGroupId, Number = PHONE_NUMBER + "1", Created = DateTime.Now, ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE };
        }

        private List<OutpostStockLevel> GenerateListOfOutpostStockLevels()
        {
            List<OutpostStockLevel> stockLevels = new List<OutpostStockLevel>();

            for (int i = 0; i < 2; i++)
            {
                Guid productId = Guid.NewGuid();
                Product product = MockRepository.GeneratePartialMock<Product>();
                product.Stub(p => p.Id).Return(productId);
                product.SMSReferenceCode = Char.ConvertFromUtf32(i + (int)'A');
                product.Name = PRODUCT_NAME + " " + i;

                Guid stockLevelId = Guid.NewGuid();
                OutpostStockLevel stockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>(); ;
                stockLevel.Stub(c => c.Id).Return(outpostStockLevelId);
                stockLevel.Product = product;
                stockLevel.StockLevel = STOCK_LEVEL;
                stockLevel.Outpost = outpost;
                stockLevel.ProductGroup = productGroup;
                stockLevel.UpdateMethod = MANUAL_UPDATED_METHOD;

                stockLevels.Add(stockLevel);
            }

            return stockLevels;
        }

        public void Setup_ProductLevelRequest()
        {
            campaign = new Campaign
            {
                Name = CAMPAIGN_NAME,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Opened = true,
                Options = ConvertHelper.StrToByteArray((ConvertHelper.ConvertToJSON(new OptionsModel { Outposts = outpostId.ToString() })))
            };

            productLevelRequest = new ProductLevelRequest
            {
                Campaign = campaign,
                ProductGroup = productGroup
            };

            productLevelRequestId = Guid.NewGuid();

            ProductModel[] model = new ProductModel[] { 
                new ProductModel { ProductItem = PRODUCT_NAME + " 0", Selected = true, SmsCode = SMS_REFERENCE_CODE },
                new ProductModel { ProductItem = PRODUCT_NAME + " 1", Selected = false, SmsCode = "P" } };
            productLevelRequest.StoreProducts<ProductModel[]>(model);
        }

        public void Setup_ProductLevelRequestMessageInput()
        {
            Product product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(p => p.Id).Return(productId);
            product.SMSReferenceCode = SMS_REFERENCE_CODE;
            product.Name = PRODUCT_NAME ;

            productLevelRequestMessageInput = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = PHONE_NUMBER },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { product }.ToList()
            };

            productLevelRequestMessageInputWithoutProducts = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactDetail = PHONE_NUMBER },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { }.ToList()
            };

            productLevelRequestMessageInputWithoutMobileNumber = new ProductLevelRequestMessageInput
            {
                Client = client,
                Contact = new Contact { ContactType = Contact.EMAIL_CONTACT_TYPE, ContactDetail = "" },
                Outpost = outpost,
                ProductGroup = productGroup,
                Products = new Product[] { product }.ToList()
            };
        }
    }
}
