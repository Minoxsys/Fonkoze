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
    public class GetCountryMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Return_JSON_WithAListOf_CountryMarkers()
        {
            //Arrange
            objectMother.queryCountry.Expect(call => call.Query()).Return(objectMother.countryList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetCountryMarkers(new FilterModel());

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryStockLevel.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(10, jsonData.TotalItems);
        }

        [Test]
        public void Return_JSON_WithAListOf_CountryMarkers_FilteredByCountry()
        {
            //Arrange
            objectMother.queryCountry.Expect(call => call.Query()).Return(objectMother.countryList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { objectMother.stockLevel }.AsQueryable());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };

            //Act
            var jsonResult = objectMother.controller.GetCountryMarkers(model);

            //Assert
            objectMother.queryCountry.VerifyAllExpectations();
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
