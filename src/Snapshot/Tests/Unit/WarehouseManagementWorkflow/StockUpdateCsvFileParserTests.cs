using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Web.Models.Parsing;
using Web.WarehouseMgmtUseCase.Model;
using Web.WarehouseMgmtUseCase.Services;

namespace Tests.Unit.WarehouseManagementWorkflow
{
    [TestFixture]
    public class StockUpdateCsvFileParserTests
    {
        private StockUpdateCsvFileParserService _sut;

        [SetUp]
        public void PerTestSetup()
        {
            _sut = new StockUpdateCsvFileParserService();
        }

        [Test]
        public void ParseStream_ReturnsTheListOfParsedObjects_WhenParsingWasSuccesfull()
        {
            //arrange
            CsvParseResult result;
            var list = CreateListOfProducts();
            var parsedProducts = list as IList<ParsedProduct> ?? list.ToList();
            using (var stream = CreateStreamWithValidValues(parsedProducts))
            {
                stream.Seek(0, SeekOrigin.Begin);

                //act
                result = _sut.ParseStream(stream);
            }

            //assert
            Assert.That(result.Success, Is.EqualTo(true));
            CollectionAssert.AreEquivalent(parsedProducts, result.ParsedProducts);
        }

        [Test]
        public void ParseStream_ReturnsAnEmptyList_WhenParsingWasSuccesfullButThereWereNoItems()
        {
            //arrange
            CsvParseResult result;
            var parsedProducts = new List<ParsedProduct>();
            using (var stream = CreateStreamWithValidValues(parsedProducts))
            {
                stream.Seek(0, SeekOrigin.Begin);

                //act
                result = _sut.ParseStream(stream);
            }

            //assert
            Assert.That(result.Success, Is.EqualTo(true));
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(0));
        }

        [Test]
        public void ParseStream_ReturnsFailedParse_WhenTheContentsOfTheFileAreTrashed()
        {
            //arrange
            CsvParseResult result;

            using (var stream = CreateStreamWithTrashContent())
            {
                stream.Seek(0, SeekOrigin.Begin);

                //act
                result = _sut.ParseStream(stream);
            }

            //assert
            Assert.That(result.Success, Is.EqualTo(false));
        }

        #region Helpers

        private Stream CreateStreamWithTrashContent()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            for (int i = 0; i < 20; i++)
            {
                if (i%2 == 0)
                {
                    writer.WriteLine("hdjdjdj  dsa {0}, ..f.f..df.d.fd.f.dfs.fdsfd.sfsd.{1}", i, i + 1);
                }
                else
                {
                    writer.WriteLine("1 1");
                }
            }
            writer.Flush();
            return stream;
        }

        private Stream CreateStreamWithValidValues(IEnumerable<ParsedProduct> productsList)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            foreach (var product in productsList)
            {
                writer.WriteLine("{0}, {1}, {2}", product.ProductGroupCode, product.ProductCode, product.StockLevel);
            }
            writer.Flush();
            return stream;
        }

        private IEnumerable<ParsedProduct> CreateListOfProducts()
        {
            var list = new List<ParsedProduct>();
            for (int i = 'A'; i < 'Z'; i++)
            {
                list.Add(new ParsedProduct {ProductGroupCode = "ASB", ProductCode = i.ToString(CultureInfo.InvariantCulture), StockLevel = i});
            }
            return list;
        }

        #endregion
    }
}
