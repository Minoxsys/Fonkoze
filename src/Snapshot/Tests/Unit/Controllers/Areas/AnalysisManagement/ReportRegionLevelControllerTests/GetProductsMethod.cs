using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportRegionLevelControllerTests
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
        public void GetProducts_ForNull_Id_Returns_JSON_with_EmptyData()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetProducts(null);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ProductsReferenceOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ProductsReferenceOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void GetProducts_ForProductGroupId_Returns_JSON_with_AllProductsForThatProductGroup()
        {
            //Arrange
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetProducts(objectMother.productGroupId);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ProductsReferenceOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ProductsReferenceOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
