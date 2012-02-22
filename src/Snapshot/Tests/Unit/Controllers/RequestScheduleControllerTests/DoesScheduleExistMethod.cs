using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;
using Core.Domain;

namespace Tests.Unit.Controllers.RequestScheduleControllerTests
{
    [TestFixture]
    public class DoesScheduleExistMethod
    {
        public const string NEW_SCHEDULE_NAME = "New Schedule Name";

        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void It_Receives_A_Schedule_Name_That_Does_Not_Exist_In_Db_And_Returns_A_Not_Found_Message()
        {
            // Arrange
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.queryServicetSchedule.Expect(call => call.Query()).Return(new Schedule[] { objectMother.scheduleForClient }.AsQueryable());

            // Act
            var result = objectMother.controller.DoesScheduleExist(NEW_SCHEDULE_NAME);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.queryServicetSchedule.VerifyAllExpectations();

            Assert.IsNotNull(result);

            var response = result.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("NotFound"));
            Assert.That(response.Message, Is.EqualTo("Schedule " + NEW_SCHEDULE_NAME + " was not found in the DB."));
        }

        [Test]
        public void It_Receives_A_Schedule_Name_That_Exists_In_Db_And_Returns_A_Found_Message()
        {
            // Arrange
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.queryServicetSchedule.Expect(call => call.Query()).Return(new Schedule[] { objectMother.scheduleForClient }.AsQueryable());

            // Act
            var result = objectMother.controller.DoesScheduleExist(ObjectMother.SCHEDULE_NAME);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.queryServicetSchedule.VerifyAllExpectations();

            Assert.IsNotNull(result);

            var response = result.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("There is a schedule with the name " + ObjectMother.SCHEDULE_NAME + " in the DB."));
        }
    }
}
