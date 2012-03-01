using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    [TestFixture]
    public class DoesCampaignExistMethod
    {
        public const string NEW_CAMPAIGN_NAME = "New Campaign Name";

        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void It_Receives_A_Campaign_Name_That_Does_Not_Exist_In_Db_And_Returns_A_Not_Found_Message()
        {
            // Arrange
            objectMother.queryCampaign.Expect(call => call.Query()).Return(new Campaign[] { objectMother.campaign }.AsQueryable());

            // Act
            var result = objectMother.controller.DoesCampaignExist(NEW_CAMPAIGN_NAME);

            // Assert
            objectMother.queryCampaign.VerifyAllExpectations();

            Assert.IsNotNull(result);

            var response = result.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("NotFound"));
            Assert.That(response.Message, Is.EqualTo("Campaign " + NEW_CAMPAIGN_NAME + " was not found in the DB."));
        }

        [Test]
        public void It_Receives_A_Campaign_Name_That_Exists_In_Db_And_Returns_A_Found_Message()
        {
            // Arrange
            objectMother.queryCampaign.Expect(call => call.Query()).Return(new Campaign[] { objectMother.campaign }.AsQueryable());

            // Act
            var result = objectMother.controller.DoesCampaignExist(ObjectMother.CAMPAIGN_NAME);

            // Assert
            objectMother.queryCampaign.VerifyAllExpectations();

            Assert.IsNotNull(result);

            var response = result.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("There is a campaign with the name " + ObjectMother.CAMPAIGN_NAME + " in the DB."));
        }
    }
}
