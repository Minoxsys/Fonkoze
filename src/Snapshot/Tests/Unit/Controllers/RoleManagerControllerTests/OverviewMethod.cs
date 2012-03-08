using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.RoleManagerControllerTests
{
    [TestFixture]
    public class OverviewMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init_Controller_And_Mock_Services();
        }

        [Test]
        public void Returns_The_ViewModel()
        {
            //Arrange

            // Act
            //var viewResult = (ViewResult)objectMother.controller.Overview();

            // Assert
            //Assert.IsNull(viewResult.Model);
            //Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
