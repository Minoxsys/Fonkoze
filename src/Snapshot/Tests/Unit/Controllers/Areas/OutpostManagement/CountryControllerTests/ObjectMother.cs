using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using Web.Areas.OutpostManagement.Controllers;
using Core.Persistence;
using Core.Domain;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.OutpostManagement.Models.Country;
using Core.Security;
using Persistence.Security;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    class ObjectMother
    {
        internal CountryController controller;
        internal ISaveOrUpdateCommand<Country> saveCommand;
        internal IDeleteCommand<Country> deleteCommand;
        internal IQueryService<Country> queryCountry;
        internal IQueryService<Region> queryRegion;
        internal IQueryService<Client> loadClient;
        public  IQueryService<User> queryUsers;
        internal IQueryService<WorldCountryRecord> queryWorldCountryRecords;
        public IPermissionsService PermissionService;
        public IQueryService<Permission> queryPermission;


        internal Country fakeCountry;
        internal Region region;
        internal Guid regionId;
        internal Guid entityId;

        internal Client client;
        internal User user;

        internal Guid ClientId = Guid.NewGuid();

        internal const string DEFAULT_VIEW_NAME = "";
        internal const string COUNTRY_NAME = "Romania";
        internal const string NEW_COUNTRY_NAME = "France";
        internal const string REGION_NAME = "Cluj";
        internal const string NEW_REGION_NAME = "Timis";
        internal const string COORDINATES = "14 44";


        internal const string FAKE_USERNAME = "username";

        internal void Init()
        {
            SetUpServices();
            SetUpController();
            StubUserAndItsClient();
            StubEntity();
            StubRegion();
        }


        internal void SetUpServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Country>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Country>>();

            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryWorldCountryRecords = MockRepository.GenerateMock<IQueryService<WorldCountryRecord>>();
            queryPermission = MockRepository.GenerateMock<IQueryService<Permission>>();
            queryRegion.Stub(m => m.Query()).Return(new Region[] { }.AsQueryable());

        }

        internal void SetUpController()
        {
            controller = new CountryController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);
            PermissionService = new FunctionRightsService(queryPermission, queryUsers);
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.PermissionService = PermissionService;
            controller.QueryCountry = queryCountry;
            controller.QueryRegion = queryRegion;
            controller.QueryWorldCountryRecords = queryWorldCountryRecords;
        }

        internal void StubEntity()
        {
            entityId = Guid.NewGuid();
            fakeCountry = MockRepository.GeneratePartialMock<Country>();
            fakeCountry.Stub(b => b.Id).Return(entityId);
            fakeCountry.Name = COUNTRY_NAME;

            queryCountry.Stub(c => c.Load(entityId)).Return(fakeCountry);
        }

        internal void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Country = fakeCountry;
        }

        internal void StubUserAndItsClient()
        {

            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            this.client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(ClientId);
            client.Name = "Minoxsys";

            this.user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c=>c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = FAKE_USERNAME;
            user.Password = "asdf";

            loadClient.Stub(c => c.Load(ClientId)).Return(client);
            queryUsers.Stub(c=>c.Query()).Return( new []{user}.AsQueryable());

            controller.LoadClient = loadClient;
            controller.QueryUsers = queryUsers;

        }


        internal IQueryable<Country> PageOfCountryData(CountryIndexModel indexModel)
        {
            List<Country> countryPageList = new List<Country>();
            
            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                countryPageList.Add(new Country
                {
                    Name = String.Format("CountryAtIndex{0}", i),
                    ISOCode = String.Format("C{0}", i),
                    PhonePrefix = String.Format("{0:00000}", i),
                    Client = client,
                    Regions = new Region[] { }.ToList()
                });
            }
            return countryPageList.AsQueryable();
        }

        internal IQueryable<WorldCountryRecord> WorldCountryRecords()
        {
            var listOfCountryRecords = new List<WorldCountryRecord>();
            listOfCountryRecords.Add(new WorldCountryRecord());
            listOfCountryRecords[0].Name = "Romania";
            listOfCountryRecords[0].ISOCode = "RO";
            listOfCountryRecords[0].PhonePrefix = "0040";
            return listOfCountryRecords.AsQueryable();
        }

        internal IQueryable<Country> CurrentUserCountries()
        {
            var listOfCountryRecords = new List<Country>();

            var client = this.client;


            listOfCountryRecords.Add(new Country());
            listOfCountryRecords[0].Name = "Romania";
            listOfCountryRecords[0].ISOCode = "RO";
            listOfCountryRecords[0].PhonePrefix = "0040";

            listOfCountryRecords[0].Client = client;
            return listOfCountryRecords.AsQueryable();
        }

        internal void QueryCountriesToReturnsEmptyResult()
        {
            this.queryCountry.Expect(call=>call.Query()).Return(new Country[]{}.AsQueryable());

        }


        internal void QueryWorldCountryRecordsReturnsEmptyResult()
        {
            this.queryWorldCountryRecords.Expect(call => call.Query()).Return(new WorldCountryRecord[] { }.AsQueryable());
        }


        internal IQueryable<Country> ExpectQueryCountryToReturnPageOfCountryDataBasedOn(CountryIndexModel indexModel)
        {
            var pageOfData = this.PageOfCountryData(indexModel);

            this.queryCountry.Expect(call => call.Query()).Return(pageOfData);
            return pageOfData;
        }

        internal IQueryable<Region> RegionForCountry()
        {
            return new Region[]{region}.AsQueryable();
        }

        internal void SetupQueryRegionToReturnARegionForThisCountry()
        {
            this.queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();

            this.queryRegion.Expect(call => call.Query()).Return(RegionForCountry());
            this.controller.QueryRegion = queryRegion;
        }
    }
}
