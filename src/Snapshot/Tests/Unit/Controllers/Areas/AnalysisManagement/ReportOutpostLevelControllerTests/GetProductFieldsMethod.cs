using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tests.Unit.Controllers.Areas.ReportOutpostLevelControllerTests;
using Web.Areas.AnalysisManagement.Controllers;
using Web.Areas.AnalysisManagement.Models.ReportOutpostLevel;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.ReportOutpostLevelControllerTests
{
    [TestFixture]
    public class GetProductFieldsMethod
    {
        private ObjectMother objMother = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            objMother.Init();
        }

        [Test]
        public void WhenOnlyUnderThreshold_IsFalse_ReturnsACertainListOfProducts()
        {
            objMother.queryOSL.Expect(it => it.Query()).Return(objMother.oslList.AsQueryable());

            string result = objMother.controller.GetProductFields(new ReportOutpostLevelInputModel() { OnlyUnderTreshold = false });

            Assert.IsNotNull(result);
            Assert.AreEqual("ProductAboveThreshold,ProdUnderThreshold", result);

        }

        [Test]
        public void WhenOnlyUnderThreshold_IsTrue_ReturnsACertainListOfProducts()
        {
            objMother.queryOSL.Expect(it => it.Query()).Return(objMother.oslList.AsQueryable());

            string result = objMother.controller.GetProductFields(new ReportOutpostLevelInputModel() { OnlyUnderTreshold = true });

            Assert.IsNotNull(result);
            Assert.AreEqual("ProdUnderThreshold", result);
        }

    }
}
