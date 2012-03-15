using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Services;
using Moq;
using System.Web;
using Web.Services;

namespace Tests.Unit.Controllers.JavaScriptControllerTests
{
    public class ObjectMother
    {
        IJavaScriptProviderService faKeJavaScriptProviderService;
        public IPathService fakePathService;
        public IETagService fakeETagService;

        public JavascriptController controller;

        public const string JS_FOLDER = "~/Assets/js/";
        public const string ABSOLUTEPATH = "D:\\Minoxsys\\Assets\\js";
        public const string UNEXISTING_PATH = "/Eu/js/";
        public const string ETAG = "ja63eugdw6tdyuw35qvsuq6rwfv36rfudv36fs2qi1svg";
        public const string SITE = "site";

        public void Init()
        {
            MockServices();
            Setup_Controller();
        }

        private void Setup_Controller()
        {
            controller = new JavascriptController(faKeJavaScriptProviderService);
            FakeControllerContext.Initialize(controller);

            Mock.Get(controller.HttpContext).Setup(it => it.Server.MapPath(JS_FOLDER)).Returns(ABSOLUTEPATH);
            Mock.Get(controller.Response).SetupAllProperties();

            controller.PathService = fakePathService;
            controller.ETagService = fakeETagService;
        }

        private void MockServices()
        {
            faKeJavaScriptProviderService = Mock.Of<IJavaScriptProviderService>();
            fakePathService = Mock.Of<IPathService>();
            fakeETagService = Mock.Of<IETagService>();
        }

        public void SetUpStubs_For_DirectoryDoesNotExist()
        {
            var existsDirectory = Mock.Get(fakePathService);
            existsDirectory.Setup(c => c.Exists(UNEXISTING_PATH)).Returns(false);
        }

        public void SetUpStubs_For_DirectoryExists_And_EtagIdentification()
        {
            var existsDirectory = Mock.Get(fakePathService);
            var getEtag = Mock.Get(fakeETagService);
            var request = Mock.Get(controller.Request);

            existsDirectory.Setup(c => c.Exists(ABSOLUTEPATH)).Returns(true);
            getEtag.Setup(c => c.Generate(ABSOLUTEPATH)).Returns(ETAG);
            request.SetupGet(c => c.Headers).Returns(new System.Net.WebHeaderCollection { { "If-None-Match", ETAG } });
        }

        public void SetUpStubs_For_DirectoryExists_UnmodifiedContent_And_EtagDontMatch()
        {
            var existsDirectory = Mock.Get(fakePathService);
            var request = Mock.Get(controller.Request);
            var lastModified = Mock.Get(fakePathService);

            existsDirectory.Setup(c => c.Exists(ABSOLUTEPATH)).Returns(true);
            request.SetupGet(c => c.Headers).Returns(new System.Net.WebHeaderCollection { { "If-Modified-Since", DateTime.UtcNow.ToString() } });
            lastModified.Setup(c => c.GetLastWriteTime(ABSOLUTEPATH)).Returns(DateTime.UtcNow.AddDays(-2));
        }

        public void SetUpStubs_For_ReturningContentResult()
        {
            var getJsService = Mock.Get(faKeJavaScriptProviderService);
            var existsDirectory = Mock.Get(fakePathService);
            var request = Mock.Get(controller.Request);
            var response = Mock.Get(controller.Response);
            var lastModified = Mock.Get(fakePathService);
            var responseCache = new Mock<HttpCachePolicyBase>();

            getJsService.Setup(c => c.GetScript(SITE)).Returns("abc");
            existsDirectory.Setup(c => c.Exists(ABSOLUTEPATH)).Returns(true);
            request.SetupGet(c => c.Headers).Returns(new System.Net.WebHeaderCollection { { "If-Modified-Since", DateTime.UtcNow.AddDays(-2).ToString() } });
            lastModified.Setup(c => c.GetLastWriteTime(ABSOLUTEPATH)).Returns(DateTime.UtcNow);
            response.SetupGet(c => c.Cache).Returns(responseCache.Object);
        }
    }
}
