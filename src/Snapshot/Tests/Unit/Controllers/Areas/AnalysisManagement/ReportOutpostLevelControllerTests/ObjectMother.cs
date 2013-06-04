using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Persistence;
using Domain;
using MvcContrib.TestHelper.Fakes;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.ReportOutpostLevelControllerTests
{
    public class ObjectMother
    {
        public ReportOutpostLevelController controller;

        public IQueryService<OutpostStockLevel> queryOSL;

        public IQueryService<User> queryUsers;
        public IQueryService<Client> queryClients;
        public Client client;
        public User user;
        public Guid clientId;
        public Guid userId;

        public Country country;
        public Region region;
        public District district;
        public Outpost outpost;
                
        public Guid countryId;
        public Guid regionId;
        public Guid districtId;
        public Guid outpostId;

        public List<OutpostStockLevel> oslList;
        public List<OutpostStockLevel> oslListUnderThreshold;

        public void Init()
        {
            SetUpQueryServices();
            SetUpController();
            StubUserAndItsClient();
            StubCountryRegionDistrictOutpost();
            CreateReturnOSL();
        }

        internal void SetUpQueryServices()
        {
            queryClients = MockRepository.GenerateMock<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryOSL = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();

        }

        internal void SetUpController()
        {
            controller = new ReportOutpostLevelController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("username"), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClient = queryClients;
            controller.QueryUser = queryUsers;
            controller.QueryOutpostStockLevel = queryOSL;
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
            user.UserName = "username";
            user.Password = "asdf";

            queryClients.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());
        }
        internal void StubCountryRegionDistrictOutpost()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "CountryName";

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "RegionName";
            region.Country = country;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = "DistrictName";
            district.Region = region;

            outpost = new Outpost();
           // outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            //outpost.Stub(b => b.Id).Return(outpostId);
            outpost.Name = "OutpostName";
            outpost.Country = country;
            outpost.Region = region;
            outpost.District = district;

        }


        internal void CreateReturnOSL()
        {
            oslList = new List<OutpostStockLevel>();
            oslListUnderThreshold = new List<OutpostStockLevel>();
            Product product = MockRepository.GeneratePartialMock<Product>();
            product.Name = "ProductAboveThreshold";
            product.LowerLimit = 10;

            Product productUnderThreshold = MockRepository.GeneratePartialMock<Product>();
            productUnderThreshold.Name = "ProdUnderThreshold";
            productUnderThreshold.LowerLimit = 6;

            OutpostStockLevel osl1 = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            osl1.Client = client;
            osl1.Outpost = outpost;
            osl1.Product = product;
            osl1.Outpost = outpost;
            osl1.StockLevel = 20;

            OutpostStockLevel osl2 = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            osl2.Client = client;
            osl2.Outpost = outpost;
            osl2.Product = productUnderThreshold;
            osl2.Outpost = outpost;
            osl2.StockLevel = 2;

            oslList.Add(osl1);
            oslList.Add(osl2);

            oslListUnderThreshold.Add(osl2);           
           
            
        }
    }
}
