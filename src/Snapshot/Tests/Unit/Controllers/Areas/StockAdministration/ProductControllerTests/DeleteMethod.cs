using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductControllerTests
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
        public void Should_Return_JSon_WithSuccessMessage_WhenProduct_AreRemovedSuccessufully()
        {
            //arrange
            objectMother.queryService.Expect(it => it.Load(objectMother.product.Id)).Return(objectMother.product);
            objectMother.deleteProduct.Expect(it => it.Execute(Arg<Product>.Matches(di => di.Id == objectMother.product.Id)));
            objectMother.queryHistoricalOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostHistoricalStockLevel[] { }.AsQueryable());
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostStockLevel[] { }.AsQueryable());
            //act
            var result = objectMother.controller.Delete(objectMother.product.Id);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            objectMother.deleteProduct.VerifyAllExpectations();
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryHistoricalOutpostStockLevel.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;

            Assert.AreEqual(response.Status, "Success");
            Assert.AreEqual(response.Message, "The product StockItem1 has been deleted!");
        }

        [Test]
        public void Should_Return_JSon_WithErrorMessage_WhenProduct_AreAsociatedToOutpostStockLevel()
        {
            //arrange
            objectMother.queryService.Expect(it => it.Load(objectMother.product.Id)).Return(objectMother.product);
            objectMother.queryHistoricalOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostHistoricalStockLevel[] { }.AsQueryable());
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostStockLevel[] { objectMother.outpostStockLevel}.AsQueryable());
            //act
            var result = objectMother.controller.Delete(objectMother.product.Id);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryHistoricalOutpostStockLevel.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;

            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The product StockItem1 has stock level available, so it can not be deleted");
        }
        [Test]
        public void Should_Return_JSon_WithErrorMessage_WhenProduct_AreAsociatedToOutpostHystoricalStockLevel()
        {
            //arrange
            objectMother.queryService.Expect(it => it.Load(objectMother.product.Id)).Return(objectMother.product);
            objectMother.queryHistoricalOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostHistoricalStockLevel[] {objectMother.outpostHystoricalStockLevel }.AsQueryable());
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(new OutpostStockLevel[] {  }.AsQueryable());
            //act
            var result = objectMother.controller.Delete(objectMother.product.Id);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryHistoricalOutpostStockLevel.VerifyAllExpectations();

            var response = result.Data as JsonActionResponse;

            Assert.AreEqual(response.Status, "Error");
            Assert.AreEqual(response.Message, "The product StockItem1 has stock level history available , so it can not be deleted");
        }

    }
}
