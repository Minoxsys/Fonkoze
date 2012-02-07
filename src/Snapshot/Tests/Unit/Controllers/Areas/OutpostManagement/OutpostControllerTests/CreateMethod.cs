using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Outpost;
using Moq;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	[TestFixture]
	public class CreateMethod
	{
		private readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void Saves_A_New_Outpost_WithTheSuppliedValues()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.ExpectSaveToBeCalledWithValuesFrom(model);

			_.controller.Create(model);

			_.VerifyThatSaveHasBeendCalled();
		}

		[Test]
		public void Answers_With_JsonActionResponse()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.ExpectSaveToBeCalledWithValuesFrom(model);

			var jsonResult = _.controller.Create(model);
			Assert.IsInstanceOf<JsonActionResponse>(jsonResult.Data);
			var response = jsonResult.Data as JsonActionResponse;
			Assert.AreEqual("Created successfully outpost Warehouse", response.Message);

			_.VerifyThatSaveHasBeendCalled();
		}


		[Test]
		public void Loads_Country()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.controller.Create(model);

			Mock.Get(_.controller.QueryCountry).Verify(c => c.Load(model.CountryId.Value));
		}

		[Test]
		public void Loads_Region()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.controller.Create(model);

			Mock.Get(_.controller.QueryRegion).Verify(c => c.Load(model.RegionId.Value));
		}


		[Test]
		public void Loads_District()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.controller.Create(model);

			Mock.Get(_.controller.QueryDistrict).Verify(c => c.Load(model.DistrictId.Value));
		}

		[Test]
		public void Loads_Warehouse()
		{
			var model = new CreateOutpostInputModel
			{
				Coordinates = "22.23 234.30",
				Name = "Warehouse",
				CountryId = Guid.NewGuid(),
				RegionId = Guid.NewGuid(),
				DistrictId = Guid.NewGuid(),
				WarehouseId = Guid.NewGuid(),
				IsWarehouse = false
			};

			_.controller.Create(model);

			Mock.Get(_.controller.QueryService).Verify(c => c.Load(model.WarehouseId.Value));
		}

	}
}
