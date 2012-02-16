using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.SmsGatewayServiceTest
{
    [TestFixture]
    public class AssignOutpostToRawSmsReceivedBySenderNumber
    {
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.SetupSmsService_and_MockServices();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Assigns_The_OutpostId_To_The_RawSmsReceived_By_Associating_The_Sender_With_The_Outpost_Contact_Number()
        {
            // Arange
            objectMother.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact}.AsQueryable<Contact>());
            objectMother.queryOutposts.Expect(call => call.GetAllContacts()).Return(new Outpost[] { objectMother.outpost }.AsQueryable<Outpost>());

            // Act
            RawSmsReceived result = objectMother.smsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsReceived);

            // Assert
            objectMother.queryServiceContact.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsNotNull(result);
            Assert.AreEqual(objectMother.outpostId, result.OutpostId);
        }

        [Test]
        public void Does_Not_Assign_OutpostId_To_The_RawSmsReceived_Because_Does_Not_Find_Outpost_With_The_Sender_Number()
        {
            // Arange
            objectMother.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact }.AsQueryable<Contact>());
            objectMother.queryOutposts.Expect(call => call.GetAllContacts()).Return(new Outpost[] { objectMother.outpost }.AsQueryable<Outpost>());

            // Act
            RawSmsReceived result = objectMother.smsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsReceivedWithWrongNumber);

            // Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsNotNull(result);
            Assert.AreEqual(Guid.Empty, result.OutpostId);
        }
    }
}
