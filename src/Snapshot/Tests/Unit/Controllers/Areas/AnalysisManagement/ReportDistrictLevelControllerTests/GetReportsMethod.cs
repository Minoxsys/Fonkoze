using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.ReportDistrictLevel;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportDistrictLevelControllerTests
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
        public void Get_Reports_ReturnData_SpecificToAllDistricts_BeacauseOf_DistrictIdSpecificToAllOptionProvided()
        {
            //arrange
            var reportInputModel = new ReportDistrictLevelInputModel
            {                
                CountryId = objectMother.country.Id,
                RegionId = objectMother.region.Id,
                DistrictId = objectMother.ID_ALL_OPTION_FOR_DISTRICTS
            };

            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());

            //act
            var result = objectMother.controller.GetReports(reportInputModel);


            //assert
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<ReportDistrictLevelTreeModel>(result.Data);

            var reportDistrictLevelTree = (ReportDistrictLevelTreeModel)result.Data;
            Assert.AreEqual(reportDistrictLevelTree.children.Count, 3);
            Assert.AreEqual(reportDistrictLevelTree.children[2].Name, "district2 ( Number of Sellers: 1 ) ");
            Assert.AreEqual(reportDistrictLevelTree.children[2].children[0].children[0].ProductLevelSum, "5");
        }


        [Test]
        public void Get_Reports_Return_OneDistrictLevel_SpecificTo_DistrictIdProvided()
        {
            //arrange
            var reportInputModel = new ReportDistrictLevelInputModel
            {
                CountryId = objectMother.country.Id,
                RegionId = objectMother.region.Id,
                DistrictId = objectMother.districts[0].Id
            };

            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevelList.AsQueryable());
            objectMother.queryOutposts.Expect(it => it.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());

            //act
            var result = objectMother.controller.GetReports(reportInputModel);

            //assert
            objectMother.queryOutpostStockLevel.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<ReportDistrictLevelTreeModel>(result.Data);

            var reportDistrictLevelTree = (ReportDistrictLevelTreeModel)result.Data;
            Assert.AreEqual(reportDistrictLevelTree.children.Count, 1);
            Assert.AreEqual(reportDistrictLevelTree.children[0].children[0].children[0].ProductLevelSum, "0");

        }
    }
}
