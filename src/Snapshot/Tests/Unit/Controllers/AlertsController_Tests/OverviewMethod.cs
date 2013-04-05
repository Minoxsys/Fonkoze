using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.AlertsController_Tests
{
    [TestFixture]
    public class OverviewMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();

        [SetUp]
        public void PerTestSetup()
        {
            _objectMother.Init();
        }

        [Test]
        public void Returns_The_ViewModel()
        {
            // Act
            var viewResult = (ViewResult)_objectMother.controller.Overview();

            // Assert
            Assert.IsNull(viewResult.Model);
            Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
