﻿using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.District;

namespace Tests.Unit.Controllers.Areas.OutpostManagement
{
    [TestFixture]
    public class OutpostController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string OUTPOST_NAME = "Outpost1";
        const string NEW_OUTPOST_NAME = "Timis";


        OutpostController controller;
        ISaveOrUpdateCommand<Outpost> saveCommand;
        IDeleteCommand<Outpost> deleteCommand;
        IQueryService<Outpost> queryOutpost;
        IQueryService<Outpost> queryWarehouse;

        IQueryService<Country> queryCountry;
        IQueryService<Region> queryRegion;
        IQueryService<District> queryDistrict;
        IQueryService<Product> queryProduct;
        IQueryService<Client> queryClient;
        IQueryService<Contact> queryContact;

        District district;
        Country country;
        Region region;
        Outpost outpost;
        Outpost warehouses;
        Product product;
        Contact contact;

        Guid districtId;
        Guid countryId;
        Guid regionId;
        Guid outpostId;
        Guid productId;
        Guid contactId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubCountry();
            StubRegion();
            StubDistrict();
            StubOutpost();
            StubProduct();
            StubWarehouse();
            StubContact();
        }


        private void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = "Country1";

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
 
        private void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = "District1";
            district.Region = region;
        }

        private void StubOutpost()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(outpostId);
            outpost.Name = "Outpost1";
            outpost.District = district;
            //outpost.Contact = contact;
        }

        private void StubWarehouse()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(outpostId);
            outpost.Name = "Outpost1";
            outpost.District = district;
        }

        private void StubProduct()
        {
            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(b => b.Id).Return(productId);
            product.Name = "Product1";
            //product.Outpost = outpost;
        }

        private void StubContact()
        {
            contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.Outpost = outpost;
            contact.Stub(b => b.Id).Return(productId);
            contact.ContactType = "Mobile Phone";
            contact.ContactDetail = "1234567";
            contact.IsMainContact = true;
        }

        private void SetUpController()
        {
            controller = new OutpostController
                        {
                            SaveOrUpdateCommand = saveCommand,
                            DeleteCommand = deleteCommand,
                            QueryClients = queryClient,
                            QueryCountry = queryCountry,
                            QueryRegion = queryRegion,
                            QueryDistrict = queryDistrict,
                            QueryService = queryOutpost,
                            QueryWarehouse = queryWarehouse,
                            QueryContact = queryContact
                        };

        }

        private void SetUpServices()
        {
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryWarehouse = MockRepository.GenerateMock<IQueryService<Outpost>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Outpost>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Outpost>>();
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>(); 
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryContact = MockRepository.GenerateMock<IQueryService<Contact>>();

        }

        [Test]
        public void ShouldReturnData_FromOutpostServiceSpecificTo_CountryIdNotNull_AndFrom_RegionQueryServiceSpecificToThatCountryId_AndFrom_DistrictsQueryServiceSpecificToRegionIdNotNull_TO_Overview()
        {
            //arange
            queryOutpost.Expect(it => it.Load(outpost.Id)).Return(outpost);
            queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { outpost }.AsQueryable());
            queryWarehouse.Expect(call => call.Query()).Return(new Outpost[] { warehouses }.AsQueryable());

            //act
            var viewResult = (ViewResult)controller.Overview(country.Id, region.Id, district.Id);

            //assert
            Assert.IsNotNull(viewResult.Model);
            var viewModel = (OutpostOverviewModel)viewResult.Model;

            Assert.AreEqual(region.Country.Id, country.Id);
            Assert.AreEqual(district.Region.Id, region.Id);

            Assert.AreEqual(viewModel.Countries[0].Value, country.Id.ToString());
            Assert.AreEqual(viewModel.Regions[0].Value, region.Id.ToString());
            Assert.AreEqual(viewModel.Districts[0].Value, district.Id.ToString());
            Assert.AreEqual(region.Country.Id, country.Id);
            Assert.AreEqual(region.Id, region.Id);
            Assert.AreEqual(district.Id, district.Id);
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
 
        }
        [Test]
        public void Should_Display_Empty_Model_When_GET_Create()
        {
            //assert

            //act
            //var result = controller.Create() as ViewResult;

            ////assert
            //Assert.IsNull(result.Model);

        }


        private OutpostInputModel SetOutpostInputModelWithData_ToBeTransmitedToCreateMethod()
        {
            var outpostInputModel = new OutpostInputModel()
            {
                Name = OUTPOST_NAME,
                Region = new OutpostInputModel.RegionInputModel { CountryId = region.Country.Id, Id = region.Id },
                District = new OutpostInputModel.DistrictInputModel { Id = district.Id }
            };
            //outpostInputModel.District.Id = district.Id;
            return outpostInputModel;
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryOutpost.Expect(it => it.Load(outpost.Id)).Return(outpost);
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());
            queryContact.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { outpost }.AsQueryable());
            queryWarehouse.Expect(call => call.Query()).Return(new Outpost[] { warehouses }.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(outpost.Id, country.Id, region.Id, district.Id);

            //assert
            Assert.IsInstanceOf<OutpostOutputModel>(result.Model);
            var viewModel = result.Model as OutpostOutputModel;
            Assert.AreEqual(OUTPOST_NAME, viewModel.Name);
            Assert.AreEqual(outpost.Id, viewModel.Id);

        }

 

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");
            queryOutpost.Expect(it => it.Load(outpost.Id)).Return(outpost);
            queryOutpost.Expect(call => call.Query()).Repeat.Once().Return(new Outpost[] { outpost }.AsQueryable());
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryRegion.Expect(call => call.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());
            queryContact.Expect(call => call.Query()).Return(new Contact[] { contact }.AsQueryable());

            var outpostInputModel = SetOutpostInputModelWithData_ToBeTransmitedToCreateMethod();

            //act
            var viewResult = (ViewResult)controller.Edit(outpostInputModel);

            //assert
            queryCountry.VerifyAllExpectations();
            queryRegion.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }

        [Test]
        public void Should_goto_Overview_when_DeleteAnOutpost_AndThereAre_NoProductsAsociated()
        {
            //arrange
            //queryProduct.Expect(call => call.Query()).Repeat.Once().Return(new Product[] { }.AsQueryable());
            queryOutpost.Expect(call => call.Load(outpost.Id)).Return(outpost);
            deleteCommand.Expect(call => call.Execute(Arg<Outpost>.Matches(b => b.Id == outpost.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Delete(outpost.Id);

            // Assert
            deleteCommand.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_GoTo_Overview_WhenDeleteAnOutpost_AndDisplayTempDataError_If_ThereAreProductsAsociated()
        {
            //arrange
            //queryProduct.Expect(call => call.Query()).Repeat.Once().Return(new Product[] { product }.AsQueryable());
            //queryOutpost.Expect(call => call.Load(outpost.Id)).Return(outpost);

            ////act
            //var redirectResult = (RedirectToRouteResult)controller.Delete(outpost.Id);

            ////assert
            //queryOutpost.VerifyAllExpectations();
            //queryDistrict.VerifyAllExpectations();

            //Assert.That(controller.TempData.ContainsKey("error"));
            //Assert.That(controller.TempData.ContainsValue("The outpost " + outpost.Name + " has products associated, so it can not be deleted"));

        }

        private OutpostInputModel BuildOutpostWithName(string OUTPOST_NAME)
        {
            var outpostInputModel = new OutpostInputModel();

            outpostInputModel.Name = OUTPOST_NAME;

            return outpostInputModel;
        }
    }
}
