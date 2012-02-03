using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.OutpostManagement.Controllers;
using Rhino.Mocks;
using Persistence.Queries.Districts;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.District;
using Core.Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.DistrictControllerTests
{
    class ObjectMother
    {
        const string DEFAULT_VIEW_NAME = "";
        const string DISTRICT_NAME = "Cluj";
        const string NEW_DISTRICT_NAME = "Timis";
        const string USER_NAME = "admin";


        DistrictController controller;
        ISaveOrUpdateCommand<District> saveCommand;
        IQueryService<Country> queryCountry;
        IDeleteCommand<District> deleteCommand;
        IQueryService<Region> queryRegion;
        IQueryService<Outpost> queryOutpost;
        IQueryService<Client> queryClient;
        IQueryService<District> queryService;
        IQueryDistrict queryDistrict;
        IQueryService<User> queryUsers;

        public District district;
        public Country country;
        public Region region;
        Client client;
        Outpost outpost;
        User user;

        Guid districtId;
        Guid countryId;
        Guid regionId;
        Guid outpostId;
        Guid clientId;
        Guid userId;

       
        public void Init()
        {
            SetUpServices();
            SetUpController();
            StubUserAndItsClient();
            StubCountry();
            StubRegion();
            StubDistrict();
            StubOutpost();
        }

        internal void StubOutpost()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(districtId);
            outpost.Name = "Cluj";
            outpost.District = district;
        }

        internal void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Cluj";
            region.Country = country;
            region.Client = new Client();

        }
        internal void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "Cluj";

        }
        internal void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
            district.Client = client;
        }

        internal void SetUpController()
        {
            controller = new DistrictController();

            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.QueryRegion = queryRegion;
            controller.QueryOutpost = queryOutpost;
            controller.QueryClients = queryClient;
            controller.QueryDistrict = queryDistrict;
            controller.QueryService = queryService;
            controller.QueryCountry = queryCountry;

        }

        internal void SetUpServices()
        {
            queryDistrict = MockRepository.GenerateMock<IQueryDistrict>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<District>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<District>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryService = MockRepository.GenerateMock<IQueryService<District>>();
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
        }
        internal void StubUserAndItsClient()
        {

            queryClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            this.client = MockRepository.GeneratePartialMock<Client>();
            clientId = Guid.NewGuid();

            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Minoxsys";

            this.user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USER_NAME;
            user.Password = "asdf";

            queryClient.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            controller.LoadClient = queryClient;
            controller.QueryUsers = queryUsers;

        }
        internal IQueryable<DistrictModel> PageOfDistrictData(DistrictIndexModel indexModel)
        {
            List<DistrictModel> districtPageList = new List<DistrictModel>();

            for (int i = indexModel.Start.Value; i < indexModel.Limit.Value; i++)
            {
                districtPageList.Add(new DistrictModel
                {
                    Name = String.Format("DistrictAtIndex{0}", i),
                    ClientId = clientId
                });
            }
            return districtPageList.AsQueryable();
        }

        public void ExpectQueryDistrictSpecificToRegionIdAndClientIdFromModel(DistrictIndexModel model)
        {
            queryService.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());
        }
    }
}
