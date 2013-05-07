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
using Web.Services.StockUpdates;
using Web.Models.Parsing;
using System.Collections.Generic;

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
        public void Upload_ReturnsSuccessMessageInTempData_WhenUploadSuccesfullAndNoFailedProducts()
        {
            CreateDummyValidStream();
            _warehouseMgmtWorflowServiceMock.Setup(s => s.ProcessWarehouseStockData(It.IsAny<Stream>(), It.IsAny<Guid>())).Returns(new StockUpdateResult { Success = true });

            _sut.Upload(_fileMock.Object, Guid.Empty);

            AssertSuccessMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_ReturnsFailParseMessageInTempData_WhenParsingFailed()
        {
            CreateDummyValidStream();
            _warehouseMgmtWorflowServiceMock.Setup(s => s.ProcessWarehouseStockData(It.IsAny<Stream>(), It.IsAny<Guid>())).Returns(new StockUpdateResult { Success = false });

            _sut.Upload(_fileMock.Object, Guid.Empty);
            AssertFaildParsingErrorMessageIsPostedInTempData();
        }

        [Test]
        public void Upload_SendDataStreamToWorkflowServiceToCarryOutTheUseCase_WhenFileIsValid()
        {
            var outpostId = Guid.NewGuid();
            var dummyStream = CreateDummyValidStream();

            _warehouseMgmtWorflowServiceMock.Setup(s => s.ProcessWarehouseStockData(It.IsAny<Stream>(), It.IsAny<Guid>())).Returns(new StockUpdateResult { Success = false });

            _sut.Upload(_fileMock.Object, outpostId);

            _warehouseMgmtWorflowServiceMock.Verify(s => s.ProcessWarehouseStockData(It.Is<Stream>(str => str == dummyStream.Object), outpostId));
        }

        [Test]
        public void Upload_ReturnsFailedProductsMessageInTempData_WhenUploadSuccesfullAndFailedProducts()
        {
            CreateDummyValidStream();
            _warehouseMgmtWorflowServiceMock.Setup(s => s.ProcessWarehouseStockData(It.IsAny<Stream>(), It.IsAny<Guid>())).Returns(new StockUpdateResult {FailedProducts = CreateFailedProducts(), Success = false });

            _sut.Upload(_fileMock.Object, Guid.Empty);
            AssertFailedProductsMessageIsPostedInTempData();
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
            Assert.That(_sut.TempData["result"], Is.EqualTo("The stock update was successful."));
        }

        private void AssertFailedProductsMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["result"], Is.EqualTo("The file uploaded successfully. The following Products could not be updated:"+ " TTB, YYZ."));
        }

        private void AssertErrorMessageIsPostedInTempData()
        {
            Assert.That(_sut.TempData["result"], Is.EqualTo("The file selected is invalid. Please choose another one."));
        }

        private List<IParsedProduct> CreateFailedProducts()
        {
            var productList = new List<IParsedProduct>();
            var failedParsedProduct1 = new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "TTB", StockLevel = 12 };
            var failedParsedProduct2 = new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "YYZ", StockLevel = 12 };
            productList.Add(failedParsedProduct1);
            productList.Add(failedParsedProduct2);

            return productList;
        }
    }
}
