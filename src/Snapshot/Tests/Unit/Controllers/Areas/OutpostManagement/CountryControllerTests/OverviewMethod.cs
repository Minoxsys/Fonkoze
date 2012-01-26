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
        public void Get_WithParam_Page_ReturnsTheViewModel_WithTheFirstPageLoaded()
        {
            const int PAGE_NUMBER = 1;
            // Arrange		

            objectMother.queryCountry.Expect(call => call.Query()).Return(new Country[] { objectMother.fakeCountry }.AsQueryable());
           

            // Act
            var viewResult = (ViewResult)objectMother.controller.Overview();

           // Assert

            Assert.IsNotNull(viewResult.Model);

            var viewModel = (CountryOverviewModel)viewResult.Model;

            Assert.IsNotNull(viewModel);

            Assert.That(viewModel.Countries.Count, Is.EqualTo(1));

            Assert.AreEqual(ObjectMother.DEFAULT_VIEW_NAME, viewResult.ViewName);
        
        }
    }
}
