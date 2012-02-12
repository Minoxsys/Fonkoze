using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.DistrictControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Should_Return_JSon_WithErrorMessage_When_DistrictIdProvided_IsNull()
        { 
            //act
            var result = objectMother.controller.Delete(null);

            //assert
            var response = result.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "You must supply a district id in order to delete the district!");

        }

        [Test]
        public void Should_Return_JSon_WithSuccessMessage_WhenDistrict_AreRemovedSuccessufully()
        {
            //arrange
            objectMother.queryService.Expect(it => it.Load(objectMother.district.Id)).Return(objectMother.district);
            objectMother.deleteCommand.Expect(it => it.Execute(Arg<District>.Matches(di => di.Id == objectMother.district.Id)));
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { }.AsQueryable());

            //act
            var result = objectMother.controller.Delete(objectMother.district.Id);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;

            Assert.AreEqual(response.Status, "Success");
            Assert.AreEqual(response.Message, "The district Cluj has been deleted!"); 
        }

        [Test]
        public void Should_Return_JSonResult_WithErrorMessage_WhenTheDistrictHas_OutpostAsociated()
        {
            //arrange
            objectMother.queryService.Expect(it => it.Load(objectMother.district.Id)).Return(objectMother.district);
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost}.AsQueryable());

            //act
            var result = objectMother.controller.Delete(objectMother.district.Id);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;

            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The district Cluj has outposts associated, so it can not be deleted!"); 
 
        }
    }
}
