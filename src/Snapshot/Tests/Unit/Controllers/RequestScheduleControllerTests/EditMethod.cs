using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;
using Web.Areas.CampaignManagement.Models.RequestSchedule;

namespace Tests.Unit.Controllers.RequestScheduleControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Request_Has_Been_Updated()
        {
            //Arrange

            objectMother.queryServicetSchedule.Expect(call => call.Load(objectMother.scheduleId)).Return(objectMother.schedule);
            objectMother.deleteCommandRequestReminder.Expect(call => call.Execute(objectMother.reminder));

            objectMother.saveCommandSchedule.Expect(call => call.Execute(Arg<Schedule>.Matches(
                    s => s.Id == objectMother.scheduleId &&
                        s.Name == ObjectMother.SCHEDULE_NAME &&
                        s.ScheduleBasis == ObjectMother.SCHEDULE_BASIS &&
                        s.FrequencyType == ObjectMother.FREQUENCY_TYPE &&
                        s.FrequencyValue == ObjectMother.FREQUENCY_VALUE &&
                        s.StartOn == ObjectMother.START_ON &&
                        s.Reminders.Count == 1
                    )));

            //Act
            var jsonResult = objectMother.controller.Edit(objectMother.inputModel);

            //Assert
            objectMother.queryServicetSchedule.VerifyAllExpectations();
            objectMother.deleteCommandRequestReminder.VerifyAllExpectations();
            objectMother.saveCommandSchedule.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Request " + ObjectMother.SCHEDULE_NAME + " has been updated."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RequestScheduleInputModel_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RequestScheduleInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a scheduleId in order to edit the schedule."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RequestScheduleInputModel_Has_An_Invalid_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new RequestScheduleInputModel() { Id = Guid.NewGuid() });

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply the scheduleId of a schedule that exists in the DB in order to edit it."));
        }
    }
}
