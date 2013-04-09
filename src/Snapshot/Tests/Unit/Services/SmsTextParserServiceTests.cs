using FluentAssertions;
using NUnit.Framework;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class SmsTextParserServiceTests
    {
        private ISmsTextParserService _sut;

        private const string ValidMessageOneProduct = "MALA88F";
        private const string ValidMessageTwoProducts = "ALBG9999N HIVC9F";
        private const string InvalidMessageThreeProducts = "ALBG9999N MALYjF HIVC9F";
        private const string MultipleSpaces = "   ALBG9999N    MALY5F    HIVC9F     ";

        [SetUp]
        public void PerTestSetup()
        {
            _sut = new SmsTextParserService();
        }

        [Test]
        public void Parse_FailsParsing_WhenEmptyMessage()
        {
            var result = _sut.Parse("");

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WithAnInvalidStringFormat()
        {
            //valid format for one product spec: "<3 letters><1 letter><only numbers><1 letter>"
            var result = _sut.Parse("abcd6eeeeef");

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsingAsap_WhenInputStringLeghtIsLessThanMinimumForValidFormat()
        {
            var result = _sut.Parse("abc");

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_ReturnsOneParsedProduct_WhenMessageContainsAValidSPecificationForOneProduct()
        {
            var result = _sut.Parse(ValidMessageOneProduct);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "MAL", ProductCode = "A", StockLevel = 88, IsClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ReturnsTwoParsedProducts_WhenMessageContainsTwoValidSpecificationsForProducts()
        {
            //valid format for more than one product spec: "<product spec> <product spec> <product spec> etc." (whith one blanck between each)
            var result = _sut.Parse(ValidMessageTwoProducts);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(2);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALB", ProductCode = "G", StockLevel = 9999, IsClientIdentifier = "N" });
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "HIV", ProductCode = "C", StockLevel = 9, IsClientIdentifier = "F" });
        }

        [Test]
        public void Parse_FailsTheParseWithDescriptiveMessage_WhenAtLeastOneProductSpecificationIsWrong()
        {
            var result = _sut.Parse(InvalidMessageThreeProducts);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("At least one product specification is invalid."));
        }

        [Test]
        public void Parse_ParsingMustContinue_WhenMessageContainsMultipleSpacesBetweenProductSPecs()
        {
            var result = _sut.Parse(MultipleSpaces);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(3);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALB", ProductCode = "G", StockLevel = 9999, IsClientIdentifier = "N" });
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "MAL", ProductCode = "Y", StockLevel = 5, IsClientIdentifier = "F" });
            result.ParsedProducts[2].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "HIV", ProductCode = "C", StockLevel = 9, IsClientIdentifier = "F" });
        }

    }
}
