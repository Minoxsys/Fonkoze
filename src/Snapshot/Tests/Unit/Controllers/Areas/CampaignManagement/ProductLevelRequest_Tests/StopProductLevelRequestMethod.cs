using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    [TestFixture]
    public class StopProductLevelRequestMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void QueriesTheProductLevelRequests_ById_IsStopped_Property()
        {

            _.controller.StopProductLevelRequest(_.StopProductLevelRequestInput());

            _.VerifyThatLoadWasCalledOnQueryProductLevelRequests();
        }
        [Test]
        public void UpdatesTheProductRequests_IsStopped_Property()
        {

            _.controller.StopProductLevelRequest(_.StopProductLevelRequestInput());

            _.VerifySaveCommandInvoked_With_ProductLevelRequestSetTo_True();
        }
    }
}
