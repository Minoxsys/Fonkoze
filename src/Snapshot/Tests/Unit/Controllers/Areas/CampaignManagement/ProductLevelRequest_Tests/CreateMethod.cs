using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{

    [TestFixture]
    public class CreateMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void CreatesANewProductLevelRequest()
        {
            _.controller.Create(_.CreateInput());

            _.VerifyCreateExpectations();
        }

        [Test]
        public void Sets_TheCampaign_As_Open()
        {
            _.controller.Create(_.CreateInput());

            _.VerifyCampaignStatusSavedOnCreate();

        }
    }
}
