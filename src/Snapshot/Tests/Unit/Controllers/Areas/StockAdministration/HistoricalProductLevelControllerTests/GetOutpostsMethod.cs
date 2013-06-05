using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.StockAdministration.HistoricalProductLevelControllerTests
{
    [TestFixture]
    public class GetOutpostsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Outposts_For_User_And_DistrictId()
        {
            //Arange
            objectMother.queryOutposts.Expect(call => call.Query()).Return(objectMother.CurrentUserOutposts());

            //Act
            var jsonResult = objectMother.controller.GetOutposts(objectMother.districtId);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<GetOutpostsOutputModel.OutpostModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<GetOutpostsOutputModel.OutpostModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_EmptyList_Of_Outposts_For_User_When_DistrictId_IsNull()
        {
            //Arange

            //Act
            var jsonResult = objectMother.controller.GetOutposts(null);

            //Assert
            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<GetOutpostsOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as GetOutpostsOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(0, jsonData.TotalItems);
        }
    }
}
