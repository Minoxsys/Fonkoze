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

        [Test]
        public void Runs_ProductLevelRequestMessagesDispatcherService_When_ScheduleId_Is_Guid_Empty()
        {
            var input = _.CreateWithEmptyScheduleInput();
            _.controller.Create(input);

            _.VerifyProductLevelRequestMessagesDispatcherServiceExpectations();
        }

        [Test]
        public void Generates_ProductLevelRequestDetails()
        {

            _.controller.Create(_.CreateInput());
            _.VerifyProductLevelRequestDetailsGenerated();
        }
    }
}
