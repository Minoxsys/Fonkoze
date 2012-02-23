using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.CampaignManagement.Controllers;
using Core.Persistence;
using Domain;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.CampaignManagement
{
    public class ObjectMother
    {
        public CampaignController controller;

        public IQueryService<Campaign> queryCampaign;
        public IQueryService<Outpost> queryOutposts;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;
        public IQueryService<Client> loadClient;
        public IQueryService<User> queryUsers;

        public ISaveOrUpdateCommand<Campaign> saveCommand;

        public Campaign campaign;
        public Country country;
        public Region region;
        public District district;
        public Outpost outpost;
        public Client client;
        public User user;

        public Guid campaignId;
        public Guid countryId;
        public Guid regionId;
        public Guid districtId;
        public Guid outpostId;
        public Guid clientId;
        public Guid userId;

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
            queryCampaign = MockRepository.GenerateMock<IQueryService<Campaign>>();
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Campaign>>();
            
        }
        private void Setup_Controller()
        {
            controller = new CampaignController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryCampaign = queryCampaign;
            controller.QueryRegions = queryRegion;
            controller.QueryDistricts = queryDistrict;
            controller.QueryOutposts = queryOutposts;

            controller.SaveOrUpdateCommand = saveCommand;
            
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
            region.Coordinates = "2 3";
            region.Country = country;
            region.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = "Cluj";
            district.Region = region;
            district.Client = client;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "Spitalul Judetean";
            outpost.Country = country;
            outpost.Region = region;
            outpost.District = district;
            outpost.Client = client;

            campaignId = Guid.NewGuid();
            campaign = MockRepository.GeneratePartialMock<Campaign>();
            campaign.Stub(c => c.Id).Return(campaignId);
            campaign.Name = "Campania 1";
            campaign.Opened = true;
            campaign.StartDate = DateTime.UtcNow;
            campaign.EndDate = DateTime.UtcNow.AddDays(2);
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

            controller.LoadClient = loadClient;
            controller.QueryUsers = queryUsers;

        }
    }
}
