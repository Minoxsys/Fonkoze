using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_ProductGroupId_IsNull()
        {
            //Arrange
            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a pproductGroupId in order to remove the product group."));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelected_ProductGroup()
        {
            //Arrange
            objectMother.queryProduct.Expect(call => call.Query()).Return(new Product[]
            {
                new Product
                {
                    ProductGroup = new ProductGroup()
                }
            }.AsQueryable());
            objectMother.queryProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<ProductGroup>.Matches(p => p.Id == objectMother.productGroup.Id)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.productGroup.Id);

            //Assert
            objectMother.queryProduct.VerifyAllExpectations();
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Product Group Malaria was removed."));
        }

        [Test]
        public void CannotRemoveProductGroup_With_Products()
        {
            //Arrange
            objectMother.queryProduct.Expect(call => call.Query()).Return(new Product[] { objectMother.product }.AsQueryable());
            objectMother.queryProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<ProductGroup>.Matches(p => p.Id == objectMother.productGroup.Id)));

            //Act 
            var jsonResult = objectMother.controller.Delete(objectMother.productGroup.Id);

            //Assert

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The Product Group Malaria has 1 product(s) associated, so it can not be deleted."));
        }
    }
}