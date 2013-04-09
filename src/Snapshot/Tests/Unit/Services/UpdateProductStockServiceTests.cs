using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class UpdateProductStockServiceTests
    {
        private UpdateStockService _sut;

        [SetUp]
        public void PerTestSetup()
        {
            _sut = new UpdateStockService();
        }

        [Test]
        public void UpdateProductStocks_ThrowsException_WhenPassedInAFailedParseResult()
        {
            Assert.That(() => _sut.UpdateProductStocks(new SmsParseResult {Success = false}), Throws.ArgumentException);
        }

        [Test]
        public void UpdateProductStocks_UpdatesTheStockLevelForOneProduct_WhenThereIsOnlyOneResultAfterPArsing()
        {
            _sut.UpdateProductStocks(new SmsParseResult
                {
                    Success = true,
                    ParsedProducts =
                        new List<IParsedProduct> {new ParsedProduct {IsClientIdentifier = "F", ProductCode = "A", ProductGroupCode = "ABC", StockLevel = 4}}
                });
        }

    }
}
