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
    public class GetRegionMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Return_JSON_WithAListOf_RegionMarkers()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(objectMother.regionList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetRegionMarkers(new FilterModel());

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(10, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_RegionMarkers_FilteredByCountry()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(objectMother.regionList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };

            //Act
            var jsonResult = objectMother.controller.GetRegionMarkers(model);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(6, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_RegionMarkers_FilteredByCountry_AndRegion()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(objectMother.regionList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId};

            //Act
            var jsonResult = objectMother.controller.GetRegionMarkers(model);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();
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
