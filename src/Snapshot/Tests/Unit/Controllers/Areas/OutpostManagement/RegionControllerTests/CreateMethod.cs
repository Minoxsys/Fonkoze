using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Web.Areas.OutpostManagement.Models.Region;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ModalState_IsNOT_Valid()
        {
            //Arrange
            
            //Act
            var jsonResult = objectMother.controller.Create(new RegionInputModel());
            
            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The regionMock has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Region_Has_Been_Saved()
        {
            //Arrange
            RegionInputModel regionInputModel = new RegionInputModel()
            {
                Name = objectMother.region.Name,
                Coordinates = objectMother.region.Coordinates,
                CountryId = objectMother.region.Country.Id
            };
            objectMother.queryCountry.Expect(call => call.Load(objectMother.countryId)).Return(objectMother.country);

            //Act
            var jsonResult = objectMother.controller.Create(regionInputModel);

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Region Transilvania has been saved."));
        }

    }
}
