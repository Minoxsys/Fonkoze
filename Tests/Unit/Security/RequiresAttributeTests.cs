using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Security;
using System.Web.Mvc;

using System.Web;
using Core.Security;
using MvcContrib.TestHelper.Fakes;
using Web.Bootstrap.Container;
using Autofac;
using AutofacContrib.Moq;
using Moq;

namespace Tests.Unit.Security
{
    [TestFixture]
    public class RequiresAttributeTests
    {
        const string DOCUMENT_UPLOADER = "DocumentUploader";

        [Test]
        public void OnAuthorization_FailsWhen_UserSignedIn_And_DoesNotHaveFunction_Assigned()
        {
            // arrange
            var authContext = FakeAuthorizationContext_UserSignedIn_DOCUMENTUPLOADER_NotAssigned();

            var requiresAttribute = new RequiresAttribute();

            requiresAttribute.Permissions = DOCUMENT_UPLOADER;// simulating [Requires(FunctionRights= "DocumentUploader")] ActionResult OnSomeMethod(){ }

            // act : NOTE that Asp.NET MVC will automatically call this employee
            requiresAttribute.OnAuthorization(authContext);

            // assert
            Assert.IsInstanceOf<RedirectToRouteResult>(authContext.Result);

        }

        [Test]
        public void OnAuthorization_FailsWhen_UserNotSignedIn()
        {

            var authContext = FakeAuthorizationContext_UserNotSignedIn();

            var requiresAttribute = new RequiresAttribute();

            requiresAttribute.Permissions = DOCUMENT_UPLOADER;// simulating [Requires(FunctionRights= "DocumentUploader")] ActionResult OnSomeMethod(){ }

            // act
            requiresAttribute.OnAuthorization(authContext);

            // assert
            Assert.IsInstanceOf<HttpUnauthorizedResult>(authContext.Result);

        }
        [Test]
        public void OnAuthorization_Works_When_UserSignedIn_And_HasFunctionRight()
        {
            var authContext = FakeAuthorizationContext_UserSignedIn_And_DOCUMENTUPLOADER_FunctionRightAssigned();

            var requiresAttribute = new RequiresAttribute();

            requiresAttribute.Permissions = DOCUMENT_UPLOADER;// simulating [Requires(FunctionRights= "DocumentUploader")] ActionResult OnSomeMethod(){ }
            // act
            requiresAttribute.OnAuthorization(authContext);

            // assert
            Assert.IsNull(authContext.Result);

        }

        private static AuthorizationContext FakeAuthorizationContext_UserSignedIn_And_DOCUMENTUPLOADER_FunctionRightAssigned()
        {
            return FakeAuthorizationContext(
                (httpContext, functionRightsService) =>
                {
                    var principal = new FakePrincipal(new FakeIdentity("mihai.lazar"), new string[] { });
                    httpContext.SetupGet(h=>h.User).Returns(principal);

                    functionRightsService.Setup(c => c.HasPermissionAssigned(
                        It.Is<string>(v => v == DOCUMENT_UPLOADER),
                        principal.Identity.Name)).Returns(true);
                }
                );
        }

        private static AuthorizationContext FakeAuthorizationContext_UserNotSignedIn()
        {
            Action<Mock<HttpContextBase>, Mock<IPermissionsService>> assign_unsigned_user = (httpContext, functionRightsService) =>
                {
                    var principal = new FakePrincipal(new FakeIdentity(null), new string[] { });
                    httpContext.SetupGet(h=>h.User).Returns(principal);
                };

            return FakeAuthorizationContext(
                assign_unsigned_user
                                );
        }

        private static AuthorizationContext FakeAuthorizationContext_UserSignedIn_DOCUMENTUPLOADER_NotAssigned()
        {

            Action<Mock<HttpContextBase>, Mock<IPermissionsService>> assign_a_signed_in_user = (httpContext, functionRightsService) =>
                {

                    var principal = new FakePrincipal(new FakeIdentity("mihai.lazar"), new string[] { });
                    httpContext.SetupGet(h=>h.User).Returns(principal);

                };


            return FakeAuthorizationContext(
                    assign_a_signed_in_user
                );
        }

        private static AuthorizationContext FakeAuthorizationContext(Action<Mock<HttpContextBase>, Mock<IPermissionsService>> establishWorkingParameters)
        {
            var container = AutoMock.GetLoose();


            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupGet(h => h.ApplicationInstance).Returns(new StubHttpApplication(container.Container));
            var permissionsService = new Mock<IPermissionsService>();


            permissionsService.Setup(c => c.HasPermissionAssigned(
                It.Is<string>(v => v == DOCUMENT_UPLOADER),
                httpContext.Object.User.Identity.Name)).Returns(false);


            establishWorkingParameters(httpContext, permissionsService);



            var authContext = new Mock<AuthorizationContext>();

            container.Provide(authContext.Object);
            container.Provide(httpContext.Object);
            container.Provide(permissionsService.Object);

            authContext.SetupGet(auth => auth.HttpContext).Returns(httpContext.Object);
            return authContext.Object;
        }

        /// <summary>
        /// This class is only used as a stub for the real application.
        /// It is used by the HttpContext as the application instance
        /// </summary>
        public class StubHttpApplication : System.Web.HttpApplication, IContainerAccessor
        {
            public StubHttpApplication(IContainer container)
            {
                Container = container;
            }

            public virtual IContainer Container
            {
                get;
                set;
            }
        }

    }
}
