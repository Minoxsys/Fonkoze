using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Country;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class GetCountriesMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Countries_For_User()
        {
            //Arange
            var currentUserCountries = objectMother.CurrentUserCountries();
            objectMother.queryCountry.Expect(call => call.Query()).Return(currentUserCountries);

            //Act
            var jsonResult = objectMother.controller.GetCountries();

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<CountryIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as CountryIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(currentUserCountries.Count(), jsonData.TotalItems);           
        }
    }
}
