using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.District;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.DistrictControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Should_ReturnJsonWithErrorMessage_WhenModelState_Invalid()
        {
 
            //act
            var result = objectMother.controller.Create(new DistrictInputModel());

            //assert
            var response = result.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The district has not been saved!"));

        }

        [Test]
        public void Should_Return_JSonSuccessMessage_WhenModelIsValid_And_DistrictSaveSucceeded()
        {
            //arrange
            var districtInputModel = new DistrictInputModel();
            districtInputModel.Name = objectMother.district.Name;
            districtInputModel.Region.Id = objectMother.district.Region.Id;

            objectMother.saveCommand.Expect(it => it.Execute(Arg<District>.Matches(st=>st.Name.Equals(objectMother.district.Name))));
            objectMother.queryService.Expect(it => it.Query()).Return(new District[] { }.AsQueryable());
            //act
            var result = objectMother.controller.Create(districtInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.saveCommand.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Success");
            Assert.AreEqual(response.Message, "District Cluj has been saved.");
 
        }

        [Test]
        public void Should_Return_JSonErrorMessage_WhenModelIsValid_But_DistrictNameAlreadyExistsInTheRegion()
        {
            //arrange
            var districtInputModel = new DistrictInputModel();
            districtInputModel.Name = objectMother.district.Name;
            districtInputModel.Region.Id = objectMother.district.Region.Id;

            objectMother.queryService.Expect(it => it.Query()).Return(new District[] { objectMother.district }.AsQueryable());
            //act
            var result = objectMother.controller.Create(districtInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The region already contains a district with the name Cluj! Please insert a different name!");

        }

    }
}
