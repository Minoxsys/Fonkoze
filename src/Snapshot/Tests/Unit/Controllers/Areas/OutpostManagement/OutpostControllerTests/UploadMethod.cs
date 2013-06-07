using System;
using MvcContrib.TestHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Web.Security;
using Web.Areas.OutpostManagement.Services;
using Web.Areas.OutpostManagement.Models.Outpost;
using System.IO;
using Web.Models.Parsing.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
    [TestFixture]
    public class UploadMethod
    {
        private readonly ObjectMother _ = new ObjectMother();
        private Mock<HttpPostedFileBase> _fileMock;

        [SetUp]
        public void BeforeEach()
        {
            _.Init();

            _fileMock = new Mock<HttpPostedFileBase>();
        }

        [Test]
        public void Upload_RedirectsToOverview_WhenDoneProcessing()
        {
            var result = _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());

            result.AssertResultIs<RedirectToRouteResult>();
            result.AssertActionRedirect().ToAction("Overview");
        }

        [Test]
        public void Upload_ReturnsErrorMessageInTempData_WhenFileIsEmpty()
        {
            _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());
            AssertErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsErrorMessageInTempData_WhenFileIsNull()
        {
            _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());
            AssertErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsSuccessMessageInTempData_WhenUploadSuccesfullAndNoFailedOutposts()
        {
            CreateDummyValidStream();

            _._outpostsParserMock.Setup(s => s.ParseStream(_fileMock.Object.InputStream)).Returns(new OutpostsParseResult { Success = true });

            _._outpostsUpdateService.Setup(s => s.ManageParseOutposts(It.IsAny<UserAndClientIdentity>(), It.IsAny<IOutpostsParseResult>())).
                                                                        Returns(new OutpostsUpdateResult { Success = true });

            _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());

            AssertSuccessMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsFailParseMessageInTempData_WhenParsingFailed()
        {
            CreateDummyValidStream();

            _._outpostsParserMock.Setup(s => s.ParseStream(_fileMock.Object.InputStream)).Returns(new OutpostsParseResult { Success = false });

            _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());

            AssertFaildParsingErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsFailedOutpostsMessageInTempData_WhenUploadSuccesfullAndFailedProducts()
        {
            CreateDummyValidStream();

            _._outpostsParserMock.Setup(s => s.ParseStream(_fileMock.Object.InputStream)).Returns(new OutpostsParseResult { Success = true });

            _._outpostsUpdateService.Setup(s => s.ManageParseOutposts(It.IsAny<UserAndClientIdentity>(), It.IsAny<IOutpostsParseResult>())).
                                                            Returns(new OutpostsUpdateResult { FailedOutposts = CreateParsedOutposts()});

            _.controller.Upload(_fileMock.Object, It.IsAny<UserAndClientIdentity>());

            AssertFailedProductsMessageIsPostedInTempData();
        }

        private Mock<Stream> CreateDummyValidStream()
        {
            var dummyStream = new Mock<Stream>();
            _fileMock.Setup(f => f.ContentLength).Returns(1);
            _fileMock.Setup(f => f.InputStream).Returns(dummyStream.Object);
            return dummyStream;
        }

        private void AssertErrorMessageIsPostedInTempData()
        {
            Assert.That(_.controller.TempData["result"], Is.EqualTo("The file selected is invalid. Please choose another one."));
        }

        private void AssertFaildParsingErrorMessageIsPostedInTempData()
        {
            Assert.That(_.controller.TempData["result"], Is.EqualTo("CSV file parsing has failed. Please check the contents of the CSV file to be valid."));
        }

        private void AssertSuccessMessageIsPostedInTempData()
        {
            Assert.That(_.controller.TempData["result"], Is.EqualTo("The Sellers update was succesful."));
        }

        private void AssertFailedProductsMessageIsPostedInTempData()
        {
            Assert.That(_.controller.TempData["result"], Is.EqualTo("The file uploaded successfully. The following Sellers could not be updated:" + " Name1, Name2."));
        }

        private IList<IParsedOutpost> CreateParsedOutposts()
        {
            return new List<IParsedOutpost>
                {
                  new ParsedOutpost { Country = "Country1", Region = "Region1", District = "District1", Name = "Name1", Longitude = "1.1", Latitude = "0.1", ContactDetail = "898989898" },
                  new ParsedOutpost { Country = "Country2", Region = "Region2", District = "District2", Name = "Name2", Longitude = "1.321", Latitude = "0.2131", ContactDetail = "877898898" }
                };
        }
    }
}
