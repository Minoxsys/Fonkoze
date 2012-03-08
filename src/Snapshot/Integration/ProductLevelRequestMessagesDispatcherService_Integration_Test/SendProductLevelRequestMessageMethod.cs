using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace IntegrationTests.ProductLevelRequestMessagesDispatcherService_Integration_Test
{
    [TestFixture]
    public class SendProductLevelRequestMessageMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            _.Setup_Stub_Data();
            _.Setup_DispatcherService_And_Required_Services();
        }

        [TearDown]
        public void AfterAll()
        {
            _.Delete_StubData();
        }

        [Test]
        public void Receives_A_ProductLevelRequest_And_Sends_An_SMS_And_An_Email()
        {
            // Arrange
            _.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { _.contact1, _.contact2 }.AsQueryable());
            _.queryServiceOutpostStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { _.outpostStockLevel1, _.outpostStockLevel2 }.AsQueryable());
            _.queryServiceOutpost.Expect(call => call.Load(_.outpost1.Id)).Return(_.outpost1);
            _.queryServiceOutpost.Expect(call => call.Load(_.outpost2.Id)).Return(_.outpost2);
            _.urlService.Expect(call => call.GetEmailLinkUrl(Arg<string>.Is.Anything)).Return("LINKURL");

            // Act
            _.service.DispatchMessagesForProductLevelRequest(_.productLevelRequest);
            // Assert
            _.queryServiceOutpostStockLevel.VerifyAllExpectations();
        }
    }
}
