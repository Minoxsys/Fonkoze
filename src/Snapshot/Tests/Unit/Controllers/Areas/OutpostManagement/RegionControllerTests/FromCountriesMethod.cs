using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class FromCountriesMethod
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
            var redirectResult = (RedirectToRouteResult)objectMother.controller.FromCountries(null);

            // Assert
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }
    }
}
