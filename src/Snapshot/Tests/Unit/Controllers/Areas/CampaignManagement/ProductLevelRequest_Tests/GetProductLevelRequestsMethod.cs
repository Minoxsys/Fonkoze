using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    [TestFixture]
    public class GetProductLevelRequestsMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();

        }

        [Test]
        public void ProductLevelRequest_AreQueried()
        {
            _.controller.GetProductLevelRequests(_.GetProductLevelRequestsInput());

            _.VerifyProductLevelRequestsQueried();

        }

        [Test]
        public void ProductLevelRequest_ReturnsAJsonResponse()
        {
            var result = _.controller.GetProductLevelRequests(_.GetProductLevelRequestsInput());

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<GetProductLevelRequestResponse>(result.Data);

            var response = result.Data as GetProductLevelRequestResponse;
            Assert.AreEqual(response.TotalItems, 200);
            Assert.AreEqual(response.ProductLevelRequests.Length, 10);
            Assert.AreEqual(response.ProductLevelRequests[0].ProductSmsCodes, "a");
        }

        [Test]
        public void ProductLevelRequest_HaveNull_When_ScheduleIsConsideredToBe_Now()
        {
            var result = _.controller.GetProductLevelRequests(_.GetProductLevelRequestsInput());

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<GetProductLevelRequestResponse>(result.Data);

            var response = result.Data as GetProductLevelRequestResponse;
            Assert.AreEqual(response.TotalItems, 200);
            Assert.AreEqual(response.ProductLevelRequests.Length, 10);
            Assert.AreEqual(response.ProductLevelRequests[0].ScheduleId, Guid.Empty.ToString());
            Assert.AreEqual(response.ProductLevelRequests[0].ScheduleName, "Now");
        }

    }
}
