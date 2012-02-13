using System;
using System.Linq;
using NUnit.Framework;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	[TestFixture]
	public class GetDistrictsMethod
	{
		public readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
			_.StubUserAndItsClient();
		}

		[TearDown]
		public void AfterEach()
		{
			_.VerifyUserAndClientExpectations();
		}

		[Test]
		public void RequiresALoggedInUserAndAClient()
		{
			var viewResult = _.controller.GetDistricts(Guid.Empty) as JsonResult;

			Assert.IsNotNull(viewResult);
		}

		[Test]
		public void NeverReturnsNull()
		{
			var viewResult = _.controller.GetDistricts(Guid.Empty) as JsonResult;

			Assert.IsNotNull(viewResult.Data);
			Assert.IsInstanceOf<GetDistrictsOutputModel>(viewResult.Data);
		}

		[Test]
		public void ReturnsEmptyArray_WhenEmptyRegionId_IsSupplied()
		{
			var viewResult = _.controller.GetDistricts(Guid.Empty) as JsonResult;
			var model = viewResult.Data as GetDistrictsOutputModel;

			Assert.IsEmpty(model.Districts);
		}

		[Test]
		public void ReturnsEmptyArray_WhenNullRegionId_IsSupplied()
		{
			var viewResult = _.controller.GetDistricts(null) as JsonResult;
			var model = viewResult.Data as GetDistrictsOutputModel;

			Assert.IsEmpty(model.Districts);
		}

		[Test]
		public void ReturnsDistrictsArray_WhenValidRegionId_IsSupplied()
		{
			var returnedDistricts = _.ExpectDistrictsToBeQueriedForRegionId();

			var viewResult = _.controller.GetDistricts(_.regionId) as JsonResult;
			var model = viewResult.Data as GetDistrictsOutputModel;

			_.VerifyThatDistrictsHaveBeenQueried();
			Assert.AreEqual( returnedDistricts.Count(),  model.Districts.Count());
			Assert.AreEqual(returnedDistricts[0].Id, model.Districts[0].Id);
			Assert.AreEqual(returnedDistricts[0].Name, model.Districts[0].Name);
		}
	}
}