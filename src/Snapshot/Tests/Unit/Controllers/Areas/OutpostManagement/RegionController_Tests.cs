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
        District district;
        Guid entityId;
        Guid districtId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
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
        }
        private void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = "Cluj";
            district.Region = entity;
 
        }
        [Test]
        public void Should_Return_Data_From_QueryService_in_Overview()
        {
            // Arrange		
            var stubData = new Region[] { entity };

            queryRegion.Expect(call => call.GetAll()).Return(stubData.AsQueryable());

            // Act
            var viewResult = (ViewResult)controller.Overview(null);

            // Assert
            queryService.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);
            var viewModel = (RegionOverviewModel)viewResult.Model;
            Assert.AreEqual(stubData[0].Name, viewModel.Regions[0].Name);
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

            //act
            var viewResult = (ViewResult)controller.Create(new RegionInputModel());

            //assert
            Assert.AreEqual("Create", viewResult.ViewName);
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryService.Expect(it => it.Load(entity.Id)).Return(entity);
            queryCountry.Expect(call => call.Query()).Return(new Country[] { new Country { Name = "Romania"}}.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(entity.Id);

            //assert
            Assert.IsInstanceOf<RegionOutputModel>(result.Model);
            var viewModel = result.Model as RegionOutputModel;
            Assert.AreEqual(REGION_NAME, viewModel.Name);
            Assert.AreEqual(entity.Id, viewModel.Id);
            Assert.AreEqual(viewModel.Countries[0].Text, "Romania");
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new RegionInputModel();
            model = BuildRegionWithName(NEW_REGION_NAME);
            
            saveCommand.Expect(it => it.Execute(Arg<Region>.Matches(c => c.Name == NEW_REGION_NAME && c.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new RegionInputModel() { Id = entity.Id, Name = NEW_REGION_NAME });

            // Assert
            saveCommand.VerifyAllExpectations();
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
