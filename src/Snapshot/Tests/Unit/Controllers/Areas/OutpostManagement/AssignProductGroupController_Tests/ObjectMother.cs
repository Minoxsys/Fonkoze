using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.OutpostManagement.Controllers;
using AutofacContrib.Moq;
using Core.Domain;
using Domain;
using Moq;
using Autofac;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.AssignProductGroupController_Tests
{
    public class ObjectMother
    {
        internal AssignProductGroupController controller;

        internal AutoMock autoMock;

        private Guid clientId = Guid.NewGuid();

        private Mock<User> userMock;
        public Mock<Client> clientMock;

        const string FAKE_USERNAME = "fake.username";

        private readonly Guid outpostId = Guid.NewGuid();

        public void Init()
        {
            autoMock = AutoMock.GetLoose();

            InitializeController();
            StubUserAndItsClient();
        }

        private void InitializeController()
        {
            controller = new AssignProductGroupController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            autoMock.Container.InjectUnsetProperties(controller);
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

        internal void VerifyUserAndClientExpectations()
        {
            var queryUser = Mock.Get(this.controller.QueryUsers);
            var loadClient = Mock.Get(this.controller.LoadClient);

            queryUser.Verify(call => call.Query());
            loadClient.Verify(call => call.Load(this.clientId));
        }


        internal AssignProductGroupController.GetOutpostStockLevelInput FakeOutpostStockLevelInput()
        {
            return new AssignProductGroupController.GetOutpostStockLevelInput
            {
                OutpostId = outpostId

            };
        }

        internal void FakeOutpostStockLevels()
        {
            var queryOutpostStockLevel = Mock.Get(controller.QueryOutpostStockLevel);

            queryOutpostStockLevel.Setup(call => call.Query()).Returns(ListOfFakeOutpostStockLevels());
        }

        private IQueryable<OutpostStockLevel> ListOfFakeOutpostStockLevels()
        {
            var stockLevels = new List<OutpostStockLevel>();

            var outpost = new Mock<Outpost>();
            outpost.Setup(c=>c.Id).Returns(outpostId);
            outpost.Setup(c=>c.Name).Returns("My Outpost");

            var group = new Mock<ProductGroup>();
            group.Setup(c => c.Id).Returns(Guid.NewGuid());
            group.Setup(c => c.Name).Returns("My Group");

            for (int i = 0; i < 50; i++)
            {
                var product = new Mock<Product>();
                product.Setup(c => c.Id).Returns(Guid.NewGuid());
                product.Setup(c => c.Name).Returns("Product " + i);

                var osl = new Mock<OutpostStockLevel>();

                osl.Setup(c => c.Id).Returns(Guid.NewGuid());

                osl.Setup(c => c.Outpost).Returns(outpost.Object);
                osl.Setup(c => c.ProductGroup).Returns(group.Object);
                osl.Setup(c => c.Product).Returns(product.Object);

                osl.Setup(c => c.Client).Returns(clientMock.Object);

                osl.Setup(c => c.Updated).Returns(DateTime.Now);
                osl.Setup(c=>c.UpdateMethod).Returns(  ((i%2 ==0) ? "sms": "email") );


                stockLevels.Add(osl.Object);
            }

                return stockLevels.AsQueryable();
        }

        internal void VerifyQueryOnOutpostStockLevelService()
        {
            var queryOutpostStockLevel = Mock.Get(controller.QueryOutpostStockLevel);

            queryOutpostStockLevel.Verify(call => call.Query());
        }
    }
}
