using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Services;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    [TestFixture]
    public class CreateSmsRequestsForProductLevelRequestMethod
    {
        private const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n{1}";

        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_SmsRequestService_And_MockServices();
            objectMother.Setup_Stub_Data();
            objectMother.Setup_ProductLevelRequest();
        }

        [Test]
        public void Given_A_ProductLevelRequest_The_Method_Will_Return_A_List_Of_SmsRequests()
        {
            // Arrange
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpostId)).Return(objectMother.outpost);
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable());
            objectMother.saveCommandSmsRequest.Expect(call => call.Execute(Arg<SmsRequest>.Matches(s => 
                s.Client == objectMother.client &&
                s.Number == ObjectMother.PHONE_NUMBER &&
                s.OutpostId == objectMother.outpostId &&
                s.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                s.Message == SmsRequestService.MESSAGE_NOT_DELIVERED
            )));

            objectMother.saveCommandSmsRequest.Expect(call => call.Execute(Arg<SmsRequest>.Matches(s =>
                s.Client == objectMother.client &&
                s.Number == ObjectMother.PHONE_NUMBER &&
                s.OutpostId == objectMother.outpostId &&
                s.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                s.Message.Equals(string.Format(SMS_MESSAGE_TEMPLATE, ObjectMother.PRODUCT_GROUP_NAME, ObjectMother.PRODUCT_GROUP_REFERENCE_CODE + " A0"))
            )));

            // Act
            var result = objectMother.smsRequestService.CreateSmsRequestsForProductLevelRequestForClient(objectMother.productLevelRequest, objectMother.client);

            // Assert
            objectMother.queryServiceOutpost.VerifyAllExpectations();
            objectMother.queryServiceStockLevel.VerifyAllExpectations();
            objectMother.saveCommandSmsRequest.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<SmsRequest>>(result);
            Assert.AreEqual(1, result.Count);
        }
    }
}
