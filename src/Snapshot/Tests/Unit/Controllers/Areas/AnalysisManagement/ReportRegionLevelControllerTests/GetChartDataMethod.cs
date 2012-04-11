using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportRegionLevelControllerTests
{
    [TestFixture]
    public class GetChartDataMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void GetGetChartData_ForNull_ProductGroupId_Returns_EmptyJSON()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetChartData(null);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ChartReferenceOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ChartReferenceOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void GetGetChartData_ForProductGroupId_Returns_JSONWithDataForChart()
        {
            //Arrange
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetChartData(objectMother.productGroupId);

            //Assert
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ChartReferenceOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ChartReferenceOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(3, jsonData.TotalItems);
            Assert.AreEqual("product", jsonData.Products[0].ProductName);
            Assert.AreEqual("region0", jsonData.Products[0].RegionName);
            Assert.AreEqual("region1", jsonData.Products[1].RegionName);
            Assert.AreEqual("region2", jsonData.Products[2].RegionName);
            Assert.AreEqual(0, jsonData.Products[0].StockLevelSum);
            Assert.AreEqual(1, jsonData.Products[1].StockLevelSum);
            Assert.AreEqual(5, jsonData.Products[2].StockLevelSum);
        }
    }
}
