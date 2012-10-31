using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ExistingRequests;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ExistingRequests_Tests
{
    [TestFixture]
    public class GetOutpostsMethod
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
            _.controller.GetOutposts(_.OutpostInput());

            _.VerifyUserAndClientExpectations();

        }
        [Test]
        public void LoadsTheGivenCampaignById()
        {
            var input = _.OutpostInput();
            _.controller.GetOutposts(input);

            _.VerifyCampaignExpectations(input.CampaignId);
        }

        [Test]
        public void QueriesOutpostsFoundIn_Campaing_OptionsModel()
        {

            var input = _.OutpostInput();
            _.controller.GetOutposts(input);

            _.VerifyOutpostQueryExpectation();
        }

        [Test]
        public void Returns_All_the_Outposts_Associated_With_The_Campaign()
        {

            var input = _.OutpostInput();
            var output = _.controller.GetOutposts(input);

            Assert.AreEqual(_.outposts.Count+1, (output.Data as GetOutpostsOutput[]).Length);
        }

        [Test]
        public void Returns_Null_WhenCampaign_Has_No_Outposts()
        {

            var input = _.OutpostInputWithNoOutpostsInCampaign();
            var output = _.controller.GetOutposts(input);

            Assert.IsNull(output.Data);

        }
    }
}
