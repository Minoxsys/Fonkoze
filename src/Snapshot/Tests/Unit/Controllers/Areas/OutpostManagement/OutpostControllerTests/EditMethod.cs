using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	[TestFixture]
	public class EditMethod
	{
		private readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void Updates_TheEntity_AtEntityId()
		{
			var entityId = Guid.NewGuid();

			_.EnsureThatQueryLoadReturnsAValidDomainEntity(entityId);

			var jsonResult = _.controller.Edit(new EditOutpostInputModel
			{
				EntityId = entityId,
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				IsWarehouse = false,
				Name = "New Outpost Name"
			});

			_.VerifyThatLoadWasInvokedWithTheGiven(entityId);
		}

		[Test]
		public void Updates_TheEntity_AtEntityId_By_ExecutingSaveOrUpdateCommand()
		{
			var entityId = Guid.NewGuid();

			_.EnsureThatQueryLoadReturnsAValidDomainEntity(entityId);

			var jsonResult = _.controller.Edit(new EditOutpostInputModel
			{
				EntityId = entityId,
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				IsWarehouse = false,
				Name = "New Outpost Name"
			});

			_.VerifyThatSaveHasBeendCalled();
		}
	}
}
