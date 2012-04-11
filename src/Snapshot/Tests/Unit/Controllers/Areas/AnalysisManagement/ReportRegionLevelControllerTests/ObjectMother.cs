using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.AnalysisManagement.Controllers;
using Domain;
using Core.Persistence;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportRegionLevelControllerTests
{
    public class ObjectMother
    {
        public ReportRegionLevelController controller;

        public IQueryService<Outpost> queryOutposts;
        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        public IQueryService<User> queryUsers;
        public IQueryService<Client> queryClients;

        public Country country;
        public List<Region> regions;
        public Outpost outpost;
        public ProductGroup productGroup;
        public Product product;
        public List<OutpostStockLevel> outpostStockLevelList;
        public Client client;
        public User user;

        public Guid clientId;
        public Guid userId;
        public Guid countryId;
        public Guid regionId;
        public Guid outpostId;
        public Guid productGroupId;
        public Guid productId;
        public Guid outpostStockLevelId;

        const String REGION_NAME = "region";
        const String COUNTRY_NAME = "country";
        const String OUTPOST_NAME = "outpost";
        const String PRODUCT_GROUP_NAME = "productGroup";
        const String PRODUCT_NAME = "product";

        const string USER_NAME = "admin";
        const string NAME_ALL_OPTION = "All";
        private Guid ID_ALL_OPTION_FOR_COUNTRIES = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private Guid ID_ALL_OPTION_FOR_REGIONS = Guid.Parse("00000000-0000-0000-0000-000000000002");

        public void Init()
        {
            SetUpServices();
            SetUpController();
            StubUserAndItsClient();
            StubCountry();
            StubRegionsForCountry();
            StubProductGroup();
            StubProduct();
            StubOutpost();
            StubOutpostStockLevelList();
            
        }

        internal void SetUpServices()
        {
            queryClients = MockRepository.GenerateMock<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
                       
        }

        internal void SetUpController()
        {
            controller = new ReportRegionLevelController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryOutpost = queryOutposts;
            controller.QueryClients = queryClients;
            controller.QueryUsers = queryUsers;
            controller.QueryOutpostStockLevel = queryOutpostStockLevel;     

        }

        internal void StubUserAndItsClient()
        {
            this.client = MockRepository.GeneratePartialMock<Client>();
            clientId = Guid.NewGuid();

            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Minoxsys";

            this.user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USER_NAME;
            user.Password = "asdf";

            queryClients.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());            
        }

        internal void StubProductGroup()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(b => b.Id).Return(productGroupId);
            productGroup.Name = PRODUCT_GROUP_NAME;
 
        }

        internal void StubProduct()
        {
            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(b => b.Id).Return(productId);
            product.Name = PRODUCT_NAME;
            product.ProductGroup = productGroup;
        }

        internal void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = COUNTRY_NAME;

        }

        internal void StubOutpost()
        {
           
        }
        internal void StubRegionsForCountry()
        {
            regions = new List<Region>();
            for (int i = 0; i < 3; i++)
            {
                regionId = Guid.NewGuid();
                var region = MockRepository.GeneratePartialMock<Region>();
                region.Stub(b => b.Id).Return(regionId);
                region.Name = REGION_NAME + i;
                region.Country = country;
                regions.Add(region);
            }

        }

        internal void StubOutpostStockLevelList()
        {
            outpostStockLevelList = new List<OutpostStockLevel>();

            for (int i = 0; i < 4; i++)
            {
                outpostStockLevelId = Guid.NewGuid();
                var outpostStockLevel = new OutpostStockLevel();
                outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
                outpostStockLevel.Stub(o => o.Id).Return(outpostStockLevelId);
                outpostStockLevel.Client = client;

                if (i < 2)
                {
                    outpost = new Outpost();
                    outpostId = Guid.NewGuid();
                    outpost = MockRepository.GeneratePartialMock<Outpost>();
                    outpost.Stub(b => b.Id).Return(outpostId);
                    outpost.Name = OUTPOST_NAME;
                    outpost.Country = country;
                    outpost.Region = regions[i];

                    outpostStockLevel.Outpost = outpost;
                    outpostStockLevel.Product = product;
                    outpostStockLevel.ProductGroup = productGroup;
                    outpostStockLevel.StockLevel = i;
                }

                if (i >= 2)
                {
                    outpost = new Outpost();
                    outpostId = Guid.NewGuid();
                    outpost = MockRepository.GeneratePartialMock<Outpost>();
                    outpost.Stub(b => b.Id).Return(outpostId);
                    outpost.Name = OUTPOST_NAME;
                    outpost.Country = country;
                    outpost.Region = regions[2];

                    outpostStockLevel.Outpost = outpost;
                    outpostStockLevel.Product = product;
                    outpostStockLevel.ProductGroup = productGroup;
                    outpostStockLevel.StockLevel = i;
 
                }
                outpostStockLevelList.Add(outpostStockLevel);
            }

        }
    }
}
