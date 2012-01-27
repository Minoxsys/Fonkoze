using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Country;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    [TestFixture]
    public class OverviewMethod
    {
        readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Get_ReturnsTheViewModel_WithTheWorldCountriesLoaded()
        {
            objectMother.queryWorldCountryRecords.Expect(call => call.Query()).Return( objectMother.WorldCountryRecords());
            // Act
            var viewResult = (ViewResult)objectMother.controller.Overview();

           // Assert
            objectMother.queryWorldCountryRecords.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);

            Assert.AreEqual(ObjectMother.DEFAULT_VIEW_NAME, viewResult.ViewName);
        
        }
    }
}
