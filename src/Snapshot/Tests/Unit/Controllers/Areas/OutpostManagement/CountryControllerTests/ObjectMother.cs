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

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    class ObjectMother
    {
        internal CountryController controller;
        internal ISaveOrUpdateCommand<Country> saveCommand;
        internal IDeleteCommand<Country> deleteCommand;
        internal IQueryService<Country> queryCountry;
        internal IQueryService<Region> queryRegion;
        internal IQueryService<Client> queryClient;
        internal IQueryService<User> queryUser;


        internal Country fakeCountry;
        internal Region region;
        internal Guid regionId;
        internal Guid entityId;

        internal const string DEFAULT_VIEW_NAME = "";
        internal const string COUNTRY_NAME = "Romania";
        internal const string NEW_COUNTRY_NAME = "France";
        internal const string REGION_NAME = "Cluj";
        internal const string NEW_REGION_NAME = "Timis";
        internal const string COORDINATES = "14 44";

        internal void Init()
        {
            SetUpServices();
            SetUpController();
            StubRegion();
            StubEntity();
        }


        internal void SetUpServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Country>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Country>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryUser = MockRepository.GenerateMock<IQueryService<User>>();

        }

        internal void SetUpController()
        {
            controller = new CountryController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("username"), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryCountry = queryCountry;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.QueryClients = queryClient;
            controller.QueryRegion = queryRegion;
            controller.QueryUsers = queryUser;
        }

        internal void StubEntity()
        {
            entityId = Guid.NewGuid();
            fakeCountry = MockRepository.GeneratePartialMock<Country>();
            fakeCountry.Stub(b => b.Id).Return(entityId);
            fakeCountry.Name = COUNTRY_NAME;
        }

        internal void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Country = fakeCountry;
        }
    }
}
