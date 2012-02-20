using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.StockAdministration.Controllers;
using Domain;
using Core.Persistence;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.StockAdministration.HistoricalProductLevelControllerTests
{
    public class ObjectMother
    {
        public HistoricalProductLevelController controller;

        public IQueryService<Outpost> queryOutposts;
        public IQueryService<ProductGroup> queryProductGroups;
        public IQueryService<Product> queryProducts;
        public IQueryService<Client> loadClient;
        public IQueryService<User> queryUsers;
        public IQueryService<OutpostHistoricalStockLevel> queryHistorical;


        public Country country;
        public Region region;
        public District district;
        public Outpost outpost1;
        public Outpost outpost2;
        public ProductGroup productGroup;
        public Product product;
        public Client client;
        public User user;
        public OutpostHistoricalStockLevel historical;

        public Guid countryId;
        public Guid regionId;
        public Guid districtId;
        public Guid outpostId1;
        public Guid outpostId2;
        public Guid productGroupId;
        public Guid productId;
        public Guid clientId;
        public Guid userId;
        public Guid historicalId;

        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
            StubUserAndItsClient();
        }

        private void MockServices()
        {
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryProductGroups = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryProducts = MockRepository.GenerateMock<IQueryService<Product>>();
            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();
            queryHistorical = MockRepository.GenerateMock<IQueryService<OutpostHistoricalStockLevel>>();

        }
        private void Setup_Controller()
        {
            controller = new HistoricalProductLevelController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryOutpost = queryOutposts;
            controller.QueryProductGroup = queryProductGroups;
            controller.QueryProduct = queryProducts;
            controller.QueryClients = loadClient;
            controller.QueryUsers = queryUsers;
            controller.QueryHistorical = queryHistorical;
        }

        private void SetUp_StubData()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = "Romania";
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Coordinates = "2 10";
            region.Country = country;
            region.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = "Cluj";
            district.Region = region;
            district.Client = client;

            outpostId1 = Guid.NewGuid();
            outpost1 = MockRepository.GeneratePartialMock<Outpost>();
            outpost1.Stub(c => c.Id).Return(outpostId1);
            outpost1.Client = client;
            outpost1.Country = country;
            outpost1.Region = region;
            outpost1.District = district;
            outpost1.Name = "Spitalul Judetean";

            outpostId2 = Guid.NewGuid();
            outpost2 = MockRepository.GeneratePartialMock<Outpost>();
            outpost2.Stub(c => c.Id).Return(outpostId2);
            outpost2.Client = client;
            outpost2.Country = country;
            outpost2.Region = region;
            outpost2.District = district;
            outpost2.Name = "Spitalul Maria";

            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = "Malaria";

            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(c => c.Id).Return(productId);
            product.Name = "Product1";
            product.ProductGroup = productGroup;

            historicalId = Guid.NewGuid();
            historical = MockRepository.GeneratePartialMock<OutpostHistoricalStockLevel>();
            historical.Stub(c => c.Id).Return(historicalId);
            historical.OutpostId = outpostId1;
            historical.ProductGroupId = productGroupId;
            historical.UpdateDate = DateTime.UtcNow;

        }

        public void StubUserAndItsClient()
        {
            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = CLIENT_NAME;

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USER_NAME;
            user.Password = "4321";

            loadClient.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            controller.QueryClients = loadClient;
            controller.QueryUsers = queryUsers;

        }

        public IQueryable<Outpost> CurrentUserOutposts()
        {
            var listOfOutposts = new List<Outpost>();
            var client = this.client;

            listOfOutposts.Add(outpost1);
            listOfOutposts[0].Client = client;

            listOfOutposts.Add(outpost2);
            listOfOutposts[1].Client = client;

            return listOfOutposts.AsQueryable();
        }

        public IQueryable<OutpostHistoricalStockLevel> GetHistoricalList()
        {
            var listOfHistoricals = new List<OutpostHistoricalStockLevel>();
            var client = this.client;

            listOfHistoricals.Add(new OutpostHistoricalStockLevel());
            listOfHistoricals[0].OutpostId = outpostId1;
            listOfHistoricals[0].ProductGroupId = productGroupId;
            listOfHistoricals[0].ProductId = Guid.NewGuid();
            listOfHistoricals[0].UpdateDate = DateTime.UtcNow;

            listOfHistoricals.Add(new OutpostHistoricalStockLevel());
            listOfHistoricals[1].OutpostId = outpostId1;
            listOfHistoricals[1].ProductGroupId = productGroupId;
            listOfHistoricals[1].ProductId = Guid.NewGuid();
            listOfHistoricals[1].UpdateDate = DateTime.UtcNow;

            listOfHistoricals.Add(new OutpostHistoricalStockLevel());
            listOfHistoricals[2].OutpostId = outpostId2;
            listOfHistoricals[2].ProductGroupId = productGroupId;
            listOfHistoricals[2].ProductId = Guid.NewGuid();
            listOfHistoricals[2].UpdateDate = DateTime.UtcNow;


            return listOfHistoricals.AsQueryable();
        }
    }
}
