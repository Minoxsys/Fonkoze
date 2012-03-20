using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{

    [TestFixture]
    public class GetProductLevelRequestDetailsMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void QueriesForProductLevelDetailsBasedOnTheGivenId()
        {
            Guid productLevelRequestId = _.GetProductLevelRequestDetailsInput();

            _.controller.GetProductLevelRequestDetails(productLevelRequestId);

            _.VerifyProductLevelRequestDetailsQueried();

        }

        [Test]
        public void GenerateFoobar()
        {
            Guid productLevelRequestId = _.GetProductLevelRequestDetailsInput();

            var result  = _.controller.GetProductLevelRequestDetails(productLevelRequestId);

            Assert.IsInstanceOf<GetProductLevelRequestDetailsModel>(result.Data);

            var model = result.Data as GetProductLevelRequestDetailsModel;

            Assert.AreEqual(model.ProductLevelRequestDetails.Count(), 200);
           

        }
      

    }
}
