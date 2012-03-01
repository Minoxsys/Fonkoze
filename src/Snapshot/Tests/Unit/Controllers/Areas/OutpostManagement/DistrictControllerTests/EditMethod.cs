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
    public class EditMethod
    {

        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Should_ReturnJSon_ErrorMessage_WhenModelStateIsNotValid()
        {
 
            //act
            var result = objectMother.controller.Edit(new DistrictInputModel());

            var response = result.Data as JsonActionResponse;

            //assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The district has not been updated!");
        }

        [Test]
        public void Should_ReturnJSon_SuccessMessage_When_EditSucceeded()
        {
            //arrange
            var districtInputModel = new DistrictInputModel();
            districtInputModel.Id = objectMother.district.Id;
            districtInputModel.Name = objectMother.district.Name;
            districtInputModel.Region.Id = objectMother.district.Region.Id;

            objectMother.queryService.Expect(it => it.Load(objectMother.district.Id)).Return(objectMother.district);
            objectMother.queryRegion.Expect(it => it.Load(objectMother.region.Id)).Return(objectMother.region);
            objectMother.queryClient.Expect(it => it.Load(objectMother.client.Id)).Return(objectMother.client);
            objectMother.saveCommand.Expect(it => it.Execute(Arg<District>.Matches(st => st.Name.Equals(objectMother.district.Name) && st.Id == objectMother.district.Id)));
            objectMother.queryService.Expect(it => it.Query()).Return(new District[] { }.AsQueryable());
            //act
            var result = objectMother.controller.Edit(districtInputModel);

            //assert
            objectMother.saveCommand.VerifyAllExpectations();
            objectMother.queryService.VerifyAllExpectations();
            objectMother.queryRegion.VerifyAllExpectations();
            objectMother.queryClient.VerifyAllExpectations();
            var response = result.Data as JsonActionResponse;

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
            var result = objectMother.controller.Edit(districtInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The region already contains a district with the name Cluj! Please insert a different name!");

        }
    }
}
