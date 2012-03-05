using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
    [TestFixture]
    public class FromDistrictsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Redirects_To_Overview()
        {
            //Arrange

            // Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.FromDistricts(null);

            // Assert
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }
    }
}
