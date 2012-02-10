using System;
using System.Linq;
using NUnit.Framework;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	[TestFixture]
	public class GetOutpostsMethod
	{
		public readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void Returns_The_JsonResult_With_TheCorrectPageOfData()
		{
			var inputModel = _.ExpectOutpostsToBeQueriedWithInputModel();
			inputModel.districtId = null;
			var viewResult = _.controller.GetOutposts(inputModel) as JsonResult;

			_.VerifyThatOutpostsHaveBeenQueried();
			Assert.IsNotNull(viewResult);
		}

		[Test]
		public void Queries_ForUser_And_Its_Client()
		{
			var inputModel = _.ExpectOutpostsToBeQueriedWithInputModel();
			inputModel.districtId = null;
			var viewResult = _.controller.GetOutposts(inputModel) as JsonResult;

			_.VerifyUserAndClientExpectations();
		}

		[Test]
		public void Query_Only_Returns_A_Subset_OfThe_Data()
		{
			var inputModel = _.ExpectOutpostsToBeQueriedWithInputModel();
			inputModel.districtId = null;
			inputModel.limit = 35;
			var viewResult = _.controller.GetOutposts(inputModel) as JsonResult;
			var data = viewResult.Data as GetOutpostsOutputModel;

			Assert.IsNotNull(data);
			Assert.That(data.TotalItems, Is.GreaterThan(data.Outposts.Length));
			Assert.That(data.Outposts.Length, Is.EqualTo(inputModel.limit));

		}

		[Test]
		public void Query_Includes_Only_Outposts_ThatMatch_SearchName()
		{
			var inputModel = _.ExepectOutpostsToBeQueriedByName("Den");

			inputModel.limit = 35;

			var viewResult = _.controller.GetOutposts(inputModel);
			var data = viewResult.Data as GetOutpostsOutputModel;

			for (int i = 0; i < data.Outposts.Count(); i++)
			{
				string outpostName = data.Outposts[i].Name;
				Assert.That(outpostName, Is.StringContaining("Den"));
			}
		}
	}
}