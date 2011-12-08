using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Controllers;
using Core.Persistence;
using Core.Domain;
using Web.Models.UserManager;
using Core.Services;
using Web.Security;

namespace UnitTests.Controllers
{
	[TestFixture]
	public class UserManagerTests
	{
        UserManagerController controller;

        private const string DEFAULT_VIEW_NAME = "";
        private const string PASSWORD = "password";
        private const string EMAIL = "email@evozon.com";
        private const string CONFIRMED_PASSWORD_MATCH = "password";
        private const string CONFIRMED_PASSWORD_DONOT_MATCH = "something";
        private Guid CLIEND_ID = Guid.Empty;
        private Guid USER_ID = Guid.NewGuid();
        private const string USERNAME = " alin.mistea";

        private ISaveOrUpdateCommand<User> _saveOrUpdateUser;
        private IDeleteCommand<User> _deleteUser;
        private IQueryService<User> _queryUser;
        private ISecurePassword _securePassword;

        [SetUp]
        public void BeforeAll()
        {
            controller = new UserManagerController();

            _saveOrUpdateUser = MockRepository.GenerateMock<ISaveOrUpdateCommand<User>>();
            _deleteUser = MockRepository.GenerateMock<IDeleteCommand<User>>();
            _queryUser = MockRepository.GenerateMock<IQueryService<User>>();
            _securePassword = new SecurePassword();

            controller.SaveOrUpdate = _saveOrUpdateUser;
            controller.QueryUsers = _queryUser;
            controller.DeleteUser = _deleteUser;
            controller.SecurePassword = _securePassword;

        }
		[Test]
		public void GET_List_BringsAllUsers()
		{
			//var empMan = new UserManagerController();

			var queryUsers = MockRepository.GenerateMock<IQueryService<User>>();

			queryUsers.Expect(c => c.Query()).Return(new User[] { }.AsQueryable());

			var listModel = MockRepository.GenerateMock<UserManagerListModel>(queryUsers);

			controller.ListModel = listModel;

			ViewResult result = controller.List();

			Assert.IsNotNull(result.ViewData.Model);

			Assert.AreEqual(listModel, result.ViewData.Model);

		}

		[Test]
		public void GET_Create_Returns_CreatePage_WithAllClientsLoaded()
		{
			var empMan = new UserManagerController();

            var queryClients = MockRepository.GenerateMock<IQueryService<Client>>();

            queryClients.Expect(c => c.Query()).Return(new Client[] { }.AsQueryable());

			empMan.CreateModel = new UserManagerCreateModel(queryClients);

			ViewResult result = empMan.Create();
            queryClients.VerifyAllExpectations();

			Assert.IsNotNull(result);

			Assert.AreEqual(empMan.CreateModel, result.ViewData.Model);
		}

        [Test]
        public void POST_Create_OnSuccess()
        {
            var model = new UserManagerCreateInputModel();
            SetAllDataForModel(model);

            _saveOrUpdateUser.Expect(call => call.Execute(Arg<User>.Matches(c => c.Id==USER_ID && c.Password == _securePassword.EncryptPassword(PASSWORD) && c.Email == EMAIL && c.ClientId == CLIEND_ID)));

            var result = (RedirectToRouteResult)controller.Create(model);

            _saveOrUpdateUser.VerifyAllExpectations();
            Assert.AreEqual(result.RouteValues["action"], "List");

        }

        [Test]
        public void POST_Create_OnFail_BecauseOf_ModelStateInvalid()
        {
            controller.ModelState.AddModelError("Password", "Required");

            var result = (ViewResult)controller.Create(new UserManagerCreateInputModel());

            Assert.AreEqual(result.ViewName, DEFAULT_VIEW_NAME);
        }

        [Test]
        public void POST_Create_OnFail_BecauseOf_PasswordAndConfirmedPasswordDoNotMatch()
        {
            var model = new UserManagerCreateInputModel();
            SetAllDataForModel_With_PasswordAndConfirmedPasswordDoNotMatch(model);

            var result = (ViewResult)controller.Create(model);

            Assert.AreNotEqual(model.ConfirmedPassword, model.Employee.Password);
            Assert.AreEqual(result.ViewName, DEFAULT_VIEW_NAME);
        }

        [Test]
        public void DELETE_Successfully_AUser()
        {
            User user = new User();
            user = StubUser(user);

            _queryUser.Expect(call => call.Load(USER_ID)).Return(user);
            _deleteUser.Expect(call => call.Execute(Arg<User>.Matches(c => c.Id == USER_ID)));

            controller.Delete(USER_ID);

            _queryUser.VerifyAllExpectations();
            _deleteUser.VerifyAllExpectations();
            
        }

        private User StubUser(User user)
        {
            USER_ID = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(x => x.Id).Return(USER_ID);
            return user;
        }
        private void SetAllDataForModel_With_PasswordAndConfirmedPasswordDoNotMatch(UserManagerCreateInputModel model)
        {
            model.Employee.ClientId = CLIEND_ID;
            model.Employee.Email = EMAIL;
            model.Employee.Password = PASSWORD;
            model.Employee.Id = USER_ID;
            model.ConfirmedPassword = CONFIRMED_PASSWORD_DONOT_MATCH;
            
        }
        private void SetAllDataForModel(UserManagerCreateInputModel model)
        {
            model.Employee.ClientId = CLIEND_ID;
            model.Employee.Email = EMAIL;
            model.Employee.Password = PASSWORD;
            model.Employee.Id = USER_ID;
            model.ConfirmedPassword = CONFIRMED_PASSWORD_MATCH;
        }

       
        
		[Test]
		public void GET_Edit_Returns_EditPage()
		{
			Guid id = Guid.Parse("{0C21D6E8-01D0-4E59-8663-53856EEC7918}");

			var empMan = new UserManagerController();

			empMan.EditModel = MockRepository.GenerateMock<UserManagerEditModel>(
				MockRepository.GenerateStub<IQueryService<User>>(),
				MockRepository.GenerateStub<IQueryService<Role>>()
				);

			empMan.EditModel.Expect(call => call.Load(id));

			var result = empMan.Edit(id);

			empMan.EditModel.VerifyAllExpectations();

			Assert.IsNotNull(result);

			Assert.IsInstanceOf<ViewResult>(result);

			var vr = result as ViewResult;

			Assert.IsNotNull(vr.ViewData.Model);
			Assert.AreEqual(empMan.EditModel, vr.ViewData.Model);

		}

		[Test]
		public void POST_Assign_ReturnsEmptyResult()
		{

			Guid UserId = Guid.Parse("{0C21D6E8-01D0-4E59-8663-53856EEC7918}");
			Guid roleId = Guid.Parse("{0C21D6E8-01D0-4E59-8663-53856EEC7917}");
			
			controller.AssignModel = MockRepository.GenerateMock<UserManagerAssignModel>(

				MockRepository.GenerateStub<IQueryService<Role>>(),
				MockRepository.GenerateStub<IQueryService<User>>(),
				MockRepository.GenerateStub<ISaveOrUpdateCommand<User>>()

				);

			controller.AssignModel.Expect(call => call.LinkUserToRole(UserId, roleId));

			var result = controller.Assign(UserId, roleId);

			controller.AssignModel.VerifyAllExpectations();

			Assert.IsNotNull(result);
		}

		[Test]
		public void POST_UnAssign_ReturnsEmptyResult()
		{

			Guid UserId = Guid.Parse("{0C21D6E8-01D0-4E59-8663-53856EEC7918}");
			Guid roleId = Guid.Parse("{0C21D6E8-01D0-4E59-8663-53856EEC7917}");
			
			controller.UnAssignModel = MockRepository.GenerateMock<UserManagerUnAssignModel>(
				MockRepository.GenerateStub<IQueryService<Role>>(),
				MockRepository.GenerateStub<IQueryService<User>>(),
				MockRepository.GenerateStub<ISaveOrUpdateCommand<User>>());

			controller.UnAssignModel.Expect(call => call.RemoveRole(UserId, roleId));

			EmptyResult result = controller.UnAssign(UserId, roleId);

			controller.UnAssignModel.VerifyAllExpectations();

			Assert.IsNotNull(result);

		}
	}
}
