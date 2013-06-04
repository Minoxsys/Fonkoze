using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using System.Web.Mvc;
using Web.Areas.CampaignManagement.Models.RequestSchedule;
using Core.Domain;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.RequestScheduleController_Tests
{
    [TestFixture]
    public class GetListOfRequestSchedulesMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
            objectMother.Init_Stub_Data();
        }

        [Test]
        public void Return_JSON_With_List_Of_RequestSchedules()
        {
            // Arrange
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.queryServicetSchedule.Expect(call => call.Query()).Return(new Schedule[] { objectMother.scheduleForClient, objectMother.scheduleForOtherClient }.AsQueryable());

            // Act
            var jsonResult = objectMother.controller.GetListOfRequestSchedules(objectMother.indexTableInputModel);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.queryServicetSchedule.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf(typeof(JsonResult), jsonResult);
            Assert.IsInstanceOf(typeof(RequestScheduleListForJsonOutput), jsonResult.Data);

            var jsonData = jsonResult.Data as RequestScheduleListForJsonOutput;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.RequestSchedules.Count());
            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
