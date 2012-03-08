using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Region;
using Rhino.Mocks;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.RegionControllerTests
{
    [TestFixture]
    public class GetRegionsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new RegionIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };
            var pageOfData = objectMother.PageOfRegionData(indexModel);
            objectMother.queryRegion.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetRegions(indexModel);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<RegionIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as RegionIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Regions_For_Country_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new RegionIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name",
                countryId = objectMother.countryId.ToString()
            };

            var pageOfData = objectMother.PageOfRegionData(indexModel);
            objectMother.queryRegion.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetRegions(indexModel);

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();

            var jsonData = jsonResult.Data as RegionIndexOuputModel;

            Assert.That(jsonData.Regions[0].Name, Is.EqualTo("RegionName9"));
            Assert.That(jsonData.Regions[0].CountryId, Is.EqualTo(objectMother.countryId));

        }
    }
}
