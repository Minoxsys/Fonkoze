using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController;
using NUnit.Framework;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Rhino.Mocks;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelControllerTests
{
    public class Method_Edit
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Should_Successfully_Save_OutpostStockLevel_WithNewStockLevel_And_SavePreviousEditedData_To_OutpostHistoricalStockLevel()
        {
            var outpostStockLevelInputModel = new OutpostStockLevelInputModel() {
                Id = objectMother.outpostStockLevels[0].Id,
                StockLevel = 30
            };

            objectMother.saveOrUpdateOutpostStockLevelHistorical.Expect(it => it.Execute(Arg<OutpostHistoricalStockLevel>
                .Matches(at => at.StockLevel == objectMother.outpostStockLevels[0].StockLevel
                 && at.PrevStockLevel == objectMother.outpostStockLevels[0].PrevStockLevel)));

            objectMother.queryOutpostStockLevel.Expect(it => it.Load(objectMother.outpostStockLevels[0].Id)).Return(objectMother.outpostStockLevels[0]);
            objectMother.saveOrUpdateOutpostStockLevel.Expect(it => it.Execute(Arg<OutpostStockLevel>
                .Matches(st => st.Id == outpostStockLevelInputModel.Id
                      && st.StockLevel == outpostStockLevelInputModel.StockLevel
                     )));


            var result = objectMother.controller.Edit(outpostStockLevelInputModel);


            objectMother.saveOrUpdateOutpostStockLevel.VerifyAllExpectations();
            
            Assert.IsInstanceOf<JsonActionResponse>(result.Data);
            var response = (JsonActionResponse)result.Data;
            Assert.AreEqual(response.Status, "Success");


        }
    }
}
