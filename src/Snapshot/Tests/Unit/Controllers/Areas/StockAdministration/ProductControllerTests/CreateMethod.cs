using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Models.Product;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductControllerTests
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
            var result = objectMother.controller.Create(new ProductInputModel());

            //assert
            var response = result.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The product has not been saved!"));

        }

        [Test]
        public void Should_Return_JSonSuccessMessage_WhenModelIsValid_And_DistrictSaveSucceeded()
        {
            //arrange
            var productInputModel = new ProductInputModel();
            productInputModel.Name = objectMother.product.Name;
            productInputModel.ProductGroup.Id = objectMother.product.ProductGroup.Id;
            productInputModel.SMSReferenceCode = objectMother.product.SMSReferenceCode;

            objectMother.saveOrUpdateProduct.Expect(it => it.Execute(Arg<Product>.Matches(st => st.Name.Equals(objectMother.product.Name))));
            objectMother.queryService.Expect(it => it.Query()).Return(new Product[] { }.AsQueryable());
            //act
            var result = objectMother.controller.Create(productInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.saveOrUpdateProduct.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Success");
            Assert.AreEqual(response.Message, "The product has been saved!");

        }

        [Test]
        public void Should_Return_JSonErrorMessage_When_ThereAreProductsWith_TheSameName_ForSameProductGroup()
        {
            //arrange
            var productInputModel = new ProductInputModel();
            productInputModel.Name = objectMother.product.Name;
            productInputModel.ProductGroup.Id = objectMother.product.ProductGroup.Id;
            productInputModel.SMSReferenceCode = objectMother.product.SMSReferenceCode;

            objectMother.queryService.Expect(it => it.Query()).Return(new Product[] { objectMother.product}.AsQueryable());
            //act
            var result = objectMother.controller.Create(productInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The Product Group already contains a product with the name StockItem1! Please insert a different name!"); 
        }
    }
}
