using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using Core.Persistence;
using Web.Services;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    public class ObjectMother
    {
        public const string OUTPOST_NAME = "Spitalul Judetean";
        public const string PRODUCT_GROUP_NAME = "Malaria";
        public const string PRODUCT_GROUP_REFERENCE_CODE = "MAL";
        public const string SMS_REFERENCE_CODE = "R";
        public const string MANUAL_UPDATED_METHOD = "Manual";
        public const string SMS_UPDATED_METHOD = "SMS";
        public const int STOCK_LEVEL = 1;
        public const int RECEIVED_STOCK_LEVEL = 2;
        public const string PHONE_NUMBER = "1234567890";

        public Guid clientId;
        public Guid outpostId;
        public Guid productGroupId;
        public Guid productId;
        public Guid outpostStockLevelId;

        public IQueryService<Outpost> queryServiceOutpost;
        public IQueryService<ProductGroup> queryServiceProductGroup;
        public IQueryService<OutpostStockLevel> queryServiceStockLevel;
        public IQueryService<SmsRequest> queryServiceSmsRequest;
        public ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest;
        public ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel;
        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveCommandOutpostHistoricalStockLevel;

        public Client client;
        public Outpost outpost;
        public Outpost outpostWithNoMainContact;
        public Outpost outpostWithNoNumberContact;
        public ProductGroup productGroup;
        public Product product;
        public List<OutpostStockLevel> stockLevels;
        public SmsReceived smsReceived;
        public SmsRequest smsRequest;
        public SmsRequest smsRequestWithDifferentPhoneNumber;

        public IOutpostStockLevelService outpostStockLevelService;
        public ISmsRequestService smsRequestService;

        public void Setup_SmsRequestService_And_MockServices()
        {
            queryServiceOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryServiceProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryServiceStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryServiceSmsRequest = MockRepository.GenerateMock<IQueryService<SmsRequest>>();
            saveCommandSmsRequest = MockRepository.GenerateMock<ISaveOrUpdateCommand<SmsRequest>>();
            saveCommandOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            saveCommandOutpostHistoricalStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostHistoricalStockLevel>>();

            outpostStockLevelService = new OutpostStockLevelService(saveCommandOutpostHistoricalStockLevel);

            smsRequestService = new SmsRequestService(queryServiceOutpost, queryServiceProductGroup, queryServiceStockLevel, queryServiceSmsRequest,
                saveCommandSmsRequest, outpostStockLevelService, saveCommandOutpostStockLevel);
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
            outpost.Contacts = new Contact[] { new Contact() { ContactType = "Mobile Number", ContactDetail = PHONE_NUMBER, IsMainContact = true } }.ToList<Contact>();

            outpostWithNoMainContact = MockRepository.GeneratePartialMock<Outpost>();
            outpostWithNoMainContact.Stub(c => c.Id).Return(outpostId);
            outpostWithNoMainContact.Name = OUTPOST_NAME;
            outpostWithNoMainContact.Contacts = new Contact[] { new Contact() { ContactType = "Mobile Number", ContactDetail = PHONE_NUMBER, IsMainContact = false } }.ToList<Contact>();

            outpostWithNoNumberContact = MockRepository.GeneratePartialMock<Outpost>();
            outpostWithNoNumberContact.Stub(c => c.Id).Return(outpostId);
            outpostWithNoNumberContact.Name = OUTPOST_NAME;
            outpostWithNoNumberContact.Contacts = new Contact[] { new Contact() { ContactType = "Email", ContactDetail = "a@a.ro", IsMainContact = true } }.ToList<Contact>();

            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = PRODUCT_GROUP_NAME;
            productGroup.ReferenceCode = "MAL";

            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(p => p.Id).Return(productId);
            product.SMSReferenceCode = SMS_REFERENCE_CODE;

            outpostStockLevelId = Guid.NewGuid();
            OutpostStockLevel stockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            stockLevel.Stub(c => c.Id).Return(outpostStockLevelId);
            stockLevel.Product = product;
            stockLevel.StockLevel = STOCK_LEVEL;
            stockLevel.Outpost = outpost;
            stockLevel.ProductGroup = productGroup;
            stockLevel.UpdateMethod = MANUAL_UPDATED_METHOD;

            stockLevels = new OutpostStockLevel[] { stockLevel }.ToList<OutpostStockLevel>();

            smsReceived = new SmsReceived() {
                ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE,
                Number = PHONE_NUMBER,
                ReceivedStockLevels = new ReceivedStockLevel[] { 
                    new ReceivedStockLevel() { ProductSmsReference = SMS_REFERENCE_CODE, StockLevel = RECEIVED_STOCK_LEVEL } }.ToList()
                };

            smsRequest = new SmsRequest() { OutpostId = outpostId, ProductGroupId = productGroupId, Number = PHONE_NUMBER, Created = DateTime.Now, ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE };
            smsRequestWithDifferentPhoneNumber = new SmsRequest() { OutpostId = outpostId, ProductGroupId = productGroupId, Number = PHONE_NUMBER + "1", Created = DateTime.Now, ProductGroupReferenceCode = PRODUCT_GROUP_REFERENCE_CODE };
        }
    }
}
