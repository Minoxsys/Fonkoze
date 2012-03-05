using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    [TestFixture]
    public class StopProductLevelRequestMethod
    {
        [Test]
        public void UpdatesTheProductRequests_IsStopped_Property()
        {
            ObjectMother _ = new ObjectMother();

            _.controller.StopProductLevelRequest(_.StopProductLevelRequestInput());

            _.VerifySaveCommandInvoked_With_ProductLevelRequestSetTo_True();
        }
    }
}
