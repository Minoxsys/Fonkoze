using System;
using System.Linq;
using NUnit.Framework;
using System.Web.Mvc;

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
		public void Returns_The_JsonResul()
		{
			var viewResult = _.controller.GetOutposts(Guid.Empty) as JsonResult;

			Assert.IsNotNull(viewResult);
		}
	}
}