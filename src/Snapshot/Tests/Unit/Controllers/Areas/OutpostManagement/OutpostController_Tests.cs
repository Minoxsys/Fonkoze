using System;
using System.Linq;
using NUnit.Framework;
using Persistence.Queries.Districts;
using Web.Areas.OutpostManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Persistence.Queries.Outposts;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.District;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Areas.OutpostManagement.Models.Country;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement
{
    [TestFixture]
    public class OutpostController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string OUTPOST_NAME = "Cluj";
        const string NEW_OUTPOST_NAME = "Timis";


        OutpostController controller;
        ISaveOrUpdateCommand<Outpost> saveCommand;
        IDeleteCommand<Outpost> deleteCommand;
        IQueryService<Outpost> queryOutpost;

        IQueryService<Country> queryCountry;
        IQueryService<Region> queryRegion;
        IQueryService<District> queryDistrict;
        IQueryService<Product> queryProduct;
        IQueryService<Client> queryClient;

        District district;
        Country country;
        Region region;
        Outpost outpost;
        Product product;
        Guid districtId;
        Guid countryId;
        Guid regionId;
        Guid outpostId;
        Guid productId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubCountry();
            StubRegion();
            StubDistrict();
            StubOutpost();
            //StubProducts();
        }

        private void StubOutpost()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(districtId);
            outpost.Name = "Outpost1";
            outpost.District = district;
        }

        private void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = "Region1";
            region.Country = country;
            region.Client = new Client();

        }
        private void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "Country1";

        }
        private void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = "District1";
            district.Region = region;
        }

        private void SetUpController()
        {
            controller = new OutpostController();

            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.QueryService = queryOutpost;
            controller.QueryClients = queryClient;
            controller.QueryCountry = queryCountry;
            controller.QueryRegion = queryRegion;
            controller.QueryDistrict = queryDistrict;
        }

        private void SetUpServices()
        {
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Outpost>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Outpost>>();
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>(); 
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();

        }

        [Test]
        public void Should_Return_Data_From_OutpostQueryService_WhenCountryIdAndRegionIdAndDistrictIdAreNull_ON_Overview()
        {
            // Arrange		
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
           
            // Act
            var viewResult = (ViewResult)controller.Overview(null, null, null);

            // Assert
           queryCountry.VerifyAllExpectations();
           

            Assert.IsNotNull(viewResult.Model);
            var viewModel = (OutpostOverviewModel)viewResult.Model;

            Assert.AreEqual(region.Country.Id, country.Id);
            Assert.AreEqual(district.Region.Id, region.Id);

            Assert.AreEqual(viewModel.Countries[0].Value, country.Id.ToString());
            Assert.AreEqual(viewModel.Regions[0].Value, region.Id.ToString());
            Assert.AreEqual(viewModel.Districts[0].Value, district.Id.ToString());
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        }

        [Test]
        public void ShouldReturnData_FromCountryServiceSpecificTo_CountryIdNotNull_AndFrom_RegionQueryServiceSpecificToThatCountryId_AndFrom_DistrictsQueryServiceSpecificToRegionIdNotNull_TO_Overview()
        {
            //arange
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Repeat.Once().Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Repeat.Once().Return(new District[] { district }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { outpost }.AsQueryable());

            //act
            var viewResult = (ViewResult)controller.Overview(country.Id, region.Id, district.Id);

            //assert
            Assert.IsNotNull(viewResult.Model);
            var viewModel = (OutpostOverviewModel)viewResult.Model;

            Assert.AreEqual(region.Country.Id, country.Id);
            Assert.AreEqual(district.Region.Id, region.Id);

            Assert.AreEqual(viewModel.Countries[0].Value, country.Id.ToString());
            //Assert.AreEqual(viewModel.Districts[0]., district.Id);
            Assert.AreEqual(viewModel.Regions[0].Value, region.Id.ToString());
            Assert.AreEqual(region.Country.Id, country.Id);
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
            var model = new OutpostInputModel();
            model = BuildOutpostWithName(OUTPOST_NAME);
            saveCommand.Expect(it => it.Execute(Arg<Outpost>.Matches(c => c.Name == OUTPOST_NAME)));
            queryClient.Expect(it => it.Load(Guid.Empty)).Return(new Client { Name = "client" });

            //act
            var result = (RedirectToRouteResult)controller.Create(new OutpostInputModel() { Name = OUTPOST_NAME, Region = new OutpostInputModel.RegionInputModel { Id = region.Id } });

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
            queryRegion.Expect(call => call.Query()).Repeat.Once().Return(new Region[] { region }.AsQueryable());

            var outpostInputModel = SetOutpostInputModelWithData_ToBeTransmitedToCreateMethod();
            //act
            var viewResult = (ViewResult)controller.Create(outpostInputModel);

            //assert
            Assert.AreEqual("Create", viewResult.ViewName);
            Assert.IsInstanceOf<DistrictOutputModel>(viewResult.Model);

            var model = (DistrictOutputModel)viewResult.Model;

            Assert.AreEqual(model.Countries[0].Value, country.Id.ToString());
            Assert.AreEqual(model.Region.CountryId, country.Id);
            Assert.AreEqual(model.Regions[0].Value, region.Id.ToString());
        }

        private OutpostInputModel SetOutpostInputModelWithData_ToBeTransmitedToCreateMethod()
        {
            var outpostInputModel = new OutpostInputModel();
            outpostInputModel.District = new OutpostInputModel.DistrictInputModel();
            outpostInputModel.Region.Id = region.Id;
            outpostInputModel.Region.CountryId = region.Country.Id;
            outpostInputModel.District.Id = district.Id;
            return outpostInputModel;
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(district.Id);

            //assert
            Assert.IsInstanceOf<DistrictOutputModel>(result.Model);
            var viewModel = result.Model as OutpostOutputModel;
            Assert.AreEqual(OUTPOST_NAME, viewModel.Name);
            Assert.AreEqual(outpost.Id, viewModel.Id);

        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new OutpostInputModel();
            model = BuildOutpostWithName(NEW_OUTPOST_NAME);

            saveCommand.Expect(it => it.Execute(Arg<Outpost>.Matches(c => c.Name == NEW_OUTPOST_NAME && c.Id == outpost.Id)));
            queryRegion.Expect(it => it.Load(region.Id)).Return(region);
            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new OutpostInputModel() { Id = outpost.Id, 
                                                                                                  Name = NEW_OUTPOST_NAME, 
                                                                                                  District = new Web.Areas.OutpostManagement.Models.Outpost.OutpostInputModel.DistrictInputModel { Id = district.Id } });


            // Assert
            saveCommand.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());

            var outpostInputModel = SetOutpostInputModelWithData_ToBeTransmitedToCreateMethod();

            //act
            var viewResult = (ViewResult)controller.Edit(outpostInputModel);

            //assert
            queryCountry.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }

        [Test]
        public void Should_goto_Overview_when_Delete_AndThereAre_NoOutpostAsociated()
        {
            //arrange
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { }.AsQueryable());
            queryDistrict.Expect(call => call.Load(district.Id)).Return(district);
            deleteCommand.Expect(call => call.Execute(Arg<Outpost>.Matches(b => b.Id == district.Id)));

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
            queryProduct.Expect(call => call.Query()).Repeat.Once().Return(new Product[] { product }.AsQueryable());
            queryOutpost.Expect(call => call.Load(outpost.Id)).Return(outpost);

            //act
            var redirectResult = (RedirectToRouteResult)controller.Delete(outpost.Id);

            //assert
            queryOutpost.VerifyAllExpectations();
            queryDistrict.VerifyAllExpectations();

            Assert.That(controller.TempData.ContainsKey("error"));
            Assert.That(controller.TempData.ContainsValue("The outpost " + outpost.Name + " has products associated, so it can not be deleted"));

        }

        private OutpostInputModel BuildOutpostWithName(string OUTPOST_NAME)
        {
            var outpostInputModel = new OutpostInputModel();

            outpostInputModel.Name = OUTPOST_NAME;

            return outpostInputModel;
        }
    }
}
