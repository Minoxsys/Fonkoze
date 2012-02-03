using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.District;
using Rhino.Mocks;
using System.Web.Mvc;
using Domain;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.DistrictControllerTests
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
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_Just_RegionIdHasValue_And_SortedDESCByName()
        {
            var indexModel = new DistrictIndexModel() {
                Limit = 50,
                dir = "DESC",
                sort = "Name",
                CountryId = null,
                RegionId = objectMother.region.Id,
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfDistrictData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { }.AsQueryable());

            //act
            var jsonResult = objectMother.controller.Index(indexModel);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsInstanceOf<JsonResult>(jsonResult);

            var jsonData = jsonResult.Data as DistrictIndexOutputModel;

            Assert.AreEqual(jsonData.districts[0].Name, "District9");
            Assert.AreEqual(jsonData.TotalItems, pageOfData.Count());

        }

        [Test]
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_Just_RegionIdHasValue_And_SortedASCByName()
        {
            var indexModel = new DistrictIndexModel()
            {
                Limit = 50,
                dir = "ASC",
                sort = "Name",
                CountryId = null,
                RegionId = objectMother.region.Id,
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfDistrictData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { }.AsQueryable());

            //act
            var jsonResult = objectMother.controller.Index(indexModel);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsInstanceOf<JsonResult>(jsonResult);

            var jsonData = jsonResult.Data as DistrictIndexOutputModel;

            Assert.AreEqual(jsonData.districts[0].Name, "District0");
            Assert.AreEqual(jsonData.TotalItems, pageOfData.Count());

        }
        [Test]
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_Just_RegionIdHasValue_And_SortedASCByName_And_SearchingDistrictByName()
        {
            var indexModel = new DistrictIndexModel()
            {
                Limit = 50,
                dir = "ASC",
                sort = "Name",
                CountryId = objectMother.country.Id,
                RegionId = null,
                SearchName = "Dis",
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfDistrictData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { }.AsQueryable());

            //act
            var jsonResult = objectMother.controller.Index(indexModel);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsInstanceOf<JsonResult>(jsonResult);

            var jsonData = jsonResult.Data as DistrictIndexOutputModel;

            Assert.AreEqual(jsonData.districts[0].Name, "District0");
            Assert.AreEqual(jsonData.TotalItems, pageOfData.Count());

        }
        [Test]
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_Just_RegionIdHasValue_And_SortedASCByName_And_SearchingDistrictByName_ReturnNoResult()
        {
            var indexModel = new DistrictIndexModel()
            {
                Limit = 50,
                dir = "ASC",
                sort = "Name",
                CountryId = objectMother.country.Id,
                RegionId = null,
                SearchName = "some",
                Start = 0,
                Page = 1
            };

            var pageOfData = objectMother.PageOfDistrictData(indexModel);
            objectMother.queryService.Expect(it => it.Query()).Return(pageOfData);
            objectMother.queryOutpost.Expect(it => it.Query()).Return(new Outpost[] { }.AsQueryable());

            //act
            var jsonResult = objectMother.controller.Index(indexModel);

            //assert
            objectMother.queryService.VerifyAllExpectations();
            Assert.IsInstanceOf<JsonResult>(jsonResult);

            var jsonData = jsonResult.Data as DistrictIndexOutputModel;

            Assert.AreEqual(jsonData.TotalItems, 0);
            Assert.AreEqual(jsonData.districts.Count, 0);

        }
       
    }
}
