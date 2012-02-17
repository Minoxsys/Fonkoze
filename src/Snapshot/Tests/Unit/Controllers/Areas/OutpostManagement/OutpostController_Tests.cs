using System;
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
                            LoadClient = queryClient,
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
