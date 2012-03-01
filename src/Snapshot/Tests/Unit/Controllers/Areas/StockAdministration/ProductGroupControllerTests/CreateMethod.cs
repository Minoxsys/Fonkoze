using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
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
            var jsonResult = objectMother.controller.Create(new ProductGroupInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The Product Group has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ProductGroupName_AlreadyExists()
        {
            //Arrange
            ProductGroupInputModel productGroupInputModel = new ProductGroupInputModel()
            {
                Name = objectMother.productGroup.Name,
                Description = objectMother.productGroup.Description,
                ReferenceCode = objectMother.productGroup.ReferenceCode
            };
            objectMother.queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { objectMother.productGroup }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(productGroupInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_ProductGroup_Has_Been_Saved()
        {
            //Arrange
            ProductGroupInputModel productGroupInputModel = new ProductGroupInputModel()
            {
                Name = objectMother.productGroup.Name + "a",
                Description = objectMother.productGroup.Description,
                ReferenceCode = objectMother.productGroup.ReferenceCode
            };
            objectMother.queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { objectMother.productGroup }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(productGroupInputModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Product Group Malariaa has been saved."));
        }
    }
}
