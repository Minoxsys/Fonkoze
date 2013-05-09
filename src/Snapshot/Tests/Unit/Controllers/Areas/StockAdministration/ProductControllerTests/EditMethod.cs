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
            var result = objectMother.controller.Edit(new ProductInputModel());

            var response = result.Data as JsonActionResponse;

            //assert
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The product has not been updated!");
        }

        [Test]
        public void Should_ReturnJSon_SuccessMessage_When_ThereAreNoProducts_WithSameName_And_EditSucceeded()
        {
            //arrange
            var productInputModel = SetProductInputModelFields();

            objectMother.queryService.Expect(it => it.Load(objectMother.product.Id)).Return(objectMother.product);
            objectMother.saveOrUpdateProduct.Expect(
                it => it.Execute(Arg<Product>.Matches(st => st.Name.Equals(objectMother.product.Name) && st.Id == objectMother.product.Id)));
            objectMother.queryService.Expect(it => it.Query()).Return(new Product[] {}.AsQueryable());

            //act
            var result = objectMother.controller.Edit(productInputModel);

            //assert
            objectMother.saveOrUpdateProduct.VerifyAllExpectations();
            objectMother.queryService.VerifyAllExpectations();
            var response = result.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Success");
            Assert.AreEqual(response.Message, "The product " + objectMother.product.Name + " has been updated!");
        }

        [Test]
        public void Should_ReturnJSon_ErrorMessage_When_ThereAreProducts_WithSameName_ForSameProductGroup()
        {
            //arrange
            var productInputModel = SetProductInputModelFields();

            objectMother.queryService.Expect(it => it.Query()).Return(new Product[] {objectMother.product2}.AsQueryable());

            //act
            var result = objectMother.controller.Edit(productInputModel);

            //assert
            var response = result.Data as JsonActionResponse;
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The Product Group already contains a product with the name StockItem1! Please insert a different name!"); 

        }

        private ProductInputModel SetProductInputModelFields()
        {
            var productInputModel = new ProductInputModel();
            productInputModel.Id = objectMother.product.Id;
            productInputModel.Name = objectMother.product.Name;
            productInputModel.ProductGroup.Id = objectMother.product.ProductGroup.Id;
            return productInputModel;
        }
    }
}
