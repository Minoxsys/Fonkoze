using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.AnalysisManagement.Controllers;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;


namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportDistrictLevelControllerTests
{
    public class ObjectMother
    {
        public ReportDistrictLevelController controller;

        public IQueryService<Outpost> queryOutposts;
        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        public IQueryService<User> queryUsers;
        public IQueryService<Client> queryClients;

        public Country country;
        public Region region;
        public List<District> districts;
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
        public Guid districtId;
        public Guid outpostId;
        public Guid productGroupId;
        public Guid productId;
        public Guid outpostStockLevelId;

        const String REGION_NAME = "region";
        const String COUNTRY_NAME = "country";
        const String DISTRICT_NAME = "district";
        const String OUTPOST_NAME = "outpost";
        const String PRODUCT_GROUP_NAME = "productGroup";
        const String PRODUCT_NAME = "product";

        const string USER_NAME = "admin";
        const string NAME_ALL_OPTION = "All";
        public Guid ID_ALL_OPTION_FOR_COUNTRIES = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public Guid ID_ALL_OPTION_FOR_REGIONS = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public Guid ID_ALL_OPTION_FOR_DISTRICTS = Guid.Parse("00000000-0000-0000-0000-000000000003");

        public void Init()
        {
            SetUpServices();
            SetUpController();
            StubUserAndItsClient();
            StubCountry();
            StubRegion();
            StubDistrictsForRegion();
            StubProductGroup();
            StubProduct();            
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
            controller = new ReportDistrictLevelController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryOutpost = queryOutposts;
            controller.QueryClient = queryClients;
            controller.QueryUser = queryUsers;
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

        internal void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
        }
        internal void StubDistrictsForRegion()
        {
            districts = new List<District>();
            for (int i = 0; i < 3; i++)
            {
                districtId = Guid.NewGuid();
                var district = MockRepository.GeneratePartialMock<District>();
                district.Stub(b => b.Id).Return(districtId);
                district.Name = DISTRICT_NAME + i;
                district.Region = region;
                districts.Add(district);
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
                    outpost.Region = region;
                    outpost.District = districts[i];

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
                    outpost.Region = region;
                    outpost.District = districts[2];

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
