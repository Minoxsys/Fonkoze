using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.AssignProductGroupController_Tests
{
    [TestFixture]
    public class GetOutpostStockLevels_Method
    {
        readonly ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void ChecksThatUserIsLoggedIn_InOrder_To_RetriveIts_Client()
        {
            _.controller.GetOutpostStockLevels(_.FakeOutpostStockLevelInput());

            _.VerifyUserAndClientExpectations();

        }

        [Test]
        public void StockLevelsAreQueried_BasedOn_CurrentClient_AndOutpost()
        {

            _.FakeOutpostStockLevels();

            _.controller.GetOutpostStockLevels(_.FakeOutpostStockLevelInput());

            _.VerifyQueryOnOutpostStockLevelService();
        }

        [Test]
        public void StockLevelsAreReturned_AsJson_Result()
        {
            _.FakeOutpostStockLevels();

            var result = _.controller.GetOutpostStockLevels(_.FakeOutpostStockLevelInput());

            Assert.IsNotNull(result);

        }

        [Test]
        public void StockLevelsAreReturned_InJsonData_AsAnArray()
        {
            _.FakeOutpostStockLevels();

            var result = _.controller.GetOutpostStockLevels(_.FakeOutpostStockLevelInput());

            Assert.IsNotNull(result.Data);

            Assert.IsInstanceOf<AssignProductGroupController.OutpostStockLevelModel[]>(result.Data);

            Assert.IsNotEmpty((AssignProductGroupController.OutpostStockLevelModel[])result.Data);

        }
    }
}
