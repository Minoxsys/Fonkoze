using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Country;
using Rhino.Mocks;
using System.Web.Mvc;

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
            var indexModel = new CountryIndexModel
            {
                dir = "ASC",
                limit = 50,
                page= 1,
                start = 0,
                sort = "Name",
            };

            var pageOfData = objectMother.PageOfCountryData(indexModel);

            objectMother.queryCountry.Expect(call => call.Query()).Return(pageOfData);
            //act

            var jsonResult = objectMother.controller.Index(indexModel);

            objectMother.queryCountry.VerifyAllExpectations();
            Assert.That(jsonResult, Is.InstanceOfType<JsonResult>());
            Assert.That(jsonResult.Data, Is.InstanceOfType<CountryIndexOutputModel>());
            var jsonData = jsonResult.Data as CountryIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual( pageOfData.Count(), jsonData.TotalItems);           

        }
    }
}
