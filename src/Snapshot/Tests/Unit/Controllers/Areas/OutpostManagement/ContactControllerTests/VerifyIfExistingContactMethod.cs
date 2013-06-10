using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.ContactControllerTests
{
    [TestFixture]
    class VerifyIfExistingContactMethod
    {
        readonly ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void VerifyIfExistingContact_ReturnSuccess_WhenContactIsNotExistent()
        {
            var existentContacts = new List<string>{"321"};
            _.StubLoadOutpost();
            _.StubQueryContact("123");

            var result = _.controller.VerifyIfContactExistent(existentContacts, Guid.Empty);

            Assert.AreEqual("OK", ((JsonActionResponse)result.Data).Status);
        }

        [Test]
        public void VerifyIfExistingContact_ReturnsErrorAndExistentContactNames_WhenActiveContactIsExistent()
        {
            var existentContacts = new List<string> { "123" };
            _.StubLoadOutpost();
            _.StubQueryContact("123");

            var result = _.controller.VerifyIfContactExistent(existentContacts, Guid.Empty);

            Assert.AreEqual("ERROR", ((JsonActionResponse)result.Data).Status);
            Assert.AreEqual("The following active Contact details already exist: 123.", ((JsonActionResponse)result.Data).Message);
        }
    }
}
