using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.EmailRequest;

namespace Tests.Unit.Controllers.EmailRequestControllerTest
{
    [TestFixture]
    public class ResponseMethod
    {
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_Controller_and_MockServices();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Get_Response_IfLink_Has_NOT_Expired_RedirectTo_OutpostStockLevel_Overview()
        {
            //Arrange
            string encodedResponse = GenerateEncodedString(DateTime.Today);
            objectMother.queryServiceEmailRequest.Expect(call => call.Load(objectMother.emailRequest.Id)).Return(objectMother.emailRequest);
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpost.Id)).Return(objectMother.outpost);

            //Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Response(encodedResponse);

            //Assert
            objectMother.queryServiceEmailRequest.VerifyAllExpectations();
            objectMother.queryServiceOutpost.VerifyAllExpectations();

            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("OutpostStockLevel", redirectResult.RouteValues["Controller"]);
        }

        private string GenerateEncodedString(DateTime time)
        {
            EmailRequestModel model = new EmailRequestModel() { Id = objectMother.emailRequest.Id.ToString(), Date = time.ToString() };
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serializer.Serialize(model)));
        }

        [Test]
        public void Get_Response_IfLink_Has_Expired_RedirectTo_Home_Index()
        {
            //Arrange
            string resultString = GenerateEncodedString(DateTime.UtcNow.AddDays(-6));

            //Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Response(resultString);

            //Assert
            Assert.AreEqual("Index", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("Home", redirectResult.RouteValues["Controller"]);
        }
    }
}
