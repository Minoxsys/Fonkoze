using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Controllers;

namespace Tests.Unit.Controllers.HomeControllerTests
{
    [TestFixture]
    public class GetNoOfAlertsInLast2DaysMethod
    {
        [Test]
        public void ReturnStringWithTheNumberOfAlertsInTheLast2Days()
        { 
          //Arrange
           IQueryService<Alert> QueryAlerts = MockRepository.GenerateMock<IQueryService<Alert>>();
           Alert oneAlert = MockRepository.GeneratePartialMock<Alert>();
           List<Alert> lstAlert = new List<Alert>();
           lstAlert.Add(oneAlert);
           QueryAlerts.Expect(call => call.Query()).Return(lstAlert.AsQueryable());
           HomeController controller = new HomeController();
           controller.QueryAlerts = QueryAlerts;

            //Act           
            string result = controller.GetNoOfAlertsInLast2Days();

            //Assert
            QueryAlerts.VerifyAllExpectations();
            Assert.AreEqual("1", result);
        
        }
    }
}
