using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Models.HistoricalProductLevel;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.HistoricalProductLevelControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_HistoricalStockLevel_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new HistoricalInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a historicalId in order to edit the historical stock level."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Historical_Has_Been_Saved()
        {
            //Arrange
            HistoricalInputModel historicalInputModel = new HistoricalInputModel()
            {
                Id = objectMother.historical.Id,
                StockLevel = 3
                
            };
            objectMother.saveCommand.Expect(call => call.Execute(Arg<OutpostHistoricalStockLevel>.Matches(p => p.Id == objectMother.historical.Id && p.StockLevel == historicalInputModel.StockLevel)));
            objectMother.queryHistorical.Expect(call => call.Load(objectMother.historical.Id)).Return(objectMother.historical);

            //Act
            var jsonResult = objectMother.controller.Edit(historicalInputModel);

            //Assert
            objectMother.saveCommand.VerifyAllExpectations();
            objectMother.queryHistorical.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("The Historical stock level has been saved."));
        }
    }
}
