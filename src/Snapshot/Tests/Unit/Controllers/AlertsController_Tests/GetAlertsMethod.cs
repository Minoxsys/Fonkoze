using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Alerts;
using Rhino.Mocks;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.AlertsController_Tests
{
    [TestFixture]
    public class GetAlertsMethod
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
                sort = "OutpostName"
            };
            var pageOfData = objectMother.PageOfAlertsData(indexModel);
            objectMother.queryAlerts.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetAlerts(indexModel);

            //Assert
            objectMother.queryAlerts.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<AlertModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<AlertModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Alerts_Order_ByOutpostName_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName"
            };
            var pageOfData = objectMother.PageOfAlertsData(indexModel);
            objectMother.queryAlerts.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetAlerts(indexModel);

            //Assert
            objectMother.queryAlerts.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<AlertModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<AlertModel>;
            Assert.IsNotNull(jsonData);

            Assert.That(jsonData.Items[0].OutpostName, Is.EqualTo("Outpost1 9"));

        }
        [Test]
        public void Returns_Alerts_WithSearchValue_Order_ByLastUpdated_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                searchValue = "9"
            };
            var pageOfData = objectMother.PageOfAlertsData(indexModel);
            objectMother.queryAlerts.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetAlerts(indexModel);

            //Assert
            objectMother.queryAlerts.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<AlertModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<AlertModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(5, jsonData.TotalItems);

        }
    }
}
