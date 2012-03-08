using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    [TestFixture]
    public class OverviewMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
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
