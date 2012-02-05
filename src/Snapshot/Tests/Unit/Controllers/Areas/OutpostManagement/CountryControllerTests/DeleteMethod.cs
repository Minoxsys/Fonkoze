using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Fact = NUnit.Framework.TestAttribute;
using BeforeEach = NUnit.Framework.SetUpAttribute;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [BeforeEach]
        public void BeforeEach()
        {
            objectMother.Init();
        }

        [Fact]
        public void Returns_JSON_With_ErrorMessage_When_CountryId_IsNull()
        {
            var jsonResult = objectMother.controller.Delete(null);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a countryId in order to remove the country"));
        }

        [Fact]
        public void Executes_DeleteCommand_WithTheSelectedCountry()
        {
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Country>.Matches(p => p.Id == objectMother.entityId)));

            //act 
            var jsonResult = objectMother.controller.Delete(objectMother.entityId);

            //asert

            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Country Romania was removed."));
        }

        [Fact]
        public void CannotRemoveCountry_With_Regions()
        {
            objectMother.SetupQueryRegionToReturnARegionForThisCountry();

            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Country>.Matches(p => p.Id == objectMother.entityId)));

            //act 
            var jsonResult = objectMother.controller.Delete(objectMother.entityId);

            //asert

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("Country Romania has 1 region(s) associated, and can not be removed."));
        }
    }
}