using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    [TestFixture]
    public class EditMethos
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Campaign_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new CampaignInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a campaignId in order to edit the campaign."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Campaign_Has_Been_Saved()
        {
            //Arrange
            CampaignInputModel inputModel = new CampaignInputModel()
            {
                Id = objectMother.campaign.Id,
                CampaignName = objectMother.campaign.Name,
                StartDate = objectMother.campaign.StartDate.ToString(),
                EndDate = objectMother.campaign.EndDate.ToString()
                
            };
            objectMother.queryCampaign.Expect(call => call.Load(objectMother.campaignId)).Return(objectMother.campaign);
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Campaign>.Matches(p =>
                                                                            p.Name == objectMother.campaign.Name &&
                                                                            p.StartDate.Value.ToShortDateString() == objectMother.campaign.StartDate.Value.ToShortDateString() &&
                                                                            p.EndDate.Value.ToShortDateString() == objectMother.campaign.EndDate.Value.ToShortDateString()
                                                                 )));
            //Act
            var jsonResult = objectMother.controller.Edit(inputModel);

            //Assert
            objectMother.queryCampaign.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Campaign Campania 1 has been saved."));
        }
    }
}
