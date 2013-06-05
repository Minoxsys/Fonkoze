using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Areas.AnalysisManagement.Models.LocationReport;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.LocationReportControllerTests
{
    [TestFixture]
    public class GetOutpostMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Return_JSON_WithAListOf_OutpostMarkers()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(new FilterModel());

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<MarkerModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<MarkerModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(10, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_OutpostMarkers_FilteredByCountry()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };

            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<MarkerModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<MarkerModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_OutpostMarkers_FilteredByCountry_Region()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId };

            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<MarkerModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<MarkerModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_OutpostMarkers_FilteredByCountry_Region_District()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId, districtId = objectMother.districtId };

            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<MarkerModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<MarkerModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
