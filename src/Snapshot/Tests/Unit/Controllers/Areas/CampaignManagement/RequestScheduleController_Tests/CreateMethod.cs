using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;
using Core.Domain;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.RequestScheduleController_Tests
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
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);

            objectMother.saveCommandSchedule.Expect(call => call.Execute(Arg<Schedule>.Matches(
                    s => s.Name == ObjectMother.SCHEDULE_NAME &&
                        s.FrequencyType == "Every " + ObjectMother.FREQUENCY_VALUE + " day(s)" &&
                        s.FrequencyValue == ObjectMother.FREQUENCY_VALUE &&
                        s.StartOn == ObjectMother.START_ON &&
                        s.Reminders.Count == 1 &&
                        s.Client == objectMother.client
                    )));

            //Act
            var jsonResult = objectMother.controller.Create(objectMother.inputModel);

            //Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.saveCommandSchedule.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Request " + ObjectMother.SCHEDULE_NAME + " has been saved."));
        }
    }
}
