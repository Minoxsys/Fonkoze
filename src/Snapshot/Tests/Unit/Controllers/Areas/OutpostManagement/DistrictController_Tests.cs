using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Persistence.Queries.Districts;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;

namespace Tests.Unit.Controllers.Areas.OutpostManagement
{
    [TestFixture]
    public class DistrictController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string DISTRICT_NAME = "Cluj";
        const string NEW_DISTRICT_NAME = "Timis";


        DistrictController controller;
        ISaveOrUpdateCommand<District> saveCommand;
        IQueryService<Country> queryCountry;
        IDeleteCommand<District> deleteCommand;
        IQueryService<Region> queryRegion;
        IQueryService<Outpost> queryOutpost;
        IQueryService<Client> queryClient;
        IQueryService<District> queryService;
        IQueryDistrict queryDistrict;

        District district;
        Country country;
        Region region;
        Outpost outpost;
        Guid districtId;
        Guid countryId;
        Guid regionId;
        Guid outpostId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubCountry();
            StubRegion();
            StubDistrict();
            StubOutpost();
        }

        private void StubOutpost()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(districtId);
            outpost.Name = "Cluj";
            outpost.District = district;
        }

        private void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Cluj";
            region.Country = country;
            region.Client = new Client();

        }
        private void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "Cluj";

        }
        private void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
        }

        private void SetUpController()
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

        private void SetUpServices()
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

    }
}
