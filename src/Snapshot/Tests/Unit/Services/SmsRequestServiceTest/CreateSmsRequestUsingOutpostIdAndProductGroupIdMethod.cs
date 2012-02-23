using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Persistence.Queries.Products;
using Web.Services;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    public class CreateSmsRequestUsingOutpostIdAndProductGroupIdMethod
    {
        private const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n{1}";
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_SmsRequestService_And_MockServices();
            objectMother.Setup_Stub_Data();
        }

        [Test]
        public void Given_An_Outpost_Id_And_An_ProductGroupId_The_Method_Will_Generate_An_SmsRequest()
        {
            // Arrange
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpostId)).Return(objectMother.outpost);
            objectMother.queryServiceProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable());

            objectMother.saveCommandSmsRequest.Expect(it => it.Execute(Arg<SmsRequest>.Matches(
                r => r.ProductGroupId.Equals(objectMother.productGroupId) &&
                    r.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                     r.OutpostId.Equals(objectMother.outpostId) &&
                     r.Message == SmsRequestService.MESSAGE_NOT_DELIVERED &&
                     r.Number.Equals("1234567890") &&
                     r.Client == objectMother.client
                )));

            objectMother.saveCommandSmsRequest.Expect(it => it.Execute(Arg<SmsRequest>.Matches(
                r => r.ProductGroupId.Equals(objectMother.productGroupId) &&
                    r.ProductGroupReferenceCode == ObjectMother.PRODUCT_GROUP_REFERENCE_CODE &&
                     r.OutpostId.Equals(objectMother.outpostId) &&
                     r.Number.Equals("1234567890") &&
                     r.Client == objectMother.client &&
                     r.Message.Equals(string.Format(SMS_MESSAGE_TEMPLATE, "Malaria", "MAL R0"))
                )));

            // Act
            SmsRequest smsRequest = objectMother.smsRequestService.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpostId, objectMother.productGroupId, objectMother.client);

            // Assert
            objectMother.queryServiceOutpost.VerifyAllExpectations();
            objectMother.queryServiceProductGroup.VerifyAllExpectations();
            objectMother.queryServiceStockLevel.VerifyAllExpectations();
            objectMother.saveCommandSmsRequest.VerifyAllExpectations();

            Assert.IsNotNull(smsRequest);
        }

        [Test]
        public void If_The_Outpost_Does_Not_Have_A_Main_Contact_Then_No_Sms_Is_Generated()
        {
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpostId)).Return(objectMother.outpostWithNoMainContact);
            objectMother.queryServiceProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable());

            SmsRequest smsRequest = objectMother.smsRequestService.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpostId, objectMother.productGroupId, objectMother.client);

            Assert.AreEqual(Guid.Empty, smsRequest.Id);
        }

        [Test]
        public void If_The_Outpost_Does_Not_Have_A_Telephone_Contact_Then_No_Sms_Is_Generated()
        {
            objectMother.queryServiceOutpost.Expect(call => call.Load(objectMother.outpostId)).Return(objectMother.outpostWithNoNumberContact);
            objectMother.queryServiceProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable());

            SmsRequest smsRequest = objectMother.smsRequestService.CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(objectMother.outpostId, objectMother.productGroupId, objectMother.client);

            Assert.AreEqual(Guid.Empty, smsRequest.Id);
        }
    }
}
