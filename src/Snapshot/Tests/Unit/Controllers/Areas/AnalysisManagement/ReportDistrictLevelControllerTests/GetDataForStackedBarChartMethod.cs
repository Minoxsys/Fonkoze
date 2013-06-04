using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.ReportDistrictLevel;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportDistrictLevelControllerTests
{
    [TestFixture]
    public class GetDataForStackedBarChartMethod
    {
        private readonly ObjectMother objMother = new ObjectMother();
                
        [SetUp]
        public void BeforeEach()
        {
            objMother.InitForChart();
        }

        [Test]
        public void ReturnJSON_WithTheRightData()
        {
            objMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objMother.oslList.AsQueryable());

            var jsonResult = objMother.controller.GetDataForStackedBarChart(Guid.Empty,null,null);//the params aren't that important to test here

            Assert.IsNotNull(jsonResult);
            var jsonData = jsonResult.Data as StoreOutputModel<DistrictStackedBarChartModel>;
            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual(2, jsonData.Items[0].Products.Count);
            foreach(var p in jsonData.Items[0].Products)
            {
                if (p.ProductName == "ProdUnderThreshold")
                {
                     Assert.AreEqual(7,p.StockLevel);
                }
                else
                {
                    Assert.AreEqual(20, p.StockLevel);
                }
            }
           

        }

      

        
    }
}
