using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.OutpostManagement.Controllers;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.OutpostManagement.Models.Region;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    public class ObjectMother
    {
        public RegionController controller;

        public IQueryService<Country> queryCountry;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;
        public IQueryService<Client> loadClient;
        public IQueryService<User> queryUsers;

        public ISaveOrUpdateCommand<Region> saveCommand;
        public IDeleteCommand<Region> deleteCommand;

        public Country country;
        public Region region;
        public Region region2;
        public District district;
        public Client client;
        public User user;

        public Guid countryId;
        public Guid regionId;
        public Guid regionId2;
        public Guid districtId;
        public Guid clientId;
        public Guid userId;

        private const string COUNTRY_NAME = "Romania";
        private const string REGION_NAME = "Transilvania";
        private const string REGION_NAME_NEW = "Ardeal";
        private const string DISTRICT_NAME = "Cluj";
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
            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Region>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Region>>();
        }
        private void Setup_Controller()
        {
            controller = new RegionController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryCountry = queryCountry;
            controller.QueryService = queryRegion;
            controller.QueryDistrict = queryDistrict;

            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
        }

        private void SetUp_StubData()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = COUNTRY_NAME;
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
            region.Client = client;

            regionId2 = Guid.NewGuid();
            region2 = MockRepository.GeneratePartialMock<Region>();
            region2.Stub(c => c.Id).Return(regionId2);
            region2.Name = REGION_NAME;
            region2.Country = country;
            region2.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
            district.Client = client;
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

        public IQueryable<Region> PageOfRegionData(RegionIndexModel indexModel)
        {
            List<Region> regionPageList = new List<Region>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                regionPageList.Add(new Region
                {
                    Name = "RegionName"+i,
                    Client = client,
                    Country = country
                });
            }
            return regionPageList.AsQueryable();
        }

        public IQueryable<Country> CurrentUserCountries()
        {
            var listOfCountryRecords = new List<Country>();

            var client = this.client;

            listOfCountryRecords.Add(new Country());
            listOfCountryRecords[0].Name = COUNTRY_NAME;
            listOfCountryRecords[0].ISOCode = "RO";
            listOfCountryRecords[0].PhonePrefix = "0040";

            listOfCountryRecords[0].Client = client;
            return listOfCountryRecords.AsQueryable();
        }
    }
}
