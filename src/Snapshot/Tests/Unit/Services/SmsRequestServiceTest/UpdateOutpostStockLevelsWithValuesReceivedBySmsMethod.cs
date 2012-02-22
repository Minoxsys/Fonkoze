using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.SmsRequestServiceTest
{
    [TestFixture]
    public class UpdateOutpostStockLevelsWithValuesReceivedBySmsMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_SmsRequestService_And_MockServices();
            objectMother.Setup_Stub_Data();
        }

        [Test]
        public void Receives_An_SmsReceived_And_Inserts_OutpostHistoricalStockLevel_And_Updates_OutpostStockLevel()
        {
            // Arrange
            objectMother.queryServiceSmsRequest.Expect(call => call.Query()).Return(new SmsRequest[] { objectMother.smsRequest }.ToList<SmsRequest>().AsQueryable<SmsRequest>());
            objectMother.queryServiceStockLevel.Expect(call => call.Query()).Return(objectMother.stockLevels.AsQueryable<OutpostStockLevel>());

            objectMother.saveCommandOutpostHistoricalStockLevel.Expect(call => call.Execute(Arg<OutpostHistoricalStockLevel>.Matches(
                r => r.OutpostId.Equals(objectMother.outpostId) &&
                r.ProductGroupId.Equals(objectMother.productGroupId) &&
                r.ProdSmsRef.Equals(ObjectMother.SMS_REFERENCE_CODE) &&
                r.StockLevel.Equals(ObjectMother.STOCK_LEVEL) &&
                r.PrevStockLevel.Equals(0) &&
                r.UpdateMethod.Equals(ObjectMother.MANUAL_UPDATED_METHOD)
            )));

            objectMother.saveCommandOutpostStockLevel.Expect(call => call.Execute(Arg<OutpostStockLevel>.Matches(
                r => r.Outpost.Id.Equals(objectMother.outpostId) &&
                r.ProductGroup.Id.Equals(objectMother.productGroupId) &&
                r.Product.SMSReferenceCode.Equals(ObjectMother.SMS_REFERENCE_CODE) &&
                r.StockLevel.Equals(ObjectMother.RECEIVED_STOCK_LEVEL) &&
                r.PrevStockLevel.Equals(ObjectMother.STOCK_LEVEL) &&
                r.UpdateMethod.Equals(ObjectMother.SMS_UPDATED_METHOD)
            )));

            // Act
            objectMother.smsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(objectMother.smsReceived);
            
            // Assert
            objectMother.queryServiceSmsRequest.VerifyAllExpectations();
            objectMother.queryServiceStockLevel.VerifyAllExpectations();
            objectMother.saveCommandOutpostHistoricalStockLevel.VerifyAllExpectations();
            objectMother.saveCommandOutpostStockLevel.VerifyAllExpectations();
        }

        [Test]
        public void Receives_An_SmsReceived_With_An_Unknown_Phone_Number_And_Updates_Nothing()
        {
            // Arrange
            objectMother.queryServiceSmsRequest.Expect(call => call.Query()).Return(new SmsRequest[] { objectMother.smsRequestWithDifferentPhoneNumber }.ToList<SmsRequest>().AsQueryable<SmsRequest>());

            // Act
            objectMother.smsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(objectMother.smsReceived);

            // Assert
            objectMother.queryServiceSmsRequest.VerifyAllExpectations();
        }
    }
}
