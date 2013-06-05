using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Domain;
using Web.Areas.AnalysisManagement.Models.ReportDistrictLevel;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportDistrictLevelControllerTests
{
    [TestFixture]
    public class GetProductsForChart
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void GetProductsForChart_ForNull_productGroupId_Returns_JSON_with_EmptyData()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetProductsForChart(null, objectMother.districtId);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<ProductsChartModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ProductsChartModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void GetProductsForChart_ForNull_districtId_Returns_JSON_with_EmptyData()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetProductsForChart(objectMother.productGroupId, null);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<ProductsChartModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ProductsChartModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void GetProductsForChart_ForProductGroupId_And_DistrictId_Returns_JSON_with_DataForCharts()
        {
            //Arrange
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetProductsForChart(objectMother.productGroupId, objectMother.districtId);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<ProductsChartModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ProductsChartModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("product", jsonData.Items[0].ProductName);
            Assert.AreEqual("5", jsonData.Items[0].StockLevel);
        }
    }
}
