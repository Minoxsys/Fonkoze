using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.AssignProductGroupController_Tests
{
	[TestFixture]
	public class GetProducts_Method
	{
		readonly ObjectMother _ = new ObjectMother();

		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void ChecksThatUserIsLoggedIn_InOrder_To_RetriveIts_Client()
		{
			_.controller.GetProducts(_.FakeGetProductsInput());

			_.VerifyUserAndClientExpectations();
		}

		[Test]
		public void Loads_Given_Outpost()
		{
			_.controller.GetProducts(_.FakeGetProductsInput());

			_.VerifyOutpostLoaded();
		}

		[Test]
		public void Loads_Given_ProductGroup()
		{
			_.controller.GetProducts(_.FakeGetProductsInput());

			_.VerifyProductGroupLoaded();
		}

		[Test]
		public void Queries_OutpostStockLevels()
		{
			_.controller.GetProducts(_.FakeGetProductsInput());

			_.VerifyOutpostStockLevelsQueried();
		}

		[Test]
		public void Queries_Products()
		{
			_.controller.GetProducts(_.FakeGetProductsInput());

			_.VerifyProductsQueried();
		}

		[Test]
		public void Marks_Products_thatExistsInOutpostStockLevel_as_Selected()
		{
			_.FakeProductsAndOutpostStockLevels();

			var result = _.controller.GetProducts(_.FakeGetProductsInput());

			Assert.IsNotNull(result);

			Assert.IsInstanceOf<AssignProductGroupController.GetProductsOutputModel[]>(result.Data);

			var data = result.Data as  AssignProductGroupController.GetProductsOutputModel[];

			Assert.IsTrue(data[0].Selected);
			Assert.IsFalse(data[1].Selected);
		}
	}
}