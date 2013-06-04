using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    [TestFixture]
    public class GetLeftGridStoreMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JsonData_With_AllCountries_ForUser()
        {
            //Arrange
            objectMother.queryCountries.Expect(call => call.Query()).Return(objectMother.CurrentUserCountries());

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetCountries", "");

            //Assert
            objectMother.queryCountries.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_Regions_WithSelected_CountriesIds()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(objectMother.CurrentUserRegions());

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetRegions", objectMother.countryId1.ToString());

            //Assert
            objectMother.queryCountries.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Regions_If_NO_CountryId_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetRegions", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_Districts_WithSelected_RegionsIds()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(objectMother.CurrentUserDistricts());

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetDistricts", objectMother.regionId.ToString());

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Districts_If_NO_RegionId_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetDistricts", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_Outposts_WithSelected_DistrictsIds()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetOutposts", objectMother.districtId.ToString());

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JsonData_With_NO_Outposts_If_NO_DistrictId_IsProvided()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.GetLeftGridStore("GetOutposts", "");

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }

    }
}
