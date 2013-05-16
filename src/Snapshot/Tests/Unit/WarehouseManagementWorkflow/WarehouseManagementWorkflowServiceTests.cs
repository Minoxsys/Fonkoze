using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Web.Models.Parsing;
using Web.Services.StockUpdates;
using Web.WarehouseMgmtUseCase.Model;
using Web.WarehouseMgmtUseCase.Services;

namespace Tests.Unit.WarehouseManagementWorkflow
{
    [TestFixture]
    public class WarehouseManagementWorkflowServiceTests
    {
        private WarehouseManagementWorkflowService _sut;
        private Mock<IStockUpdateCsvFileParserService> _stockUpdateCsvFileParser;
        private Mock<IUpdateStockService> _updateStockServiceMock;
        private Mock<IAssigningExistingProductsToWarehouseService> _assigningExistingProductsToWarehouseServiceMock;
        private Mock<Stream> _dummyStream;
        private Guid _outpostId;

        [SetUp]
        public void PerTestSetup()
        {
            _outpostId = Guid.NewGuid();
            _dummyStream = new Mock<Stream>();
            _stockUpdateCsvFileParser = new Mock<IStockUpdateCsvFileParserService>();
            _updateStockServiceMock = new Mock<IUpdateStockService>();
            _assigningExistingProductsToWarehouseServiceMock = new Mock<IAssigningExistingProductsToWarehouseService>();
            _sut = new WarehouseManagementWorkflowService(_stockUpdateCsvFileParser.Object, _updateStockServiceMock.Object, _assigningExistingProductsToWarehouseServiceMock.Object);
        }

        [Test]
        public void ProcessWarehouseStockData_AssignExistingProductsToWarehouseFromUpdateStockFailedProducts_WhenParseSuccess()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = true });

            var stockUpdateResult = new StockUpdateResult { Success = false, FailedProducts = new List<IParsedProduct>() };
            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<IParseResult>(), _outpostId, It.IsAny<StockUpdateMethod>()))
                .Returns(stockUpdateResult);

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _assigningExistingProductsToWarehouseServiceMock.Verify(
                s => s.AssigningProductsToWarehouse(stockUpdateResult.FailedProducts, _outpostId));
        }

        [Test]
        public void ProcessWarehouseStockData_DontAssignExistingProductsToWarehouseFromUpdateStockFailedProducts_WhenParseSuccessAndFailedProductsFromStockUpdateResultIsNull()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = true });

            var stockUpdateResult = new StockUpdateResult { Success = false, FailedProducts = null };
            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<IParseResult>(), _outpostId, It.IsAny<StockUpdateMethod>()))
                .Returns(stockUpdateResult);

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _assigningExistingProductsToWarehouseServiceMock.Verify(
                s => s.AssigningProductsToWarehouse(stockUpdateResult.FailedProducts, _outpostId), Times.Never());
        }

        [Test]
        public void ProcessWarehouseStockData_TryToMakeAnotherStockUpdate_WhenParseSuccessAndThereAreFailedProducts()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = true });

            var stockUpdateResult = new StockUpdateResult { Success = false, FailedProducts = CreateFailedProducts() };
            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<IParseResult>(), _outpostId, It.IsAny<StockUpdateMethod>()))
                .Returns(stockUpdateResult);

            var csvParseResult = new CsvParseResult
            {
                Success = true,
                ParsedProducts = stockUpdateResult.FailedProducts
            };

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _updateStockServiceMock.Verify(
                s => s.IncrementProductStocksForOutpost(It.Is<CsvParseResult>(x => x.Success == true), _outpostId, StockUpdateMethod.CSV));

            _updateStockServiceMock.Verify(
                s => s.IncrementProductStocksForOutpost(It.Is<CsvParseResult>(x => x.Success == true && x.ParsedProducts == stockUpdateResult.FailedProducts), _outpostId, StockUpdateMethod.CSV));

        }

        [Test]
        public void IncrementsStockLevelsFromParsedDataFromTheStream_WhenParsingSuccessful()
        {
            var list = new List<IParsedProduct>();

            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { ParsedProducts = list, Success = true });

            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<IParseResult>(), _outpostId, It.IsAny<StockUpdateMethod>()))
                .Returns(new StockUpdateResult());

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _updateStockServiceMock.Verify(
                s => s.IncrementProductStocksForOutpost(It.Is<CsvParseResult>(r => r.Success && r.ParsedProducts == list), _outpostId, StockUpdateMethod.CSV));
        }

        [Test]
        public void DoesNotIncrementStock_WhenParsingFails()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = false });

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _updateStockServiceMock.Verify(
                s => s.IncrementProductStocksForOutpost(It.IsAny<CsvParseResult>(), _outpostId, StockUpdateMethod.CSV), Times.Never());
        }

        [Test]
        public void ReturnsNullFailedProductsAndSuccessTrue_WhenParseSuccessAndNoFailedProducts()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = true });

            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<CsvParseResult>(), _outpostId, StockUpdateMethod.CSV))
                                     .Returns(new StockUpdateResult { FailedProducts = null, Success = true });

            var result = _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            Assert.IsTrue(result.Success);
            Assert.IsNull(result.FailedProducts);
        }

        [Test]
        public void ReturnsNotNullFailedProductsAndSuccessFalse_WhenParseSuccessAndThereAreFailedProducts()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = true });

            _updateStockServiceMock.Setup(s => s.IncrementProductStocksForOutpost(It.IsAny<CsvParseResult>(), _outpostId, StockUpdateMethod.CSV))
                                     .Returns(new StockUpdateResult { FailedProducts = CreateFailedProducts(), Success = false });

            var result = _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.FailedProducts);
        }

        [Test]
        public void ReturnsNullFailedProductsAndSuccessFalse_WhenParseFailed()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult { Success = false });

            var result = _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.FailedProducts);
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
