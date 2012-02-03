using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.District;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.DistrictControllerTests
{
    [TestFixture]
    public class IndexMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Return_TheData_Paginated_SpecificToIndexModel_ValueFields_When_Just_RegionIdHasValue()
        {
            var indexModel = new DistrictIndexModel() {
                Limit = 50,
                dir = "DESC",
                sort = "Name",
                CountryId = null,
                RegionId = objectMother.region.Id,
                Start = 0,
                Page = 1
            };

            objectMother.ExpectQueryDistrictSpecificToRegionIdAndClientIdFromModel(indexModel);




        }
    }
}
