using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ExistingRequests;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ExistingRequests_Tests
{
    [TestFixture]
    public class GetExistingRequestsMethod
    {
        ObjectMother _ = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {

            _.Init();
        }

        [Test]
        public void RequiresUserAndClientToHaveBeenLoaded()
        {
            _.controller.GetExistingRequests(_.ExistingRequestsInput());

            _.VerifyUserAndClientExpectations();

        }

        [Test]
        public void QueriesForAllRequestRecords()
        {

            _.controller.GetExistingRequests(_.ExistingRequestsInput());
            _.VerifyExistingRequestsQueried();
        }

        [Test]
        public void ReturnsThe_Requests_As_JsonResult()
        {
            var model = _.ExistingRequestsInput();

            var result = _.controller.GetExistingRequests(model);

            var output = result.Data as StoreOutputModel<GetExistingRequestModel>;

            Assert.AreEqual(100, output.TotalItems);

            Assert.AreEqual(model.limit.Value, output.Items.Count());

        }
      
    }
}
