using System;
using System.Linq;
using Web.Areas.OutpostManagement.Controllers;
using Moq;
using AutofacContrib.Moq;
using Core.Domain;
using Domain;
using MvcContrib.TestHelper.Fakes;
using Autofac;
using Web.Areas.OutpostManagement.Models.Contact;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.ContactControllerTests
{
    public class ObjectMother
    {
        const string DEFAULT_VIEW_NAME = "";

        const string FAKE_USERNAME = "fake.username";

        internal ContactController controller;

        internal AutoMock autoMock;

        private Guid clientId = Guid.NewGuid();
        private Guid outpostId = Guid.NewGuid();

        private Mock<User> userMock;
        public Mock<Client> clientMock;

        public void Init()
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
            controller = new ContactController();

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

        internal ContactController.GetContactIndex GetIndexMethodInput()
        {
            var model = new ContactController.GetContactIndex();

            model.OutpostId = outpostId; 

            return model;
        }

        internal void VerifyOutpostLoaded()
        {
            var loadOutpost = Mock.Get(this.controller.QueryOutposts);
            loadOutpost.Verify(call => call.Load(this.outpostId));
        }

        internal void VerifyContactsQueried()
        {
            var queryContacts = Mock.Get(this.controller.QueryContact);

            queryContacts.Verify(call => call.Query());
        }

        internal ContactController.PostContactIndex GetPostModel()
        {
            var model = new ContactController.PostContactIndex()
            {
                IsMainContact = true,
                OutpostId = outpostId,
                Id = Guid.Empty,
                ContactDetail = "Phone",
                ContactType = "077 055 044"
            };

            return model;
        }


        internal void StubLoadOutpost(string contactDetail=null)
        {
            var outpostFake = new Mock<Outpost>();
            outpostFake.Setup(o => o.Id).Returns(outpostId);
            outpostFake.Setup(o => o.DetailMethod).Returns(contactDetail);

            var loadOutpost = Mock.Get(this.controller.QueryOutposts);


            loadOutpost.Setup(l => l.Load(outpostId)).Returns(outpostFake.Object);
        }

        internal ContactController.PutContactIndex GetPutModel_WithEmptyId()
        {
            var model = new ContactController.PutContactIndex()
            {
                IsMainContact = true,
                OutpostId = outpostId,
                Id = Guid.Empty,
                ContactDetail = "Phone",
                ContactType = "077 055 044"
            };

            return model;
 
        }
        internal ContactController.PutContactIndex GetPutModel()
        {
            var model = new ContactController.PutContactIndex()
            {
                IsMainContact = true,
                OutpostId = outpostId,
                Id = Guid.NewGuid(),
                ContactDetail = "Phone",
                ContactType = "077 055 044"
            };

            return model;
        }

        internal void VerifySaveCommandCalledWithDataFrom(ContactModel putModel)
        {
            var saveCommand = Mock.Get(controller.SaveOrUpdateCommand);

            saveCommand.Verify(call => call.Execute(Moq.It.Is<Contact>(
                c =>
                    c.Id == putModel.Id.Value &&
                    c.IsMainContact == putModel.IsMainContact &&
                    c.Outpost.Id == putModel.OutpostId &&
                    c.ContactDetail == putModel.ContactDetail &&
                    c.ContactType == putModel.ContactType)));
        }

        internal void VerifyUpdateOutpostCommandCalledWithDataFrom(ContactModel putModel)
        {
            var updateCommand = Mock.Get(controller.UpdateOutpost);

            updateCommand.Verify(call => call.Execute(Moq.It.Is<Outpost>(
                o => o.DetailMethod == putModel.ContactDetail)));
        }

        internal void StubLoadContact(ContactModel contact)
        {
            var contactFake = new Mock<Contact>();
            contactFake.Setup(o => o.Id).Returns(contact.Id.Value);
            contactFake.Setup(c => c.Outpost).Returns(new Mock<Outpost>().Object);
            contactFake.Setup(c => c.IsMainContact).Returns(contact.IsMainContact);
            contactFake.Setup(c => c.ContactDetail).Returns(contact.ContactDetail);
            contactFake.Setup(c => c.ContactType).Returns(contact.ContactType);

            contactFake.Setup(c => c.Client).Returns(clientMock.Object);
            contactFake.Setup(c => c.ByUser).Returns(userMock.Object);

            Mock.Get(contactFake.Object.Outpost).Setup(o => o.Id).Returns(outpostId);

            var loadContact = Mock.Get(this.controller.QueryContact);

            loadContact.Setup(l => l.Load(contact.Id.Value)).Returns(contactFake.Object);
        }


        internal void VerifyDeleteCommandExecuted()
        {
            var delete = Mock.Get(controller.DeleteCommand);
            delete.Verify(c => c.Execute(It.IsAny<Contact>()));
        }

        internal ContactController.DeleteContactIndex GetDeleteModel()
        {
            var model = new ContactController.DeleteContactIndex()
            {
                IsMainContact = true,
                OutpostId = outpostId,
                Id = Guid.NewGuid(),
                ContactType = "Phone",
                ContactDetail = "077 055 044"
            };

            return model;
        }
    }
}