﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Models.Product;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductControllerTests
{
    [TestFixture]
    public class GetProductsMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }
        [Test]
        public void Return_Products_Paginated_SpecificToIndexModel_ValueFields_When_ProductGroupIdProvided_And_SortedDESCByName()
        {
            //arrange
            var indexModel = new ProductIndexModel()
            {
                Limit = 50,
                dir = "DESC",
                sort = "Name",
                ProductGroupId = objectMother.productGroup.Id,
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfProductData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);

            //act
            var result = objectMother.controller.GetProducts(indexModel);

            //assert
            Assert.IsInstanceOf<ProductIndexOutputModel>(result.Data);
            var resultData = (ProductIndexOutputModel)result.Data;
            Assert.AreEqual(resultData.products[0].Name, "Product9");
            Assert.AreEqual(resultData.TotalItems, pageOfData.Count());
        }
        [Test]
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_ProductGroupIdIsNull_AndSearchName_HasValue_And_SortedASCByName()
        {

            //arrange
            var indexModel = new ProductIndexModel()
            {
                Limit = 50,
                dir = "ASC",
                sort = "Name",
                SearchName = "o",
                ProductGroupId = null,
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfProductData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);

            //act
            var result = objectMother.controller.GetProducts(indexModel);

            //assert
            Assert.IsInstanceOf<ProductIndexOutputModel>(result.Data);
            var resultData = (ProductIndexOutputModel)result.Data;
            Assert.AreEqual(resultData.products[0].Name, "Product0");
            Assert.AreEqual(resultData.TotalItems, pageOfData.Count());

        }

    }
}
