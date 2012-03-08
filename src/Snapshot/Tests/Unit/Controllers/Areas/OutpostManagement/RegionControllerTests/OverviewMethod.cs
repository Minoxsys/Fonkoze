using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Domain;
using Web.Areas.OutpostManagement.Models.Region;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
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
            //Assert.AreEqual("Overview", viewResult.ViewName);
            //Assert.IsInstanceOf<FromCountryModel>(viewResult.Model);
        }

    }
}
