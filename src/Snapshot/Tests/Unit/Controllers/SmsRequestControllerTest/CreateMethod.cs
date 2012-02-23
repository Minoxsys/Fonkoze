using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using System.Web.Mvc;
using Web.Models.SmsRequest;
using Core.Domain;

namespace Tests.Unit.Controllers.SmsRequestControllerTest
{
    [TestFixture]
    public class CreateMethod
    {
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_Controller_And_Mock_Services();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Get_Create_Returns_View_With_A_ListOfOutpost_And_A_ListOfProductGroups()
        {
            // Arange
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.queryServiceOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());
            objectMother.queryServiceProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { objectMother.productGroup }.AsQueryable());

            //Act
            var result = (ViewResult)objectMother.controller.Create();

            //Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.queryServiceOutpost.VerifyAllExpectations();
            objectMother.queryServiceProductGroup.VerifyAllExpectations();

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<SmsRequestCreateModel>(result.Model);
        }

        [Test]
        public void Post_Create_On_Success_Saves_The_SmsRequest_And_Sends_It()
        {
            // Arrange
            SmsRequestCreateModel model = new SmsRequestCreateModel()
            {
                Outpost = new ReferenceModel() { Id = objectMother.outpostId, Name = objectMother.outpost.Name },
                ProductGroup = new ReferenceModel() { Id = objectMother.productGroupId, Name = objectMother.productGroup.Name }
            };

            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.smsRequestService.Expect(call => call.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpostId, objectMother.productGroupId, objectMother.client)).Return(objectMother.smsRequest);

            objectMother.smsGatewayService.Expect(call => call.SendSmsRequest(objectMother.smsRequest)).Return("Post return");

            // Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Create(model);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            objectMother.smsGatewayService.VerifyAllExpectations();

            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("SmsRequest", redirectResult.RouteValues["Controller"]);
        }

        [Test]
        public void Post_Create_On_Failure_Redirects_To_Home()
        {
            // Arrange
            SmsRequestCreateModel model = new SmsRequestCreateModel()
            {
                Outpost = new ReferenceModel() { Name = string.Empty },
                ProductGroup = new ReferenceModel() { Name = string.Empty }
            };
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);

            // Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Create(model);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            Assert.AreEqual("Create", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Post_Create_On_SmsCreation_Redirects_To_Home()
        {
            // Arrange
            SmsRequestCreateModel model = new SmsRequestCreateModel()
            {
                Outpost = new ReferenceModel() { Id = objectMother.outpostId, Name = objectMother.outpost.Name },
                ProductGroup = new ReferenceModel() { Id = objectMother.productGroupId, Name = objectMother.productGroup.Name }
            };
            objectMother.queryServiceUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());
            objectMother.queryServiceClients.Expect(call => call.Load(objectMother.clientId)).Return(objectMother.client);
            objectMother.smsRequestService.Expect(call => call.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpostId, objectMother.productGroupId, objectMother.client)).Return(objectMother.nullSmsRequest);

            // Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Create(model);

            // Assert
            objectMother.queryServiceUsers.VerifyAllExpectations();
            objectMother.queryServiceClients.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();

            Assert.AreEqual("Index", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("Home", redirectResult.RouteValues["Controller"]);
        }
    }
}
