using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Web.Controllers;
using Core.Services;
using Web.Services.Paths;
using Web.Services.Etag;
using System.Web;

namespace Tests.Unit.Controllers.CssControllerTests
{
    public class ObjectMother
    {
        public ICssProviderService fakeCssProviderService;
        public IPathService fakePathService;
        public IETagService fakeETagService;

        public CssController controller;

        public const string RELATIVEPATH = "~/Assets/css/";
        public const string ABSOLUTEPATH = "D:\\Minoxsys\\Assets\\css";
        public const string CSS_FOLDER = "~/Assets/css/";
        public const string UNEXISTING_PATH = "/Eu/css";
        public const string ETAG = "jasgu7et4wkgbwue6tqwakgfuyf6wt";
        public const string SITE = "site";

        public void Init()
        {
            MockServices();
            Setup_Controller();
        }

        private void Setup_Controller()
        {
            controller = new CssController(fakeCssProviderService);
            FakeControllerContext.Initialize(controller);

            Mock.Get(controller.HttpContext).Setup(it => it.Server.MapPath(CSS_FOLDER)).Returns(ABSOLUTEPATH);
            Mock.Get(controller.Response).SetupAllProperties();

            controller.PathService = fakePathService;
            controller.ETagService = fakeETagService;
        }

        private void MockServices()
        {
            fakeCssProviderService = Mock.Of<ICssProviderService>();
            fakePathService = Mock.Of<IPathService>();
            fakeETagService = Mock.Of<IETagService>();

            var getCssService = Mock.Get(fakeCssProviderService);
            getCssService.Setup(c => c.GetCss(SITE)).Returns("abc");
        }

        public void SetUpStubs_For_DirectoryDoesNotExist()
        {
            var existsDirectory = Mock.Get(fakePathService);
            existsDirectory.Setup(c => c.Exists(UNEXISTING_PATH)).Returns(false);
        }

        public void SetUpStubs_For_DirectoryExists_And_IdentificationBylEtag()
        {
            var existsDirectory = Mock.Get(fakePathService);
            var getEtag = Mock.Get(fakeETagService);
            var request = Mock.Get(controller.Request);

            existsDirectory.Setup(c => c.Exists(ABSOLUTEPATH)).Returns(true);
            getEtag.Setup(c => c.Generate(ABSOLUTEPATH)).Returns(ETAG);
            request.SetupGet(c => c.Headers).Returns(new System.Net.WebHeaderCollection { { "If-None-Match", ETAG } });
        }


        public void SetUpStubs_For_DirectoryExists_IdentificationBylEtag_NotMatch_And_Unmodified()
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
            var existsDirectory = Mock.Get(fakePathService);
            var request = Mock.Get(controller.Request);
            var response = Mock.Get(controller.Response);
            var lastModified = Mock.Get(fakePathService);
            var responseCache = new Mock<HttpCachePolicyBase>();

            existsDirectory.Setup(c => c.Exists(ABSOLUTEPATH)).Returns(true);
            request.SetupGet(c => c.Headers).Returns(new System.Net.WebHeaderCollection { { "If-Modified-Since", DateTime.UtcNow.AddDays(-2).ToString() } });
            lastModified.Setup(c => c.GetLastWriteTime(ABSOLUTEPATH)).Returns(DateTime.UtcNow);
            response.SetupGet(c => c.Cache).Returns(responseCache.Object);
        }
    }
}
