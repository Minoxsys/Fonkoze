using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.CampaignManagement.Models.Campaign;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.CampaignController_Tests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Campaign_Has_Been_Saved()
        {
            //Arrange
            CampaignInputModel model = new CampaignInputModel()
            {
                CampaignName = objectMother.campaign.Name,
                StartDate = objectMother.campaign.StartDate.ToString(),
                EndDate = objectMother.campaign.EndDate.ToString(),
                CountriesIds = objectMother.country1.Id.ToString(),
                RegionsIds = objectMother.regionId.ToString(),
                DistrictsIds = objectMother.districtId.ToString(),
                OutpostsIds = objectMother.outpostId.ToString()
            };

            objectMother.saveCommand.Expect(call => call.Execute(Arg<Campaign>.Matches(p =>
                                                                            p.Name == objectMother.campaign.Name &&
                                                                            p.Opened == objectMother.campaign.Opened &&
                                                                            p.StartDate.Value.ToShortDateString() == objectMother.campaign.StartDate.Value.ToShortDateString() &&
                                                                            p.EndDate.Value.ToShortDateString() == objectMother.campaign.EndDate.Value.ToShortDateString() &&
                                                                            p.Options.Count() == objectMother.campaign.Options.Count()
                                                                 )));

            //Act
            var jsonResult = objectMother.controller.Create(model);

            //Assert
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Campaign Campania 1 has been saved."));
        }



    }
}
