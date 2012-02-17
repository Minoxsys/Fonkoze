using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.RequestScheduleControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Request_Has_Been_Saved()
        {
            //Arrange

            objectMother.saveCommandRequestSchedule.Expect(call => call.Execute(Arg<RequestSchedule>.Matches(
                    s => s.ScheduleName == ObjectMother.SCHEDULE_NAME &&
                        s.ScheduleBasis == ObjectMother.SCHEDULE_BASIS &&
                        s.FrequencyType == ObjectMother.FREQUENCY_TYPE &&
                        s.FrequencyValue == ObjectMother.FREQUENCY_VALUE &&
                        s.StartOn == ObjectMother.START_ON &&
                        s.Reminders.Count == 1
                    )));

            //Act
            var jsonResult = objectMother.controller.Create(objectMother.inputModel);

            //Assert
            objectMother.saveCommandRequestSchedule.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Request " + ObjectMother.SCHEDULE_NAME + " has been saved."));
        }
    }
}
