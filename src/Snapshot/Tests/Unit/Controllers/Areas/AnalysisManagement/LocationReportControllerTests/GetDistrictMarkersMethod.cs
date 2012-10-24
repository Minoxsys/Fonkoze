using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Areas.AnalysisManagement.Models.LocationReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.LocationReportControllerTests
{
    [TestFixture]
    public class GetDistrictMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Return_JSON_WithAListOf_DistrictMarkers()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.districtList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(new FilterModel());

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(10, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_DistrictMarkers_FilteredByCountry()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.districtList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(6, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_DistrictMarkers_FilteredByCountry_ByRegion()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.districtList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_DistrictMarkers_FilteredByCountry_ByRegion_ByDistrict()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.districtList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId, districtId = objectMother.districtId };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
