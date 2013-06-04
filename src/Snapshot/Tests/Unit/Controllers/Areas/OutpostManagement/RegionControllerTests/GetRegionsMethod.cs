using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Region;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.Shared;

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
            var indexModel = new IndexTableInputModel
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
            var jsonResult = objectMother.controller.GetRegions(indexModel, string.Empty);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<RegionModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<RegionModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Regions_For_Country_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name",
            };

            var pageOfData = objectMother.PageOfRegionData(indexModel);
            objectMother.queryRegion.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetRegions(indexModel, objectMother.countryId.ToString());

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();

            var jsonData = jsonResult.Data as StoreOutputModel<RegionModel>;

            Assert.That(jsonData.Items[0].Name, Is.EqualTo("RegionName9"));
            Assert.That(jsonData.Items[0].CountryId, Is.EqualTo(objectMother.countryId));

        }
    }
}
