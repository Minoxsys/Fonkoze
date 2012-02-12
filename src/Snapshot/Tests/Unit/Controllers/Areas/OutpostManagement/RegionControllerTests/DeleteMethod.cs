using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;
using Core.Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RegionId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.loadClient.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a regionId in order to remove the region"));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelectedRegion()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Load(objectMother.regionId)).Return(objectMother.region);
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { }.AsQueryable());
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Region>.Matches(p => p.Id == objectMother.regionId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.regionId);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.loadClient.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Region Transilvania was removed."));
        }

        [Test]
        public void CannotRemoveRegion_With_Districts()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Load(objectMother.regionId)).Return(objectMother.region);
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { objectMother.district }.AsQueryable());
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Region>.Matches(p => p.Id == objectMother.regionId)));
            objectMother.queryUsers.VerifyAllExpectations();
            objectMother.loadClient.VerifyAllExpectations();

            //Act 
            var jsonResult = objectMother.controller.Delete(objectMother.regionId);

            //Assert

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("Region Transilvania has 1 district(s) associated, and can not be removed."));
        }
    }
}
