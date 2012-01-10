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
using Web.Areas.OutpostManagement.Models.Country;
using Core.Domain;
using Persistence.Queries.Countries;



namespace Tests.Unit.Controllers.Areas.OutpostManagement
{
    [TestFixture]
    public class CountryController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string COUNTRY_NAME = "Romania";
        const string NEW_COUNTRY_NAME = "France";
        const string REGION_NAME = "Cluj";
        const string NEW_REGION_NAME = "Timis";
        const string COORDINATES = "14 44";


        CountryController controller;
        ISaveOrUpdateCommand<Country> saveCommand;
        IDeleteCommand<Country> deleteCommand;
        IQueryService<Country> queryCountry;
        IQueryService<Region> queryRegion;
        IQueryService<Client> queryClient;
        

        Country entity;
        Region region;
        Guid regionId;
        Guid entityId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubRegion();
            StubEntity();
        }

        private void SetUpServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Country>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Country>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();

        }

        private void SetUpController()
        {
            controller = new CountryController();

            controller.QueryCountry = queryCountry;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.QueryClients = queryClient;
            controller.QueryRegion = queryRegion;
        }

        private void StubEntity()
        {
            entityId = Guid.NewGuid();
            entity = MockRepository.GeneratePartialMock<Country>();
            entity.Stub(b => b.Id).Return(entityId);
            entity.Name = COUNTRY_NAME;
        }

        private void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Country = entity;
        }

         [Test]
        public void Should_Return_DataSpecificToCountryId_From_QueryService_in_Overview()
        {
            // Arrange		
            
            queryCountry.Expect(call => call.Query()).Repeat.Once().Return(new Country[] { entity }.AsQueryable());
            //queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            //queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());

            // Act
            var viewResult = (ViewResult)controller.Overview(1);

            // Assert
            queryCountry.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);
            var viewModel = (CountryOverviewModel)viewResult.Model;
            Assert.AreEqual(entity.Name, viewModel.Countries[0].Name);
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        }


        [Test]
        public void Should_Display_Empty_Model_When_GET_Create()
        {
            //assert
           
            //act
            var result = controller.Create(1) as ViewResult;

            //assert
            Assert.IsNull(result.Model);
            
        }

        [Test]
        public void Should_Save_Country_When_Save_Succedes()
        {
            //arrange
            var model = new CountryInputModel();
            model = BuildCountryWithName(COUNTRY_NAME);
            saveCommand.Expect(it => it.Execute(Arg<Country>.Matches(c => c.Name == COUNTRY_NAME)));
            queryClient.Expect(it => it.Load(Guid.Empty)).Return(new Client { Name = "client" });

            //act
            var result = (RedirectToRouteResult)controller.Create(new CountryInputModel() { Name = COUNTRY_NAME });

            //assert
            saveCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", result.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Create_When_POST_Create_Fails_BecauseOf_ModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");

            var countryInputModel = SetCountryInputModelData_ToPassToCreateMethod();

            //act
            var viewResult = (ViewResult)controller.Create(countryInputModel);

            //assert
            Assert.AreEqual("Create", viewResult.ViewName);
            Assert.IsInstanceOf<CountryOutputModel>(viewResult.Model);

            var model = (CountryOutputModel)viewResult.Model;
            Assert.AreEqual(model.Id, entity.Id);            

        }

        private CountryInputModel SetCountryInputModelData_ToPassToCreateMethod()
        {
            var countryInputModel = new CountryInputModel();
            countryInputModel.Id = entity.Id;
            return countryInputModel;
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryCountry.Expect(it => it.Load(entity.Id)).Return(entity);
            //act
            var result = (ViewResult)controller.Edit(entity.Id);

            //assert
            Assert.IsInstanceOf<CountryOutputModel>(result.Model);
            var viewModel = result.Model as CountryOutputModel;
            Assert.AreEqual(COUNTRY_NAME, viewModel.Name);
            Assert.AreEqual(entity.Id, viewModel.Id);
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new CountryInputModel();
            model = BuildCountryWithName(NEW_COUNTRY_NAME);

            queryCountry.Expect(call => call.Load(entity.Id)).Return(entity);
            saveCommand.Expect(it => it.Execute(Arg<Country>.Matches(c => c.Name == NEW_COUNTRY_NAME && c.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new CountryInputModel() { Id = entity.Id, Name = NEW_COUNTRY_NAME});

            // Assert
            saveCommand.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            controller.ModelState.AddModelError("Name", "Field required");
 
            var viewResult = (ViewResult)controller.Edit(new CountryInputModel());

            queryCountry.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }


        [Test]
        public void Should_goto_Overview_when_Delete_AndThereAre_NoRegionAsociated()
        {
            //arrange
            queryCountry.Expect(call => call.Load(entity.Id)).Return(entity);
            deleteCommand.Expect(call => call.Execute(Arg<Country>.Matches(b => b.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);

            // Assert
            deleteCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_GoTo_Overview_WhenDeleteARegion_AndDisplayTempDataError_If_ThereAreRegionsAsociated()
        {
            //arrange
           queryCountry.Expect(call => call.Load(entity.Id)).Return(entity);
           queryRegion.Expect(call => call.Query()).Repeat.Once().Return(new Region[] { region }.AsQueryable());

            //act
            var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);
 
            //act

            //assert
            queryCountry.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();

            Assert.That(controller.TempData.ContainsKey("error"));
            Assert.That(controller.TempData.ContainsValue("The Country " +entity.Name + " has regions associated, so it can not be deleted"));
            
        }
        private CountryInputModel BuildCountryWithName( string name)
        {
        
            var model = new CountryInputModel();
            model.Name = name;
            model.Id = entityId;

            return model;
        }
               
    }
}
