using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services.StockUpdates;
using Web.WarehouseMgmtUseCase.Model;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class UpdateProductStockServiceTests
    {
        private UpdateStockService _sut;
        private Mock<ISaveOrUpdateCommand<OutpostStockLevel>> _outpostStockLevelSaveCommandMock;
        private Mock<IQueryService<OutpostStockLevel>> _outpostStockLevelQueryServiceMock;
        private Mock<IOutpostHistoricalStockLevelService> _outpostStockLevelHistoryServiceMock;
        private Mock<Outpost> _outpostMock;
        private readonly Guid _outpostId = Guid.NewGuid();
        private ParsedProduct _parsedProduct;

        [SetUp]
        public void PerTestSetup()
        {
            _outpostStockLevelSaveCommandMock = new Mock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            _outpostStockLevelQueryServiceMock = new Mock<IQueryService<OutpostStockLevel>>();
            _outpostStockLevelHistoryServiceMock = new Mock<IOutpostHistoricalStockLevelService>();
            _outpostMock = new Mock<Outpost>();
            _outpostMock.SetupGet(o => o.Id).Returns(_outpostId);
            _parsedProduct = CreateParsedProduct("ABC", "A", 4);
            _sut = new UpdateStockService(_outpostStockLevelSaveCommandMock.Object, _outpostStockLevelQueryServiceMock.Object,
                                          _outpostStockLevelHistoryServiceMock.Object);
        }

        [Test]
        public void UpdateProductStocks_ThrowsException_WhenPassedInAFailedParseResult()
        {
            Assert.That(() => _sut.UpdateProductStocksForOutpost(new SmsParseResult {Success = false}, Guid.Empty, StockUpdateMethod.Manual), Throws.ArgumentException);
        }

        [Test]
        public void UpdateProductStocks_UpdatesTheStockLevelForOneExistingProduct_WhenThereIsOnlyOneResultAfterParsing()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel(_parsedProduct.ProductGroupCode, _parsedProduct.ProductCode)
                }).AsQueryable());

            _sut.UpdateProductStocksForOutpost(new SmsParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {_parsedProduct}
                }, _outpostId, StockUpdateMethod.Manual);

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == _parsedProduct.StockLevel && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == _parsedProduct.ProductCode &&
                                                                   osl.ProductGroup.ReferenceCode == _parsedProduct.ProductGroupCode &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));
        }

        [Test]
        public void UpdateProductStocks_UpdatesTheStockLevelForTwoExistingProduct_WhenThereAreTwoResultsAfterParsing()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel(_parsedProduct.ProductGroupCode, _parsedProduct.ProductCode),
                    CreateOutpostStockLevel("GHJ", "K")
                }).AsQueryable());

            _sut.UpdateProductStocksForOutpost(new SmsParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {_parsedProduct, CreateParsedProduct("GHJ", "K", 3)}
                }, _outpostId, StockUpdateMethod.Manual);

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == _parsedProduct.StockLevel && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == _parsedProduct.ProductCode &&
                                                                   osl.ProductGroup.ReferenceCode == _parsedProduct.ProductGroupCode &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == 3 && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == "K" &&
                                                                   osl.ProductGroup.ReferenceCode == "GHJ" &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));
        }

        [Test]
        public void IncrementProductStocks_UpdatesTheStockLevelForTwoExistingProduct_WhenThereAreTwoResultsAfterParsing()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel("DEF", "L", 1),
                    CreateOutpostStockLevel("GHJ", "K", 2)
                }).AsQueryable());

            _sut.IncrementProductStocksForOutpost(new CsvParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {CreateParsedProduct("DEF", "L", 3), CreateParsedProduct("GHJ", "K", 5)}
                }, _outpostId, StockUpdateMethod.Manual);

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == 4 && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == "L" &&
                                                                   osl.ProductGroup.ReferenceCode == "DEF" &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == 7 && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == "K" &&
                                                                   osl.ProductGroup.ReferenceCode == "GHJ" &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));
        }

        [Test]
        public void DecrementProductStocks_UpdatesTheStockLevelForTwoExistingProduct_WhenThereAreTwoResultsAfterParsing()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel("DEF", "L", 10),
                    CreateOutpostStockLevel("GHJ", "K", 20)
                }).AsQueryable());

            _sut.DecrementProductStocksForOutpost(new CsvParseResult
            {
                Success = true,
                ParsedProducts =
                    new List<IParsedProduct> { CreateParsedProduct("DEF", "L", 3), CreateParsedProduct("GHJ", "K", 5) }
            }, _outpostId, StockUpdateMethod.Manual);

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == 7 && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == "L" &&
                                                                   osl.ProductGroup.ReferenceCode == "DEF" &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));

            _outpostStockLevelSaveCommandMock.Verify(
                cmd => cmd.Execute(It.Is<OutpostStockLevel>(osl => osl.StockLevel == 15 && osl.Outpost.Id == _outpostId &&
                                                                   osl.Product.SMSReferenceCode == "K" &&
                                                                   osl.ProductGroup.ReferenceCode == "GHJ" &&
                                                                   osl.UpdateMethod == StockUpdateMethod.Manual.ToString())));
        }

        [Test]
        public void UpdateProductStocks_NoUpdatesTakePlace_WhenTheParsedCodesDoNotExistInTheSystem()
        {
            //arrange
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel("XXX", "A")
                }).AsQueryable());

            _sut.UpdateProductStocksForOutpost(new SmsParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {_parsedProduct}
                }, _outpostId, StockUpdateMethod.SMS);

            _outpostStockLevelSaveCommandMock.Verify(cmd => cmd.Execute(It.IsAny<OutpostStockLevel>()), Times.Never());
        }

        [Test]
        public void UpdateProductStocks_SetHistoricalStockLevel_WhenThereIsAResultAfterParsing()
        {
            var existingStockLevel = CreateOutpostStockLevel(_parsedProduct.ProductGroupCode, _parsedProduct.ProductCode);
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    existingStockLevel
                }).AsQueryable());

            _sut.UpdateProductStocksForOutpost(new SmsParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {_parsedProduct}
                }, _outpostId, StockUpdateMethod.Manual);

            _outpostStockLevelHistoryServiceMock.Verify(s => s.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(
                It.Is<OutpostStockLevel>(osl => osl == existingStockLevel)));
        }

        [Test]
        public void IncrementProductStocks_ReturnsSuccessAndNoFailedProducts_WhenAllParsedProductsWereFoundToBeValidAndUpdated()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel("DEF", "L", 1),
                    CreateOutpostStockLevel("GHJ", "K", 2)
                }).AsQueryable());

            var result = _sut.IncrementProductStocksForOutpost(new CsvParseResult
            {
                Success = true,
                ParsedProducts =
                    new List<IParsedProduct> { CreateParsedProduct("DEF", "L", 3), CreateParsedProduct("GHJ", "K", 5) }
            }, _outpostId, StockUpdateMethod.Manual);

            Assert.IsTrue(result.Success);
            Assert.Null(result.FailedProducts);
        }

        [Test]
        public void IncrementProductStocks_ReturnsFailAndOneFailedProductOut_WhenOneParsedProductsWasNotFoundToBeRegisteredInTheSystem()
        {
            _outpostStockLevelQueryServiceMock.Setup(s => s.Query()).Returns((new List<OutpostStockLevel>
                {
                    CreateOutpostStockLevel("DEF", "L", 1),
                }).AsQueryable());
            var extraParsedProduct = CreateParsedProduct("GHJ", "K", 5);


            var result = _sut.IncrementProductStocksForOutpost(new CsvParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {CreateParsedProduct("DEF", "L", 3), extraParsedProduct}
                }, _outpostId, StockUpdateMethod.Manual);

            Assert.IsFalse(result.Success);
            CollectionAssert.AreEquivalent(result.FailedProducts, new List<IParsedProduct> {extraParsedProduct});
        }

        #region Helpers

        private OutpostStockLevel CreateOutpostStockLevel(string productGroupCode, string productCode, int stockLevel = 0)
        {
            return new OutpostStockLevel
                {
                    ProductGroup = new ProductGroup {ReferenceCode = productGroupCode},
                    Product = new Product {SMSReferenceCode = productCode},
                    Outpost = _outpostMock.Object,
                    StockLevel = stockLevel
                };
        }

        private static ParsedProduct CreateParsedProduct(string productGroupCode, string productCode, int stockLevel)
        {
            return new ParsedProduct {ClientIdentifier = "F", ProductCode = productCode, ProductGroupCode = productGroupCode, StockLevel = stockLevel};
        }

        #endregion
    }
}
