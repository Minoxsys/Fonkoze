using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    [TestFixture]
    public class GetRightGridStoreMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JsonData_With_Countries_That_Have_The_Selected_Ids()
        {
            //Arrange
            objectMother.queryCountries.Expect(call => call.Query()).Return(objectMother.CurrentUserCountries());
            var ids = objectMother.countryId1 + "," + objectMother.countryId2;

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetCountriesForIds", ids);

            //Assert
            objectMother.queryCountries.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Countries_If_NO_CountryId_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetCountriesForIds", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_Regions_That_Have_The_Selected_Ids()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(objectMother.CurrentUserRegions());

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetRegionsForIds", objectMother.regionId.ToString());

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Regions_If_NO_Id_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetRegionsForIds", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_Districts_That_Have_The_Selected_Ids()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.CurrentUserDistricts());

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetDistrictsForIds", objectMother.districtId.ToString());

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Districts_If_NO_Id_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetDistrictsForIds", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_Outposts_That_Have_The_Selected_Ids()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetOutpostsForIds", objectMother.outpostId.ToString());

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Outposts_If_NO_Id_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetRightGridStore("GetOutpostsForIds", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }
    }
}
