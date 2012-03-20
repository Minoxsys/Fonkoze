using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{

    [TestFixture]
    public class RecalculateProductLevelRequestDetailsMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void Loads_TheGivenProductLevelRequest()
        {
            Guid productLevelRequestId = Guid.NewGuid();

            _.SetupProductLevelRequest(productLevelRequestId);

            _.controller.RecalculateProductLevelRequestDetails(productLevelRequestId);

            _.VerifyProductLevelRequestLoadedById(productLevelRequestId);
        }
        [Test]
        public void GeneratesProductLevelDetails()
        {
            Guid productLevelRequestId = Guid.NewGuid();

            _.SetupProductLevelRequest(productLevelRequestId);

            _.controller.RecalculateProductLevelRequestDetails(productLevelRequestId);

            _.VerifyProductLevelRequestDetailsGenerated();
        }

    }
}
