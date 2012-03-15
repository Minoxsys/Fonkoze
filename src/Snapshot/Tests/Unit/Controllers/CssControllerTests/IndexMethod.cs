using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.CssControllerTests
{
    [TestFixture]
    public class IndexMethod
    {
        public ObjectMother objectMother = new ObjectMother();
        public const string SITE = "site";

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void GetIndex_IfCssDirectoryDoesNotExist_Returns_StatusCode_404()
        {
            //Arrange
            objectMother.SetUpStubs_For_DirectoryDoesNotExist();

            //Act
            var result = objectMother.controller.Index(SITE);

            //Assert
            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.AreEqual(404, objectMother.controller.Response.StatusCode);
            Assert.AreEqual(true, objectMother.controller.HttpContext.Response.SuppressContent);
        }

        [Test]
        public void GetIndex_ETagHasNotBeenModified_Returns_StatusCode_304()
        {
            //Arrange
            objectMother.SetUpStubs_For_DirectoryExists_And_IdentificationBylEtag();

            //Act
            var result = objectMother.controller.Index(SITE);

            //Assert
            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.AreEqual(304, objectMother.controller.Response.StatusCode);
            Assert.AreEqual(true, objectMother.controller.HttpContext.Response.SuppressContent);
        }

        [Test]
        public void GetIndex_IfContextHasNotBeenModified_Returns_StatusCode_304()
        {
            //Arrange
            objectMother.SetUpStubs_For_DirectoryExists_IdentificationBylEtag_NotMatch_And_Unmodified();

            //Act
            var result = objectMother.controller.Index(SITE);

            //Assert
            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.AreEqual(304, objectMother.controller.Response.StatusCode);
            Assert.AreEqual(true, objectMother.controller.HttpContext.Response.SuppressContent);
        }

        [Test]
        public void GetIndex_Returns_ContentResult()
        {
            //Arrange
            objectMother.SetUpStubs_For_ReturningContentResult();

            //Act
            var result = objectMother.controller.Index(SITE);

            //Assert
            Assert.IsInstanceOf<ContentResult>(result);
            var contentResult = (ContentResult)result;
            Assert.AreEqual("abc", contentResult.Content);
            Assert.AreEqual(Encoding.UTF8, contentResult.ContentEncoding);
            Assert.AreEqual("text/css", contentResult.ContentType);
        }
    }
}
