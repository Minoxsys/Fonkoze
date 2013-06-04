using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Country;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    [TestFixture]
    public class IndexMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page= 1,
                start = 0,
                sort = "Name"
            };

            var pageOfData = objectMother.ExpectQueryCountryToReturnPageOfCountryDataBasedOn(indexModel);
            //act

            var jsonResult = objectMother.controller.Index(indexModel);

            objectMother.queryCountry.VerifyAllExpectations();
            dynamic jsonData = jsonResult.Data;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual( pageOfData.Count(), jsonData.TotalItems);           
        }
  
       

        [Test]
        public void Orders_ByName_DESC()
        {
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            objectMother.ExpectQueryCountryToReturnPageOfCountryDataBasedOn(indexModel);

            //act

            var jsonResult = objectMother.controller.Index(indexModel);


            //assert
            objectMother.queryCountry.VerifyAllExpectations();

            dynamic jsonData = jsonResult.Data;

            Assert.That(jsonData.Countries[0].Name, Is.EqualTo("CountryAtIndex9"));
           
        }

        [Test]
        public void Orders_ByPhonePrefix_DESC()
        {
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "PhonePrefix"
            };

            objectMother.ExpectQueryCountryToReturnPageOfCountryDataBasedOn(indexModel);
            //act

            var jsonResult = objectMother.controller.Index(indexModel);


            //assert
            objectMother.queryCountry.VerifyAllExpectations();

            dynamic jsonData = jsonResult.Data;

            Assert.That(jsonData.Countries[0].PhonePrefix, Is.EqualTo("00049"));

        }
    }
}
