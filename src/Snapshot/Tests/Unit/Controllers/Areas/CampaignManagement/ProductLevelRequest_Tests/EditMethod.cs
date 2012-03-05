using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    [TestFixture]
    public class EditMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void CreatesANewProductLevelRequest()
        {
            var input = _.EditInput();
            _.controller.Edit(input);

            _.VerifyEditExpectations(input);
        }
    }
}
