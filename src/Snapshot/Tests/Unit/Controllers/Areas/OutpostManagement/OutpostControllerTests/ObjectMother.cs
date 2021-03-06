﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutofacContrib.Moq;
using Core.Domain;
using Domain;
using Moq;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.OutpostManagement.Controllers;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Services;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
    public class ObjectMother
    {
        private const string DEFAULT_VIEW_NAME = "";

        private const string FAKE_USERNAME = "fake.username";

        internal OutpostController controller;

        internal AutoMock autoMock;
        private Guid clientId = Guid.NewGuid();
        private Mock<User> userMock;
        public Mock<Client> clientMock;
        public Guid regionId;
        private Mock<Region> regionMock;
        private District[] districts;

        internal Mock<IOutpostsFileParseService> _outpostsParserMock;
        internal Mock<IOutpostsUpdateService> _outpostsUpdateService;

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
            queryUser.Setup(c => c.Query()).Returns(new[] {userMock.Object}.AsQueryable());

            controller.LoadClient = loadClient.Object;
            controller.QueryUsers = queryUser.Object;
        }

        private void InitializeController()
        {
            _outpostsParserMock = new Mock<IOutpostsFileParseService>();
            _outpostsUpdateService = new Mock<IOutpostsUpdateService>();

            controller = new OutpostController(_outpostsParserMock.Object, _outpostsUpdateService.Object);

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] {});
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

            this.districts = new District[]
                {
                    new District
                        {
                            Name = "District 1",
                            Client = this.clientMock.Object,
                            Region = this.regionMock.Object
                        },

                    new District
                        {
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

        internal IndexTableInputModel ExpectOutpostsToBeQueriedWithInputModel()
        {
            var model = new IndexTableInputModel()
                {
                    dir = "ASC",
                    limit = 50,
                    page = 1,
                    sort = "Name",
                    start = 0
                };

            var queryOutposts = Mock.Get(controller.QueryService);

            queryOutposts.Setup(c => c.Query()).Returns(this.AllOutposts(false));

            return model;
        }

        internal IndexTableInputModel ExpectOutpostsToBeQueriedWithInputModelAskingOnlyForWarehouses()
        {
            var model = new IndexTableInputModel
                {
                    dir = "ASC",
                    limit = 50,
                    page = 1,
                    sort = "Name",
                    start = 0,
                };

            var queryOutposts = Mock.Get(controller.QueryService);

            queryOutposts.Setup(c => c.Query()).Returns(AllOutpostsAndWarehouses);

            return model;
        }

        private IQueryable<Outpost> AllOutposts(bool withWarehousesMixed)
        {
            var listOfOutposts = new List<Outpost>();

            var fakeCountry = new Mock<Country>();
            fakeCountry.Setup(c => c.Id).Returns(Guid.NewGuid());
            var fakeRegion = new Mock<Region>();
            fakeRegion.Setup(c => c.Id).Returns(Guid.NewGuid());
            var fakeDistrict = new Mock<District>();
            fakeDistrict.Setup(c => c.Id).Returns(Guid.NewGuid());

            for (int i = 0; i < 500; i++)
            {

                var outpost = new Mock<Outpost>();
                outpost.Setup(c => c.Id).Returns(Guid.NewGuid());

                if (i%9 != 0)
                {
                    outpost.Setup(c => c.Name).Returns("Denhaag Outpost " + i);
                }
                else
                {
                    if (i%8 != 0)
                    {
                        outpost.Setup(c => c.Name).Returns("Gama Outpost Denim " + i);
                    }
                    else
                        outpost.Setup(c => c.Name).Returns("Beta Outpost " + i);
                }
                if (withWarehousesMixed)
                {
                    outpost.Setup(c => c.IsWarehouse).Returns(i%2 == 0);
                }
                else
                {
                    outpost.Setup(c => c.IsWarehouse).Returns(true);
                }

                outpost.Setup(c => c.Client).Returns(clientMock.Object);

                outpost.Setup(c => c.Country).Returns(fakeCountry.Object);
                outpost.Setup(c => c.Region).Returns(fakeRegion.Object);
                outpost.Setup(c => c.District).Returns(fakeDistrict.Object);

                listOfOutposts.Add(outpost.Object);
            }
            return listOfOutposts.AsQueryable();
        }

        private IQueryable<Outpost> AllOutpostsAndWarehouses()
        {
            return AllOutposts(true);
        }

        internal void VerifyThatOutpostsHaveBeenQueried()
        {
            var queryService = Mock.Get(controller.QueryService);
            queryService.Verify(c => c.Query());
        }

        internal void ExpectWarehousesToBeQueried()
        {
            var queryService = Mock.Get(controller.QueryService);
            queryService.Setup(q => q.Query()).Returns(this.Warehouses());
        }

        private IQueryable<Outpost> Warehouses()
        {
            var listOfWarehouses = new List<Outpost>();

            for (int i = 0; i < 10; i++)
            {
                var warehouse = new Mock<Outpost>();
                warehouse.SetupGet(c => c.Id).Returns(Guid.NewGuid());
                warehouse.SetupGet(c => c.IsWarehouse).Returns(true);
                warehouse.SetupGet(c => c.Name).Returns("Warehouse " + i);
                listOfWarehouses.Add(warehouse.Object);
            }

            return listOfWarehouses.AsQueryable();
        }

        internal void VerifyThatWarehousesHaveBeenQueried()
        {
            var queryService = Mock.Get(controller.QueryService);
            queryService.Verify(q => q.Query());
        }

        internal void ExpectSaveToBeCalledWithValuesFrom(CreateOutpostInputModel model)
        {
            var saveCommand = Mock.Get(controller.SaveOrUpdateCommand);
            saveCommand.Setup(c => c.Execute(Moq.It.Is<Outpost>(
                o => o.Name == model.Name && o.IsWarehouse == model.IsWarehouse)));
        }

        internal void VerifyThatSaveHasBeendCalled()
        {
            var saveCommand = Mock.Get(controller.SaveOrUpdateCommand);
            saveCommand.Verify(c => c.Execute(Moq.It.IsAny<Outpost>()));
        }

        internal void VerifyThatDelectCommandWasExecutedWith(Guid outpostId)
        {
            var deleteCommand = Mock.Get(controller.DeleteCommand);
            deleteCommand.Verify(c => c.Execute(It.Is<Outpost>(o => o.Id == outpostId)));
        }

        internal void EnsureThatQueryLoadReturnsAValidDomainEntity(Guid outpostId)
        {
            var queryService = Mock.Get(controller.QueryService);
            var outpost = new Mock<Outpost>();
            outpost.Setup(c => c.Id).Returns(outpostId);
            outpost.Setup(c => c.Name).Returns("I am an outpost");

            queryService.Setup(c => c.Load(It.Is<Guid>(g => g == outpostId))).Returns(outpost.Object);
        }

        internal void VerifyThatLoadWasInvokedWithTheGiven(Guid outpostId)
        {
            var queryService = Mock.Get(controller.QueryService);
            queryService.Verify(c => c.Load(It.Is<Guid>(g => g == outpostId)));
        }


        internal IndexTableInputModel ExepectOutpostsToBeQueriedByName(string outpostName)
        {
            var model = new IndexTableInputModel()
                {
                    dir = "ASC",
                    limit = 50,
                    page = 1,
                    sort = "Name",
                    start = 0,
                    searchValue = outpostName
                };

            var queryOutposts = Mock.Get(controller.QueryService);

            queryOutposts.Setup(c => c.Query()).Returns(this.AllOutposts(false));

            return model;
        }
    }
}