using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Core.Domain;
using Core.Persistence;
using Core.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Controllers;
using Web.Models.Account;
using Web.Security;

namespace UnitTests.Controllers
{

	[TestFixture]
	public class AccountControllerTest
	{

		ISaveOrUpdateCommand<User> saveOrUpdateStub;
		IQueryService<User> queryServiceStub;
        IMembershipService authenticateuserService;
        ISecurePassword securePassword;

		[TestFixtureSetUp]
		public void BeforeAll()
		{
			saveOrUpdateStub = MockRepository.GenerateStub<ISaveOrUpdateCommand<User>>();

			queryServiceStub = MockRepository.GenerateStub<IQueryService<User>>();

            securePassword = new SecurePassword();
            authenticateuserService = new AuthenticateUser(queryServiceStub,securePassword);

		}

		
		[Test]
		public void LogOff_LogsOutAndRedirects()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.LogOff();

			// Assert
			Assert.IsInstanceOf<RedirectToRouteResult>(result);
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("LogOn", redirectResult.RouteValues["action"]);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignOut_WasCalled);
		}

		[Test]
		public void LogOn_Get_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.LogOn();

			// Assert
			Assert.IsInstanceOf<ViewResult>(result);
		}

		[Test]
		public void LogOn_Post_ReturnsRedirectOnSuccess_WithoutReturnUrl()
		{
			// Arrange
			AccountController controller = GetAccountController();
            controller.AuthenticateUser = authenticateuserService;
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				RememberMe = false
			};

            queryServiceStub.Expect(call => call.Query()).Repeat.Once().Return(new User[] { new User { UserName = model.UserName, Password = securePassword.EncryptPassword(model.Password) } }.AsQueryable());

			// Act
			ActionResult result = controller.LogOn(model, null);

			queryServiceStub.VerifyAllExpectations();
			// Assert
			Assert.IsInstanceOf<RedirectToRouteResult>(result);
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[Test]
		public void LogOn_Post_ReturnsRedirectOnSuccess_WithReturnUrl()
		{
			// Arrange
			AccountController controller = GetAccountController();
            controller.AuthenticateUser = authenticateuserService;
            //queryServiceStub.Expect(call => call.Query()).Repeat.Once().Return(new User[] { new User { UserName="someUser", Password="goodPassword"} }.AsQueryable());
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				RememberMe = false
			};
            queryServiceStub.Expect(call => call.Query()).Repeat.Once().Return(new User[] { new User { UserName = model.UserName, Password = securePassword.EncryptPassword(model.Password) } }.AsQueryable());
			// Act
			ActionResult result = controller.LogOn(model, "/someUrl");
			queryServiceStub.VerifyAllExpectations();

			// Assert
			Assert.IsInstanceOf<RedirectResult>(result);
			RedirectResult redirectResult = (RedirectResult)result;
			Assert.AreEqual("/someUrl", redirectResult.Url);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[Test]
		public void LogOn_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				RememberMe = false
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.LogOn(model, null);

			// Assert
			Assert.IsInstanceOf<ViewResult>(result);
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
		}

		[Test]
		public void LogOn_Post_ReturnsViewIfValidateUserFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
            controller.AuthenticateUser = authenticateuserService;
            queryServiceStub.Expect(call => call.Query()).Repeat.Once().Return(new User[] {}.AsQueryable());

			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "badPassword",
				RememberMe = false
			};

			// Act
			ActionResult result = controller.LogOn(model, null);

			// Assert
			Assert.IsInstanceOf<ViewResult>(result);
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual("The user name or password provided is incorrect.", controller.ModelState[""].Errors[0].ErrorMessage);
		}

				
		private  AccountController GetAccountController()
		{
			AccountController controller = new AccountController()
			{
				FormsService = new MockFormsAuthenticationService(),
				//MembershipService = new StubMembershipService(),
				QueryUser = queryServiceStub,
				SaveUser = saveOrUpdateStub
			};
			controller.ControllerContext = new ControllerContext()
			{
				Controller = controller,
				RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
			};
			return controller;
		}

		private class MockFormsAuthenticationService : IAuthenticationService
		{
			public bool SignIn_WasCalled;
			public bool SignOut_WasCalled;

			public void SignIn( string userName, bool createPersistentCookie )
			{
				// verify that the arguments are what we expected
				Assert.AreEqual("someUser", userName);
				Assert.IsFalse(createPersistentCookie);

				SignIn_WasCalled = true;
			}

			public void SignOut()
			{
				SignOut_WasCalled = true;
			}
		}

		private class MockHttpContext : HttpContextBase
		{
			private readonly IPrincipal _user = new GenericPrincipal(new GenericIdentity("someUser"), null /* roles */);

			public override IPrincipal User
			{
				get
				{
					return _user;
				}
				set
				{
					base.User = value;
				}
			}
		}

		private class StubMembershipService : IMembershipService
		{

			public bool ValidateUser( string userName, string password )
			{
				return (userName == "someUser" && password == "goodPassword");
			}

		}

	}
}
