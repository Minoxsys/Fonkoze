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
        private Mock<IWarehouseManagementWorkflowService> _warehouseMgmtWorflowService;

        [SetUp]
        public void PerTestSetup()
        {
            _fileMock = new Mock<HttpPostedFileBase>();
            _fileMock.Setup(f => f.ContentLength).Returns(0);
            _warehouseMgmtWorflowService = new Mock<IWarehouseManagementWorkflowService>();
            _sut = new WarehouseManagementController(_warehouseMgmtWorflowService.Object);
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
        public void Upload_SendDataStreamToWorkflowServiceToCarryOutTheUseCase_WhenFileIsValid()
        {
            var outpostId = Guid.NewGuid();
            var dummyStream = new Mock<Stream>();
            _fileMock.Setup(f => f.ContentLength).Returns(1);
            _fileMock.Setup(f => f.InputStream).Returns(dummyStream.Object);

            _sut.Upload(_fileMock.Object, outpostId);

            _warehouseMgmtWorflowService.Verify(s => s.ProcessWarehouseStockData(It.Is<Stream>(str => str == dummyStream.Object), outpostId));
        }

        private void AssertErrorMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["invalidFile"], Is.EqualTo("The file selected is an invalid. Please choose another one."));
        }
    }
}
