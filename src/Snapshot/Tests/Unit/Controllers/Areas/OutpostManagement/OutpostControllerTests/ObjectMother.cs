using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Controllers;
using Autofac;
using AutofacContrib.Moq;
using MvcContrib.TestHelper.Fakes;
using System.Web.Mvc;
using Moq;
using Core.Domain;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	public class ObjectMother
	{
		const string DEFAULT_VIEW_NAME = "";

		const string FAKE_USERNAME = "fake.username";

		internal OutpostController controller;

		internal AutoMock autoMock;
		private Guid clientId = Guid.NewGuid();
		private Mock<User> userMock;
		public Mock<Client> clientMock;
		public Guid regionId;
		private Mock<Region> regionMock;
		private District[] districts;

		internal void Init()
		{
			autoMock = AutoMock.GetLoose();

			InitializeController();
			StubUserAndItsClient();
		}

		internal void StubUserAndItsClient()
		{
			var loadClient = Mock.Get(this.controller.LoadClient);
			var queryUser = Mock.Get(this.controller.QueryUsers);

			this.clientMock = new Mock<Client>();
			clientMock.Setup(c => c.Id).Returns(this.clientId);
			clientMock.Setup(c => c.Name).Returns("minoxsys");

			this.userMock = new Mock<User>();
			userMock.Setup(c => c.Id).Returns(Guid.NewGuid());
			userMock.Setup(c => c.ClientId).Returns(clientMock.Object.Id);
			userMock.Setup(c => c.UserName).Returns(FAKE_USERNAME);
			userMock.Setup(c => c.Password).Returns("asdf");

			loadClient.Setup(c => c.Load(this.clientId)).Returns(clientMock.Object);
			queryUser.Setup(c => c.Query()).Returns(new[] { userMock.Object }.AsQueryable());

			controller.LoadClient = loadClient.Object;
			controller.QueryUsers = queryUser.Object;
		}

		private void InitializeController()
		{
			controller = new OutpostController();

			FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
			FakeControllerContext.Initialize(controller);
          
			autoMock.Container.InjectUnsetProperties(controller);
		}

		internal void VerifyUserAndClientExpectations()
		{
			var queryUser = Mock.Get(this.controller.QueryUsers);
			var loadClient = Mock.Get(this.controller.LoadClient);

			queryUser.Verify(call => call.Query());
			loadClient.Verify(call => call.Load(this.clientId));
		}

		internal District[] ExpectDistrictsToBeQueriedForRegionId()
		{
			StubDistrictsRegion();
			var queryDistricts = Mock.Get(this.controller.QueryDistrict);
			
			queryDistricts.Setup(c => c.Query()).Returns(
				districts.AsQueryable());

			return districts;
		}

		private void StubDistrictsRegion()
		{
			this.regionId = Guid.NewGuid();
			this.regionMock = new Mock<Region>();
			this.regionMock.SetupGet(r => r.Id).Returns(this.regionId);
			this.regionMock.SetupGet(r => r.Name).Returns("Region");

			this.regionMock.SetupGet(r => r.Client).Returns(this.clientMock.Object);

			this.districts = new District[]{
				new District{
					Name = "District 1",
					Client = this.clientMock.Object,
					Region = this.regionMock.Object
				},

				new District{
					Name = "District 2",
					Client = this.clientMock.Object,
					Region = this.regionMock.Object
				}
			};

		}

		internal void VerifyThatDistrictsHaveBeenQueried()
		{
			var queryDistricts = Mock.Get(this.controller.QueryDistrict);
			queryDistricts.Verify(c => c.Query());
		}

		internal GetOutpostsInputModel ExpectOutpostsToBeQueriedWithInputModel()
		{
			var model = new GetOutpostsInputModel()
			{
				dir = "ASC",
				districtId = null,
				limit= 50,
				page=1,
				sort = "Name",
				start= 0
			};

			var queryOutposts = Mock.Get(controller.QueryService);

			queryOutposts.Setup(c => c.Query()).Returns(this.AllOutposts());

			return model;
		}

		private  IQueryable<Outpost> AllOutposts()
		{
			var listOfOutposts = new List<Outpost>();

			for (int i = 0; i < 500; i++)
			{

				var outpost = new Mock<Outpost>();
				outpost.Setup(c => c.Id).Returns(Guid.NewGuid());
				outpost.Setup(c => c.Name).Returns("Outpost " + i);
				outpost.Setup(c => c.IsWarehouse).Returns(true);
				outpost.Setup(c => c.Client).Returns(clientMock.Object);

				listOfOutposts.Add(outpost.Object);
			}
			return listOfOutposts.AsQueryable();
		}

		internal void VerifyThatOutpostsHaveBeenQueried()
		{
			var queryService = Mock.Get(controller.QueryService);
			queryService.Verify(c => c.Query());
		}
	}
}