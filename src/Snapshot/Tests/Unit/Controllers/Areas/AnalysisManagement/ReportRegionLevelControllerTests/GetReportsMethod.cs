using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.ReportRegionLevel;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportRegionLevelControllerTests
{
    [TestFixture]
    public class GetReportsMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Get_Reports_ReturnData_SpecificToAllRegions_BeacauseOf_JustCountryIdProvided()
        {
            //arrange
            var reportInputModel = new FilterInputModel
            {
                CountryId = objectMother.country.Id,
                RegionId = Guid.Empty
            };

            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[]{objectMother.outpost}.AsQueryable());

            //act
            var result = objectMother.controller.GetReports(reportInputModel);


            //assert
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<ReportRegionLevelTreeModel>(result.Data);

            var reportRegionLevelTree = (ReportRegionLevelTreeModel)result.Data;
            Assert.AreEqual(reportRegionLevelTree.children.Count, 3);
            Assert.AreEqual(reportRegionLevelTree.children[2].Name, "region2 ( Sellers:1 ) ");
            Assert.AreEqual(reportRegionLevelTree.children[2].children[0].children[0].ProductLevelSum, "5");
        }

        [Test]
        public void Get_Reports_Return_OneRegionLevel_SpecificTo_RegionIdProvided()
        {
            //arrange
            var reportInputModel = new FilterInputModel
            {
                CountryId = objectMother.country.Id,
                RegionId = objectMother.regions[0].Id
            };

            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());

            //act
            var result = objectMother.controller.GetReports(reportInputModel);


            //assert
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<ReportRegionLevelTreeModel>(result.Data);

            var reportRegionLevelTree = (ReportRegionLevelTreeModel)result.Data;
            Assert.AreEqual(reportRegionLevelTree.children.Count, 1);
            Assert.AreEqual(reportRegionLevelTree.children[0].children[0].children[0].ProductLevelSum, "0");
 
        }
    }
}
