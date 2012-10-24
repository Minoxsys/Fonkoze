using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.AnalysisManagement.Controllers;
using Core.Persistence;
using Domain;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.LocationReportControllerTests
{
    public class ObjectMother
    {
        public LocationReportController controller;

        public IQueryService<Outpost> queryOutpost;
        public IQueryService<District> queryDistrict;
        public IQueryService<Region> queryRegion;
        public IQueryService<Country> queryCountry;
        public IQueryService<OutpostStockLevel> queryStockLevel;

        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;

        public Client client;
        public Guid clientId;
        public User user;
        public Guid userId;

        public Guid countryId;
        public Country country;
        public Guid regionId;
        public Region region;
        public Guid districtId;
        public District district;
        public Guid outpostId;
        public Outpost outpost;
        public Guid stockLevelId;
        public OutpostStockLevel stockLevel;

        public List<Country> countryList;
        public List<Region> regionList;
        public List<District> districtList;
        public List<Outpost> outpostList;

        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            StubUserAndItsClient();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();

        }
        private void Setup_Controller()
        {
            controller = new LocationReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryCountries = queryCountry;
            controller.QueryRegions = queryRegion;
            controller.QueryDistricts = queryDistrict;
            controller.QueryOutposts = queryOutpost;
            controller.QueryStockLevel = queryStockLevel;
        }

        public void StubUserAndItsClient()
        {
            queryClient = MockRepository.GenerateStub<IQueryService<Client>>();
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

            queryClient.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            controller.QueryClients = queryClient;
            controller.QueryUsers = queryUsers;

        }

        private void SetUp_StubData()
        {
            countryList = new List<Country>();
            regionList = new List<Region>();
            districtList = new List<District>();
            outpostList = new List<Outpost>();

            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = "Country";
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Region";
            region.Country = country;
            region.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = "District";
            district.Region = region;
            district.Client = client;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "District";
            outpost.Country = country;
            outpost.Region = region;
            outpost.District = district;
            outpost.Client = client;

            stockLevelId = Guid.NewGuid();
            stockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            stockLevel.Stub(c => c.Id).Return(stockLevelId);
            stockLevel.Outpost = outpost;
            stockLevel.PrevStockLevel = 10;
            stockLevel.StockLevel = 20;
            stockLevel.Client = client;

            countryList.Add(country);
            regionList.Add(region);
            districtList.Add(district);
            outpostList.Add(outpost);

            for (int i = 0; i < 9; i++)
            {
                Guid newCountryId = Guid.NewGuid();
                Country newCountry = MockRepository.GeneratePartialMock<Country>();
                newCountry.Stub(c => c.Id).Return(newCountryId);
                newCountry.Name = "Country"+i;
                newCountry.Client = client;

                countryList.Add(newCountry);

                Guid newRegionId = Guid.NewGuid();
                Region newRegion = MockRepository.GeneratePartialMock<Region>();
                newRegion.Stub(c => c.Id).Return(newRegionId);
                newRegion.Name = "Region"+i;
                if (i % 2 != 0)
                    newRegion.Country = newCountry;
                else
                    newRegion.Country = country;
                newRegion.Client = client;

                regionList.Add(newRegion);

                Guid newDistrictId = Guid.NewGuid();
                District newDistrict = MockRepository.GeneratePartialMock<District>();
                newDistrict.Stub(c => c.Id).Return(newDistrictId);
                newDistrict.Name = "District" + i;
                if (i % 4 != 0)
                    newDistrict.Region = newRegion;
                else
                    newDistrict.Region = region;
                newDistrict.Client = client;

                districtList.Add(newDistrict);

                Guid newOutpostId = Guid.NewGuid();
                Outpost newOutpost = MockRepository.GeneratePartialMock<Outpost>();
                newOutpost.Stub(c => c.Id).Return(newOutpostId);
                newOutpost.Name = "District";

                newOutpost.Country = newCountry;
                newOutpost.Region = newRegion;
                newOutpost.District = newDistrict;
                newOutpost.Client = client;

                outpostList.Add(newOutpost);
            }
            
        }


    }
}
