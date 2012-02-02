using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Domain;

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
            objectMother.queryCountry.Expect(call => call.Query()).Return(new Country[] { objectMother.country }.AsQueryable());

            // Act
            var viewResult = (ViewResult)objectMother.controller.Overview(objectMother.countryId);

            // Assert
            objectMother.queryCountry.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual("", viewResult.ViewName);
        }
    }
}
