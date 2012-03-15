using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.JavaScriptControllerTests
{
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
        public void GetIndex_IfJavaScriptDirectoryDoesNotExist_Returns_StatusCode_404()
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
        public void GetIndex_IfETagHasNotBeenModified_Returns_StatusCode_304()
        {
            //Arrange
            objectMother.SetUpStubs_For_DirectoryExists_And_EtagIdentification();

            //Act
            var result = objectMother.controller.Index(SITE);

            //Assert
            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.AreEqual(304, objectMother.controller.Response.StatusCode);
            Assert.AreEqual(true, objectMother.controller.HttpContext.Response.SuppressContent);
        }

        [Test]
        public void GetIndex_IfContentHasNotBeenModified_Returns_StatusCode_304()
        {
            //Arrange
            objectMother.SetUpStubs_For_DirectoryExists_UnmodifiedContent_And_EtagDontMatch();

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
            Assert.AreEqual("text/javascript", contentResult.ContentType);
        }
    }
}
