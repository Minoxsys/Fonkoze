using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{

    [TestFixture]
    public class GetSchedulesMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void LoadsUserAndClient()
        {
            _.controller.GetSchedules();

            _.VerifyUserAndClientExpectations();

        }

        [Test]
        public void QueriesForSchedules()
        {
            _.controller.GetSchedules();

            _.VerifySchedulesQueried();
        }

        [Test]
        public void JsonResult_ContainsAnArray_Of_ScheduleModel_Results()
        {
            _.StubSchedulesData();

            var result = _.controller.GetSchedules();

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<ScheduleModel[]>(result.Data);

            var scheduleModel = result.Data as ScheduleModel[];

            Assert.AreEqual(scheduleModel.Count(), 10);
            Assert.AreEqual(scheduleModel[0].Reminders.Count(), 1);
        }
    }
}
