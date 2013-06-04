using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.ReportOutpostLevelControllerTests
{
    [TestFixture]
    public class GetDataForStackedBarChartMethod
    {
        private readonly ObjectMother objMother = new ObjectMother();
                
        [SetUp]
        public void BeforeEach()
        {
            objMother.Init();
        }

        [Test]
        public void ForOnlyUnderThreshold_False_ReturnJSON_WithDataForChart()
        {
            objMother.queryOSL.Expect(it => it.Query()).Return(objMother.oslList.AsQueryable());

            var jsonResult = objMother.controller.GetDataForStackedBarChart(new ReportOutpostLevelInputModel() {OnlyUnderTreshold=false});

            Assert.IsNotNull(jsonResult);
            dynamic jsonData = jsonResult.Data;
            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual(2, jsonData.Items[0].Products.Count);

        }

        [Test]
        public void ForOnlyUnderThreshold_True_ReturnJSON_WithDataForChart()
        {
            objMother.queryOSL.Expect(it => it.Query()).Return(objMother.oslList.AsQueryable());

            var jsonResult = objMother.controller.GetDataForStackedBarChart(new ReportOutpostLevelInputModel() { OnlyUnderTreshold=true});

            Assert.IsNotNull(jsonResult);
            dynamic jsonData = jsonResult.Data;
            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual(1, jsonData.Items[0].Products.Count);
            Assert.AreEqual("ProdUnderThreshold", jsonData.Items[0].Products[0].ProductName);
        }

        
    }
}
