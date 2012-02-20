using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.AssignProductGroupController_Tests
{
    [TestFixture]
    public class ModifyProductAssignments_Method
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
			_.controller.ModifyProductAssignments(_.FakeModifyProductAssignmentsInput());

			_.VerifyUserAndClientExpectations();
		}

		[Test]
		public void Removes_OutpostStockLevels_ForProducts_ThatAre_Not_Selected()
		{
			_.controller.ModifyProductAssignments(_.FakeModifyProductAssignmentsInput());

			_.VerifyThatNotSelectedProductsAreBeingRemoved();
		}

		[Test]
		public void Adds_OutpostStockLevels_ForProducts_ThatAre_Selected()
		{
			_.controller.ModifyProductAssignments(_.FakeModifyProductAssignmentsInput());

			_.VerifyThatSelectedProductsAreBeingAdded();
		}


    }
}
