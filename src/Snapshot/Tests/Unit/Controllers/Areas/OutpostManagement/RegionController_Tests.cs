using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Controllers;
using Domain;
using Core.Persistence;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Region;
using Core.Domain;
using Persistence.Queries.Regions;



namespace Tests.Unit.Controllers.Areas.OutpostManagement
{
    [TestFixture]
    public class RegionController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string REGION_NAME = "Cluj";
        const string NEW_REGION_NAME = "Timis";
        const string COORDINATES = "14 44";
        

        RegionController controller;
        IQueryService<Region> queryService;
        ISaveOrUpdateCommand<Region> saveCommand;
        IDeleteCommand<Region> deleteCommand;
        IQueryService<Country> queryCountry;
        IQueryService<District> queryDistrict;
        IQueryService<Client> queryClient;
        IQueryRegion queryRegion; 

        Region entity;
        Country country;
        District district;
        Guid countryId;
        Guid entityId;
        Guid districtId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubCountry();
            StubEntity();
            StubDistrict();
        }

        private void SetUpServices()
        {
            queryService = MockRepository.GenerateMock<IQueryService<Region>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Region>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Region>>();
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryRegion = MockRepository.GenerateMock<IQueryRegion>();

        }

        private void SetUpController()
        {
            controller = new RegionController();

            controller.QueryService = queryService;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.QueryCountry = queryCountry;
            controller.QueryDistrict = queryDistrict;
            controller.QueryClients = queryClient;
            controller.QueryRegion = queryRegion;
        }
        private void StubEntity()
        {
            entityId = Guid.NewGuid();
            entity = MockRepository.GeneratePartialMock<Region>();
            entity.Stub(b => b.Id).Return(entityId);
            entity.Name = REGION_NAME;
            entity.Coordinates = COORDINATES;
            entity.Country = country;
        }
        private void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = "Cluj";
            district.Region = entity;
 
        }
        private void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "Cluj";
            
        }
        [Test]
        public void Should_Return_DataSpecificToCountryId_From_QueryService_in_Overview()
        {
            // Arrange		
            
            queryService.Expect(call => call.Query()).Repeat.Once().Return(new Region[] { entity }.AsQueryable());
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());

            // Act
            var viewResult = (ViewResult)controller.Overview(country.Id);

            // Assert
            queryService.VerifyAllExpectations();
            queryDistrict.VerifyAllExpectations();
            queryCountry.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);
            var viewModel = (RegionOverviewModel)viewResult.Model;
            Assert.AreEqual(entity.Name, viewModel.Regions[0].Name);
            Assert.AreEqual(viewModel.Countries[0].Value, entity.Country.Id.ToString());
            Assert.AreEqual(viewModel.Regions[0].DistrictNo, 1);
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        }

        [Test]
        public void Should_Return_AllExistentCountries_AndNoRegions_WhenCountryIdIsNull_FromQueryService_in_Overview()
        { 
            //arrange
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
          

            // Act
            var viewResult = (ViewResult)controller.Overview(Guid.Empty);


            // Assert
           queryCountry.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);
            var viewModel = (RegionOverviewModel)viewResult.Model;
            Assert.IsNotNull(viewModel.Countries); 
            Assert.IsEmpty(viewModel.Regions);
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        }
        [Test]
        public void Should_Display_Empty_Model_When_GET_Create()
        {
            //assert
           
            //act
            var result = controller.Create() as ViewResult;

            //assert
            Assert.IsNull(result.Model);
            
        }

        [Test]
        public void Should_Save_Region_When_Save_Succedes()
        {
            //arrange
            var model = new RegionInputModel();
            model = BuildRegionWithName(REGION_NAME);
            saveCommand.Expect(it => it.Execute(Arg<Region>.Matches(c => c.Name == REGION_NAME)));
            queryClient.Expect(it => it.Load(Guid.Empty)).Return(new Client { Name = "client" });

            //act
            var result = (RedirectToRouteResult)controller.Create(new RegionInputModel() { Name = REGION_NAME });

            //assert
            saveCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", result.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Create_When_POST_Create_Fails_BecauseOf_ModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");

            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());

            var regionInputModel = SetRegionInputModelData_ToPassToCreateMethod();

            //act
            var viewResult = (ViewResult)controller.Create(regionInputModel);

            //assert
            Assert.AreEqual("Create", viewResult.ViewName);
            Assert.IsInstanceOf<RegionOutputModel>(viewResult.Model);

            var model = (RegionOutputModel)viewResult.Model;
            Assert.AreEqual(model.CountryId, country.Id);
            Assert.AreEqual(model.Countries[0].Value, country.Id.ToString());
            Assert.AreEqual(model.Id, entity.Id);
            

        }

        private RegionInputModel SetRegionInputModelData_ToPassToCreateMethod()
        {
            var regionInputModel = new RegionInputModel();
            regionInputModel.CountryId = country.Id;
            regionInputModel.Id = entity.Id;
            return regionInputModel;
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryService.Expect(it => it.Load(entity.Id)).Return(entity);
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country}.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(entity.Id);

            //assert
            Assert.IsInstanceOf<RegionOutputModel>(result.Model);
            var viewModel = result.Model as RegionOutputModel;
            Assert.AreEqual(REGION_NAME, viewModel.Name);
            Assert.AreEqual(entity.Id, viewModel.Id);
            Assert.AreEqual(viewModel.Countries[0].Text, country.Name);
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new RegionInputModel();
            model = BuildRegionWithName(NEW_REGION_NAME);

            queryCountry.Expect(call => call.Load(country.Id)).Return(country);
            saveCommand.Expect(it => it.Execute(Arg<Region>.Matches(c => c.Name == NEW_REGION_NAME && c.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new RegionInputModel() { Id = entity.Id, Name = NEW_REGION_NAME, CountryId = country.Id });

            // Assert
            saveCommand.VerifyAllExpectations();
            queryCountry.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            controller.ModelState.AddModelError("Name", "Field required");
            queryCountry.Expect(call => call.Query()).Return(new Country[] { new Country { Name = "Romania" } }.AsQueryable());

            var viewResult = (ViewResult)controller.Edit(new RegionInputModel());

            queryCountry.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }


        [Test]
        public void Should_goto_Overview_when_Delete_AndThereAre_NoDistrictAsociated()
        {
            //arrange
            queryDistrict.Expect(call => call.Query()).Repeat.Once().Return(new District[]{}.AsQueryable());
            queryService.Expect(call => call.Load(entity.Id)).Return(entity);
            deleteCommand.Expect(call => call.Execute(Arg<Region>.Matches(b => b.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);

            // Assert
            deleteCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_GoTo_Overview_WhenDeleteARegion_AndDisplayTempDataError_If_ThereAreDistrictsAsociated()
        {
            //arrange
            queryDistrict.Expect(call => call.Query()).Repeat.Once().Return(new District[] { district }.AsQueryable());
            queryService.Expect(call => call.Load(entity.Id)).Return(entity);

            //act
            var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);

            //assert
            queryService.VerifyAllExpectations();
            queryDistrict.VerifyAllExpectations();

            Assert.That(controller.TempData.ContainsKey("error"));
            Assert.That(controller.TempData.ContainsValue("The region " +entity.Name + " has districts associated, so it can not be deleted"));
            
        }
        private RegionInputModel BuildRegionWithName( string name)
        {
        
            var model = new RegionInputModel();
            model.Name = name;
            model.Id = entityId;

            return model;
        }
               
    }
}
