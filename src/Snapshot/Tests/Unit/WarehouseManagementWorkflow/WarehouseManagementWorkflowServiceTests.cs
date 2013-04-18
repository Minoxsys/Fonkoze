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
        private Mock<Stream> _dummyStream;
        private Guid _outpostId;

        [SetUp]
        public void PerTestSetup()
        {
            _outpostId = Guid.NewGuid();
            _dummyStream = new Mock<Stream>();
            _stockUpdateCsvFileParser = new Mock<IStockUpdateCsvFileParserService>();
            _updateStockServiceMock = new Mock<IUpdateStockService>();
            _sut = new WarehouseManagementWorkflowService(_stockUpdateCsvFileParser.Object, _updateStockServiceMock.Object);
        }

        [Test]
        public void UpdatesStockWithParsedDataFromTheStream_WhenParsingSuccessful()
        {
            var list = new List<IParsedProduct>();
            
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult {ParsedProducts = list, Success = true});

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _updateStockServiceMock.Verify(
                s => s.UpdateProductStocksForOutpost(It.Is<CsvParseResult>(r => r.Success && r.ParsedProducts == list), _outpostId));
        }

        [Test]
        public void DoesNotUpdateStock_WhenParsingFails()
        {
            _stockUpdateCsvFileParser.Setup(s => s.ParseStream(_dummyStream.Object))
                                     .Returns(new CsvParseResult {Success = false});

            _sut.ProcessWarehouseStockData(_dummyStream.Object, _outpostId);

            _updateStockServiceMock.Verify(
                s => s.UpdateProductStocksForOutpost(It.IsAny<CsvParseResult>(), _outpostId), Times.Never());
        }
    }
}
