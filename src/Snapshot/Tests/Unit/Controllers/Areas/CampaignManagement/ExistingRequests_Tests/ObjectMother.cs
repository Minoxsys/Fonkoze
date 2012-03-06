using System;
using System.Linq;
using Web.Areas.CampaignManagement.Controllers;
using AutofacContrib.Moq;
using Moq;
using Core.Domain;
using Domain;
using Autofac;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.CampaignManagement.Models.ExistingRequests;
using Web.Areas.CampaignManagement.Models.Campaign;
using System.Collections.Generic;
using System.Text;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ExistingRequests_Tests
{
    public class ObjectMother
    {


        const string FAKE_USERNAME = "fake.username";

        internal ExistingRequestsController controller;

        internal AutoMock autoMock;
        private Guid clientId = Guid.NewGuid();
        private Mock<User> userMock;
        public Mock<Client> clientMock;
        public List<Mock<Outpost>> outposts;

        internal void Init()
        {
            autoMock = AutoMock.GetLoose();

            InitializeController();
            StubUserAndItsClient();
            StubOutposts();
        }

        private void StubOutposts()
        {
            outposts = new List<Mock<Outpost>>();
            for (int i = 0; i < 12; i++)
            {
                var outpost = new Mock<Outpost>();
                outpost.SetupAllProperties();
                outpost.SetupGet(c => c.Id).Returns(Guid.NewGuid());
                outpost.SetupGet(c => c.Client).Returns(clientMock.Object);
                outpost.SetupGet(c => c.Name).Returns("Outpost " + i);

                outposts.Add(outpost);
            }
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
            controller = new ExistingRequestsController();

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

        internal GetOutpostsInput OutpostInput()
        {
            var model = new GetOutpostsInput();

            model.CampaignId = Guid.NewGuid();


            var campaignQuery = Mock.Get(controller.QueryCampaign);
            campaignQuery.Setup(call => call.Load(model.CampaignId.Value)).Returns(MockCampaign(model.CampaignId));

            var outpostsQuery = Mock.Get(controller.QueryOutposts);
            outpostsQuery.Setup(call => call.Query()).Returns(outposts.Select(o => o.Object).AsQueryable());

            return model;
        }

        private Campaign MockCampaign(Guid? campaignId)
        {
            var campaing = new Mock<Campaign>();

            campaing.Setup(call => call.Id).Returns(campaignId.Value);
            campaing.Setup(call => call.RestoreOptions<OptionsModel>()).Returns(new OptionsModel
            {
                Outposts = GetCommaSeparatedOutposts()

            });

            return campaing.Object;
        }

        private string GetCommaSeparatedOutposts()
        {
            var sb = new StringBuilder();
            outposts.ForEach(value => sb.AppendFormat("{0},", value.Object.Id));
            return sb.ToString();
        }

        internal void VerifyCampaignExpectations(Guid? campaignId)
        {
            var campaignQuery = Mock.Get(controller.QueryCampaign);

            campaignQuery.Verify(call => call.Load(campaignId.Value));
        }

        internal void VerifyOutpostQueryExpectation()
        {
            var outpostsQuery = Mock.Get(controller.QueryOutposts);
            outpostsQuery.Verify(call => call.Query());
        }

        internal GetOutpostsInput OutpostInputWithNoOutpostsInCampaign()
        {
            var model = new GetOutpostsInput();

            model.CampaignId = Guid.NewGuid();


            var campaignQuery = Mock.Get(controller.QueryCampaign);


            var campaign = Mock.Get(MockCampaign(model.CampaignId));

            campaign.Setup(call => call.RestoreOptions<OptionsModel>()).Returns((OptionsModel)null);

            campaignQuery.Setup(call => call.Load(model.CampaignId.Value)).Returns(campaign.Object);

            var outpostsQuery = Mock.Get(controller.QueryOutposts);
            outpostsQuery.Setup(call => call.Query()).Returns(outposts.Select(o => o.Object).AsQueryable());

            return model;
        }

        internal GetExistingRequestsInput ExistingRequestsInput()
        {
            var model = new GetExistingRequestsInput();

            model.limit = 50;
            model.start = 0;
            model.page = 1;

            var queryRequests = Mock.Get(controller.QueryRequests);

            queryRequests.Setup(call => call.Query()).Returns(ListOfRequests());

            return model;
        }

        private IQueryable<RequestRecord> ListOfRequests()
        {
            var reqs = new List<RequestRecord>();
            for (int i = 0; i < 100; i++)
            {
                reqs.Add(MockRequestRecord(i));

            }

            return reqs.AsQueryable();
        }

        private RequestRecord MockRequestRecord(int i)
        {
            var mock = new Mock<RequestRecord>();
            mock.SetupAllProperties();
            mock.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            mock.SetupGet(c => c.CampaignId).Returns(Guid.NewGuid());
            mock.SetupGet(c => c.OutpostId).Returns(Guid.NewGuid());
            mock.SetupGet(c => c.ProductGroupId).Returns(Guid.NewGuid());

            mock.SetupGet(c => c.CampaignName).Returns("Campaign " + i);
            mock.SetupGet(c => c.ProductGroupName).Returns("ProductGroup " + i);
            mock.SetupGet(c => c.OutpostName).Returns("Outpost " + i);

            mock.SetupGet(c => c.Client).Returns(clientMock.Object);

            return mock.Object;
        }

        internal void VerifyExistingRequestsQueried()
        {
            var queryRequests = Mock.Get(controller.QueryRequests);


            queryRequests.Verify(call => call.Query());
        }
    }
}
