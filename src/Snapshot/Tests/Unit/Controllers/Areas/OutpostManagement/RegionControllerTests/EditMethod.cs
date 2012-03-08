using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Region;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Region_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RegionInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a regionId in order to edit the region."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ModalState_IsNOT_Valid()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RegionInputModel() { Id = objectMother.region.Id});

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The region has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyARegionWithTheSameName_IsNOT_Valid()
        {
            //Arrange
            RegionInputModel regionInputModel = new RegionInputModel()
            {
                Id = objectMother.region.Id,
                Name = objectMother.region.Name,
                CountryId = objectMother.region.Country.Id
            };
            objectMother.queryRegion.Expect(call => call.Query()).Return(new Region[] { objectMother.region2 }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(regionInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Region_Has_Been_Saved()
        {
            //Arrange
            RegionInputModel regionInputModel = new RegionInputModel()
            {
                Id = objectMother.region.Id,
                Name = objectMother.region.Name,
                CountryId = objectMother.region.Country.Id
            };
            objectMother.queryCountry.Expect(call => call.Load(objectMother.countryId)).Return(objectMother.country);
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Region>.Matches(p => p.Name == objectMother.region.Name)));
            objectMother.queryRegion.Expect(call => call.Query()).Return(new Region[]{}.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(regionInputModel);

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Region Transilvania has been saved."));
        }
    }
}
