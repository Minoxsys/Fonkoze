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
    public class GetCampaignsMethod
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
            var pageOfData = objectMother.PageOfCampaignData(indexModel);
            objectMother.queryCampaign.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetCampaigns(indexModel);

            //Assert
            objectMother.queryCampaign.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<CampaignOutputModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<CampaignOutputModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Campaigns_Paginated_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            var pageOfData = objectMother.PageOfCampaignData(indexModel);
            objectMother.queryCampaign.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetCampaigns(indexModel);

            //Assert
            objectMother.queryCampaign.VerifyAllExpectations();

            var jsonData = jsonResult.Data as StoreOutputModel<CampaignOutputModel>;

            Assert.That(jsonData.Items[0].Name, Is.EqualTo("Campaign9"));
        }
        [Test]
        public void Returns_Campaigns_Paginated_Order_ByEndDate_ASC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "EndDate"
            };

            var pageOfData = objectMother.PageOfCampaignData(indexModel);
            objectMother.queryCampaign.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetCampaigns(indexModel);

            //Assert
            objectMother.queryCampaign.VerifyAllExpectations();
            var jsonData = jsonResult.Data as StoreOutputModel<CampaignOutputModel>;

            Assert.That(jsonData.Items[0].EndDate, Is.EqualTo(objectMother.campaign.EndDate.Value.ToString("dd-MMM-yyyy")));
        }

    }
}
