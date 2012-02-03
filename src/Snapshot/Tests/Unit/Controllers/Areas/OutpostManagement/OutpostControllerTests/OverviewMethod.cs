using System;
using System.Linq;
using NUnit.Framework;
using System.Web.Mvc;


namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{

    [TestFixture]
    public class OverviewMethod
    {
        public readonly ObjectMother _ = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }
        [Test]
        public void Returns_The_DefaultView()
        {

            var viewResult = _.controller.Overview() as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.Empty);
        }
    }
}
