using System;
using FluentAssertions;
using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.WarehouseMgmtUseCase.Controllers;
using Web.WarehouseMgmtUseCase.Services;

namespace Tests.Unit.WarehouseManagementWorkflow.WarehouseManagementControllerTests
{
    [TestFixture]
    public class WarehouseManagementControllerTests
    {
        private WarehouseManagementController _sut;
        private Mock<HttpPostedFileBase> _fileMock;
        private Mock<IWarehouseManagementWorkflowService> _warehouseMgmtWorflowServiceMock;

        [SetUp]
        public void PerTestSetup()
        {
            _fileMock = new Mock<HttpPostedFileBase>();
            _fileMock.Setup(f => f.ContentLength).Returns(0);
            _warehouseMgmtWorflowServiceMock = new Mock<IWarehouseManagementWorkflowService>();
            _sut = new WarehouseManagementController(_warehouseMgmtWorflowServiceMock.Object);
        }

        [Test]
        public void Overview_ReturnsTheCorrectViewWithEmptyViewModel()
        {
            var result = _sut.Overview();

            result.AssertViewRendered().ForView("");
            Assert.That(result.Model, Is.InstanceOf<OutpostOverviewModel>());
            (result.Model as OutpostOverviewModel).ShouldHave().AllProperties().EqualTo(new OutpostOverviewModel());
        }

        [Test]
        public void Upload_RedirectsToOverview_WhenDoneProcessing()
        {
            var result = _sut.Upload(_fileMock.Object, Guid.Empty);

            result.AssertResultIs<RedirectToRouteResult>();
            result.AssertActionRedirect().ToAction("Overview");
        }

        [Test]
        public void Upload_ReturnsErrorMessageInTempData_WhenFileIsEmpty()
        {
            _sut.Upload(_fileMock.Object, Guid.Empty);
            AssertErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsErrorMessageInTempData_WhenFileIsNull()
        {
            _sut.Upload(null, Guid.Empty);
            AssertErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsSuccessMessageInTempData_WhenUploadSuccesfull()
        {
            CreateDummyValidStream();
            _warehouseMgmtWorflowServiceMock.Setup(s => s.ProcessWarehouseStockData(It.IsAny<Stream>(), It.IsAny<Guid>())).Returns(true);

            _sut.Upload(_fileMock.Object, Guid.Empty);

            AssertSuccessMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsFailParseMessageInTempData_WhenParsingFailed()
        {
            CreateDummyValidStream();

            _sut.Upload(_fileMock.Object, Guid.Empty);
            AssertFaildParsingErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_SendDataStreamToWorkflowServiceToCarryOutTheUseCase_WhenFileIsValid()
        {
            var outpostId = Guid.NewGuid();
            var dummyStream = CreateDummyValidStream();

            _sut.Upload(_fileMock.Object, outpostId);

            _warehouseMgmtWorflowServiceMock.Verify(s => s.ProcessWarehouseStockData(It.Is<Stream>(str => str == dummyStream.Object), outpostId));
        }

        private Mock<Stream> CreateDummyValidStream()
        {
            var dummyStream = new Mock<Stream>();
            _fileMock.Setup(f => f.ContentLength).Returns(1);
            _fileMock.Setup(f => f.InputStream).Returns(dummyStream.Object);
            return dummyStream;
        }

        private void AssertFaildParsingErrorMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["result"], Is.EqualTo("CSV file parsing has failed. Please check the contents of the CSV file to be valid."));
        }

        private void AssertSuccessMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["result"], Is.EqualTo("The file uploaded successfully."));
        }

        private void AssertErrorMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["result"], Is.EqualTo("The file selected is an invalid. Please choose another one."));
        }
    }
}
