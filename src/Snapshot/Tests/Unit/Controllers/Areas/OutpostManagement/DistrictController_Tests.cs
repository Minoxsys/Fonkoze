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

        //[Test]
        //public void Should_Return_Data_From_QueryService_in_Overview()
        //{
        //    // Arrange		
        //    var stubData = new District[] { district };

        //    queryDistrict.Expect(call => call.GetAll()).Return(stubData.AsQueryable());

        //    // Act
        //    var viewResult = (ViewResult)controller.Overview(null,null);

        //    // Assert
        //    queryDistrict.VerifyAllExpectations();

        //    Assert.IsNotNull(viewResult.Model);
        //    var viewModel = (DistrictOverviewModel)viewResult.Model;
        //    Assert.AreEqual(stubData[0].Name, viewModel.Districts[0].Name);
        //    Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        //}

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
            var model = new DistrictInputModel();
            model = BuildDistrictWithName(DISTRICT_NAME);
            saveCommand.Expect(it => it.Execute(Arg<District>.Matches(c => c.Name == DISTRICT_NAME)));
            queryClient.Expect(it => it.Load(Guid.Empty)).Return(new Client { Name = "client" });

            //act
            var result = (RedirectToRouteResult)controller.Create(new DistrictInputModel() { Name = DISTRICT_NAME, Region = new RegionModel { Name = region.Name, Id = region.Id} });

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
            var viewResult = (ViewResult)controller.Create(new DistrictInputModel());

            //assert
            Assert.AreEqual("Create", viewResult.ViewName);
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryService.Expect(it => it.Load(district.Id)).Return(district);
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(district.Id);

            //assert
            Assert.IsInstanceOf<DistrictOutputModel>(result.Model);
            var viewModel = result.Model as DistrictOutputModel;
            Assert.AreEqual(DISTRICT_NAME, viewModel.Name);
            Assert.AreEqual(district.Id, viewModel.Id);
            //Assert.AreEqual(viewModel.Regions[0].Text, "Cluj");
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new DistrictInputModel();
            model = BuildDistrictWithName(NEW_DISTRICT_NAME);

            saveCommand.Expect(it => it.Execute(Arg<District>.Matches(c => c.Name == NEW_DISTRICT_NAME && c.Id == district.Id)));
            queryRegion.Expect(it => it.Load(region.Id)).Return(region);
            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new DistrictInputModel() { Id = district.Id, Name = NEW_DISTRICT_NAME, Region = new RegionModel { Name = region.Name, Id = region.Id } });


            // Assert
            saveCommand.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            controller.ModelState.AddModelError("Name", "Field required");
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());

            var viewResult = (ViewResult)controller.Edit(new DistrictInputModel());
            queryCountry.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }

        [Test]
        public void Should_goto_Overview_when_Delete_AndThereAre_NoOutpostAsociated()
        {
            //arrange
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { }.AsQueryable());
            queryService.Expect(call => call.Load(district.Id)).Return(district);
            deleteCommand.Expect(call => call.Execute(Arg<District>.Matches(b => b.Id == district.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Delete(district.Id);

            // Assert
            deleteCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_GoTo_Overview_WhenDeleteADistrict_AndDisplayTempDataError_If_ThereAreOutpostsAsociated()
        {
            //arrange
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { outpost }.AsQueryable());
            queryService.Expect(call => call.Load(district.Id)).Return(district);

            //act
            var redirectResult = (RedirectToRouteResult)controller.Delete(district.Id);

            //assert
            queryService.VerifyAllExpectations();
            queryDistrict.VerifyAllExpectations();

            Assert.That(controller.TempData.ContainsKey("error"));
            Assert.That(controller.TempData.ContainsValue("The district " + district.Name + " has outposts associated, so it can not be deleted"));

        }
        private DistrictInputModel BuildDistrictWithName(string DISTRICT_NAME)
        {
            var districtInputModel = new DistrictInputModel();

            districtInputModel.Name = DISTRICT_NAME;

            return districtInputModel;
        }
    }
}
