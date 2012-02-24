using System;
using Rhino.Mocks;
using Domain;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.Product;
using System.Linq;
using System.Collections.Generic;
using Core.Domain;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductControllerTests
{
    public class ObjectMother
    {
        const string PRODUCTGROUP_NAME = "stockgroup1";
        const string PRODUCTGROUP_DESCRIPTION = "description23";

        const string PRODUCT_NAME = "StockItem1";
        const string PRODUCT_DESCRIPTION = "Description1";
        const string PRODUCT_SMSREFERENCE_CODE = "004";
        const int PRODUCT_LOWERLIMIT = 3;
        const int PRODUCT_UPPERLIMIT = 1000;
        
        public ProductGroup productGroup;
        public Product product;
        public Product product2;
        public OutpostStockLevel outpostStockLevel;
        public OutpostHistoricalStockLevel outpostHystoricalStockLevel;

        public IQueryService<User> queryUsers;

        public IQueryService<Client> queryClients;

        Guid outpostHystoricalStockLevelId;
        Guid outpostStockLevelId;
        Guid productId;
        Guid productId2;
        Guid productGroupId;

        private Client client;
        private User user;
        private const string FAKE_USERNAME = "fake-user-name";

        public ProductController controller;

        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        public IQueryService<ProductGroup> queryProductGroup;
        public ISaveOrUpdateCommand<Product> saveOrUpdateProduct;
        public IDeleteCommand<Product> deleteProduct;
        public IQueryService<Product> queryService;
        public IQueryService<OutpostHistoricalStockLevel> queryHistoricalOutpostStockLevel;

        
        public void Init()
        {
            BuildControllerAndServices();
            StubUserAndClient();
            StubProductGroup();
            StubProduct();
            StubProduct2();
            StubOutpostStockLevel();
            StubOutpostHystoricalStockLevel();
        }

        private void StubUserAndClient()
        {
            client = MockRepository.GeneratePartialMock<Client>();
            user = MockRepository.GeneratePartialMock<User>();

            client.Stub(p => p.Id).Return(Guid.NewGuid());
            client.Name = "Minoxsys";

            user.Stub(p => p.Id).Return(Guid.NewGuid());
            user.UserName = FAKE_USERNAME;
            user.ClientId = client.Id;

            queryClients.Expect(call => call.Load(client.Id)).Return(client);
            queryUsers.Expect(call => call.Query()).Return(new User[] { user }.AsQueryable());


        }

        internal void StubProduct()
        {
            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(b => b.Id).Return(productId);
            product.Name = PRODUCT_NAME;
            product.Description = PRODUCT_DESCRIPTION;
            product.ProductGroup = productGroup;
            product.LowerLimit = PRODUCT_LOWERLIMIT;
            product.UpperLimit = PRODUCT_UPPERLIMIT;
            product.SMSReferenceCode = PRODUCT_SMSREFERENCE_CODE;
            product.Client = client;
            product.ByUser = user;

        }
        internal void StubProduct2()
        {
            productId2 = Guid.NewGuid();
            product2 = MockRepository.GeneratePartialMock<Product>();
            product2.Stub(b => b.Id).Return(productId2);
            product2.Name = PRODUCT_NAME;
            product2.Description = PRODUCT_DESCRIPTION;
            product2.ProductGroup = productGroup;
            product2.LowerLimit = PRODUCT_LOWERLIMIT;
            product2.UpperLimit = PRODUCT_UPPERLIMIT;
            product2.SMSReferenceCode = PRODUCT_SMSREFERENCE_CODE;

            product2.Client = client;
            product2.ByUser = user;

        }
        internal void StubOutpostStockLevel()
        {
            outpostStockLevelId = Guid.NewGuid();
            outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            outpostStockLevel.Stub(b => b.Id).Return(outpostStockLevelId);
            outpostStockLevel.Product = product;
 
        }
        internal void StubOutpostHystoricalStockLevel()
        {
            outpostHystoricalStockLevelId = Guid.NewGuid();
            outpostHystoricalStockLevel = MockRepository.GeneratePartialMock<OutpostHistoricalStockLevel>();
            outpostHystoricalStockLevel.Stub(it => it.Id).Return(outpostHystoricalStockLevelId);
            outpostHystoricalStockLevel.ProductId = product.Id;
        }
        internal void StubProductGroup()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(b => b.Id).Return(productGroupId);
            productGroup.Name = PRODUCTGROUP_NAME;
            productGroup.Description = PRODUCTGROUP_DESCRIPTION;

        }

        internal void BuildControllerAndServices()
        {
            controller = new ProductController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);


            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveOrUpdateProduct = MockRepository.GenerateMock<ISaveOrUpdateCommand<Product>>();
            deleteProduct = MockRepository.GenerateMock<IDeleteCommand<Product>>();
            queryService = MockRepository.GenerateMock<IQueryService<Product>>();
            queryHistoricalOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostHistoricalStockLevel>>();
            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();

            queryClients = MockRepository.GenerateMock<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();

            controller.QueryOutpostStockLevel = queryOutpostStockLevel;
            controller.QueryProductGroup = queryProductGroup;
            controller.SaveOrUpdateProduct = saveOrUpdateProduct;
            controller.DeleteProduct = deleteProduct;
            controller.QueryService = queryService;
            controller.QueryOutpostStockLevelHystorical = queryHistoricalOutpostStockLevel;

            controller.QueryClients = queryClients;
            controller.QueryUsers = queryUsers;

        }

        internal IQueryable<Product> PageOfProductData(ProductIndexModel indexModel)
        {
            List<Product> productPageList = new List<Product>();

            for (int i = indexModel.Start.Value; i < indexModel.Limit.Value; i++)
            {
                productPageList.Add(new Product
                {
                    Client = client,
                    ByUser = user,
                    Name = String.Format("Product{0}", i),
                    Description = product.Description,
                    ProductGroup = productGroup
                });
            }
            return productPageList.AsQueryable();
        }

        internal void VerifyUserAndClientQueries()
        {
            controller.QueryUsers.VerifyAllExpectations();
            controller.QueryClients.VerifyAllExpectations();
        }
    }
}
