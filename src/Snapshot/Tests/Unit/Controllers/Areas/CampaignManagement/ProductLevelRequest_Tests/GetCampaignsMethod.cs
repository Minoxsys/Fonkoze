using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{ 
    [TestFixture]
    public class GetCampaignsMethod
    {
        ObjectMother _ = new ObjectMother();
        [SetUp]
        public void Init()
        {
            _.Init();

        }

        [Test]
        public void LoadsUserAndClient()
        {
            _.controller.GetCampaigns();

            _.VerifyUserAndClientExpectations();
        }

        [Test]
        public void QueriesForProductsWithTheGivenProductId()
        {
            _.controller.GetCampaigns();

            _.VerifyCampaignsQueried();
        }

        [Test]
        public void ReturnsAListOfCampaigns()
        {
            var result = _.controller.GetCampaigns();

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<CampaignModel[]>(result.Data);
        }
    }
}
