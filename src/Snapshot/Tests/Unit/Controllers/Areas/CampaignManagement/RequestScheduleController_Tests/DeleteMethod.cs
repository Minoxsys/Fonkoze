using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Core.Domain;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.RequestScheduleController_Tests
{
    [TestFixture]
    public class DeleteMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RequestScheduleId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;

            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a scheduleId in order to remove the schedule."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_RoleId_Is_Invalid()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(Guid.NewGuid());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply the scheduleId of a role that exists in the DB in order to remove it."));
        }

        [Test]
        public void Executes_DeleteCommand_WithTheSelectedUser()
        {
            //Arrange
            objectMother.queryServicetSchedule.Expect(call => call.Load(objectMother.scheduleForClientId)).Return(objectMother.scheduleForClient);
            objectMother.deleteCommandRequestReminder.Expect(call => call.Execute(objectMother.reminder));
            objectMother.deleteCommandRequestSchedule.Expect(call => call.Execute(Arg<Schedule>.Matches(r => r.Id == objectMother.scheduleForClientId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.scheduleForClientId);

            //Assert
            objectMother.queryServicetSchedule.VerifyAllExpectations();
            objectMother.deleteCommandRequestReminder.VerifyAllExpectations();
            objectMother.deleteCommandRequestSchedule.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);

            var response = jsonResult.Data as JsonActionResponse;

            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Schedule " + ObjectMother.SCHEDULE_NAME + " was removed."));
        }
    }
}
