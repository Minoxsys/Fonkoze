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
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    public class ObjectMother
    {
        public const string CAMPAIGN_NAME = "Campania 1";

        public CampaignController controller;

        public IQueryService<Campaign> queryCampaign;
        public IQueryService<Outpost> queryOutposts;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;
        public IQueryService<Country> queryCountries;
        public IQueryService<Client> loadClient;
        public IQueryService<User> queryUsers;

        public ISaveOrUpdateCommand<Campaign> saveCommand;

        public Campaign campaign;
        public Country country1;
        public Country country2;
        public Region region;
        public District district;
        public Outpost outpost;
        public Client client;
        public User user;

        public Guid campaignId;
        public Guid countryId1;
        public Guid countryId2;
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
            StubUserAndItsClient();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryCampaign = MockRepository.GenerateMock<IQueryService<Campaign>>();
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            loadClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();
            queryCountries = MockRepository.GenerateMock<IQueryService<Country>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Campaign>>();
            
        }
        private void Setup_Controller()
        {
            controller = new CampaignController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryCampaign = queryCampaign;
            controller.QueryCountries = queryCountries;
            controller.QueryRegions = queryRegion;
            controller.QueryDistricts = queryDistrict;
            controller.QueryOutposts = queryOutposts;

            controller.SaveOrUpdateCommand = saveCommand;
            
        }

        private void SetUp_StubData()
        {
            countryId1 = Guid.NewGuid();
            country1 = MockRepository.GeneratePartialMock<Country>();
            country1.Stub(c => c.Id).Return(countryId1);
            country1.Name = "Romania";
            country1.Client = client;

            countryId2 = Guid.NewGuid();
            country2 = MockRepository.GeneratePartialMock<Country>();
            country2.Stub(c => c.Id).Return(countryId2);
            country2.Name = "Bulgaria";
            country2.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Coordinates = "2 3";
            region.Country = country1;
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
            outpost.Country = country1;
            outpost.Region = region;
            outpost.District = district;
            outpost.Client = client;

            campaignId = Guid.NewGuid();
            campaign = MockRepository.GeneratePartialMock<Campaign>();
            campaign.Stub(c => c.Id).Return(campaignId);
            campaign.Name = CAMPAIGN_NAME;
            campaign.Opened = false;
            campaign.StartDate = DateTime.UtcNow;
            campaign.EndDate = DateTime.UtcNow.AddDays(2);
            campaign.CreationDate = DateTime.UtcNow;
            campaign.Options = StrToByteArray((ConvertToJSON(GetOptionsModel(countryId1.ToString(), regionId.ToString(), districtId.ToString(), outpostId.ToString()))));
            campaign.Client = client;
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

        public IQueryable<Campaign> PageOfCampaignData(IndexTableInputModel model)
        {
            List<Campaign> campaignPageList = new List<Campaign>();

            for (int i = model.start.Value; i < model.limit.Value; i++)
            {
                campaignPageList.Add(new Campaign
                {
                    Name = "Campaign" + i,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    CreationDate = campaign.CreationDate,
                    Options = campaign.Options,
                    Client = client
                });
            }
            return campaignPageList.AsQueryable();
        }

        private string ConvertToJSON(OptionsModel model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(model);
        }
        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private OptionsModel GetOptionsModel(string countries, string regions, string districts, string outposts)
        {
            OptionsModel model = new OptionsModel();
            model.Countries = countries;
            model.Regions = regions;
            model.Districts = districts;
            model.Outposts = outposts;

            return model;
        }


        public IQueryable<Country> CurrentUserCountries()
        {
            var listOfCountryRecords = new List<Country>();
            var client = this.client;

            listOfCountryRecords.Add(country1);
            listOfCountryRecords[0].Client = client;
           
            listOfCountryRecords.Add(country2);
            listOfCountryRecords[1].Client = client;
            return listOfCountryRecords.AsQueryable();
        }

        public IQueryable<Region> CurrentUserRegions()
        {
            var listOfRegionRecords = new List<Region>();
            var client = this.client;

            listOfRegionRecords.Add(new Region());
            listOfRegionRecords[0].Name = "Region1";
            listOfRegionRecords[0].Coordinates = "12 25";
            listOfRegionRecords[0].Country = country1;
            listOfRegionRecords[0].Client = client;

            listOfRegionRecords.Add(region);
            listOfRegionRecords[1].Client = client;

            return listOfRegionRecords.AsQueryable();
        }

        public IQueryable<District> CurrentUserDistricts()
        {
            var listOfDistrictRecords = new List<District>();
            var client = this.client;

            listOfDistrictRecords.Add(district);
            listOfDistrictRecords[0].Client = client;

            listOfDistrictRecords.Add(new District());
            listOfDistrictRecords[1].Name = "District2";
            listOfDistrictRecords[1].Region = region;
            listOfDistrictRecords[1].Client = client;

            return listOfDistrictRecords.AsQueryable();
        }

        public IQueryable<Outpost> CurrentUserOutposts()
        {
            var listOfOutpostRecords = new List<Outpost>();
            var client = this.client;

            listOfOutpostRecords.Add(new Outpost());
            listOfOutpostRecords[0].Name = "Outpost1";
            listOfOutpostRecords[0].Country = country1;
            listOfOutpostRecords[0].Region = region;
            listOfOutpostRecords[0].District = district;
            listOfOutpostRecords[0].Client = client;

            listOfOutpostRecords.Add(outpost);
            listOfOutpostRecords[1].Client = client;
            return listOfOutpostRecords.AsQueryable();
        }


    }
}
