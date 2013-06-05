using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.HistoricalProductLevel;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.StockAdministration.HistoricalProductLevelControllerTests
{
    [TestFixture]
    public class GetHistoricalProductLevelMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_All_HistoricalOutposts_When_OutpostId_IsNullOrDefault()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());
            objectMother.queryHistorical.Expect(call => call.Query()).Return(objectMother.GetHistoricalList());
            objectMother.queryProductGroups.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);

            //Act
            var jsonResult = objectMother.controller.GetHistoricalProductLevel(objectMother.countryId, objectMother.regionId, objectMother.districtId, null);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.queryHistorical.VerifyAllExpectations();
            objectMother.queryProductGroups.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<OutpostGridModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<OutpostGridModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
            Assert.AreEqual(2, jsonData.Items[0].NumberOfProducts);
            Assert.AreEqual(1, jsonData.Items[1].NumberOfProducts);
        }

        [Test]
        public void Returns_JSON_With_Historicals_For_OutpostId()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());
            objectMother.queryHistorical.Expect(call => call.Query()).Return(objectMother.GetHistoricalList());
            objectMother.queryProductGroups.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);

            //Act
            var jsonResult = objectMother.controller.GetHistoricalProductLevel(objectMother.countryId, objectMother.regionId, objectMother.districtId, objectMother.outpostId1);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.queryHistorical.VerifyAllExpectations();
            objectMother.queryProductGroups.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<OutpostGridModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<OutpostGridModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual(2, jsonData.Items[0].NumberOfProducts);
        }

        [Test]
        public void Returns_JSON_With_Zero_Historical_When_There_Are_No_Historical()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());
            objectMother.queryHistorical.Expect(call => call.Query()).Return(new OutpostHistoricalStockLevel[] { }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetHistoricalProductLevel(objectMother.countryId, objectMother.regionId, objectMother.districtId, objectMother.outpostId1);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.queryHistorical.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<OutpostGridModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<OutpostGridModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }


    }
}
