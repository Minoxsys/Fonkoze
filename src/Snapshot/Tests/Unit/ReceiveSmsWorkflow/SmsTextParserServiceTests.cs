using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsers;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class SmsTextParserServiceTests
    {
        private ISmsTextParserService _sut;

        private const string ActivateMessage = "activate";
        private const string ValidMessageOneProduct = "MALA88F";
        private const string ValidMessageTwoProducts = "ALBG9999N HIVC9F";
        private const string InvalidMessageThreeProducts = "ALBG9999N MALYjF HIVC9F";
        private const string MultipleSpaces = "   ALBG9999N    MALY5F    HIVC9F     ";

        private const string SimpleStockCountMessage = "SC MALA88F";
        private const string InvalidStockCountMessage = "S MALB88F";
        private const string StockCountMesageWithInvalidContents = "SC asdfghjg";
        private const string SimpleReceivedMessage = "RD MALA88F";
        private const string InvalidReceivedMessage = "R MALB88F";
        private const string ReceivedMesageWithInvalidContents = "RD asdfghjg";
       

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
        public void Parse_FailsParsingAsap_WhenInputStringLengthIsLessThanMinimumForValidFormat()
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
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "MAL", ProductCode = "A", StockLevel = 88, ClientIdentifier = "F"});
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
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALB", ProductCode = "G", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "HIV", ProductCode = "C", StockLevel = 9, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ParsingMustContinue_WhenMessageContainsMultipleSpacesBetweenProductSPecs()
        {
            var result = _sut.Parse(MultipleSpaces);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(3);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALB", ProductCode = "G", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "MAL", ProductCode = "Y", StockLevel = 5, ClientIdentifier = "F"});
            result.ParsedProducts[2].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "HIV", ProductCode = "C", StockLevel = 9, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ParsingResultMessageTypeMustBeSetProperly()
        {
            var result = _sut.Parse(MultipleSpaces);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ValidMessageOneProduct);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ValidMessageTwoProducts);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(InvalidMessageThreeProducts);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ActivateMessage);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.Activation));

            result = _sut.Parse(SimpleStockCountMessage);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockCount));
        }

        [Test]
        public void Parse_ShouldBeFlexibleWithActivationMessageAndAllowPunctuationCharsAtTheEndAndBeginning()
        {
            var result = _sut.Parse(",.,." + ActivateMessage + ".,;.");
            Assert.That(result.MessageType, Is.EqualTo(MessageType.Activation));
        }

        [Test]
        public void Parse_FailsParse_WhenProductGroupCodeContainsAnythingBut3Letters()
        {
            var result = _sut.Parse("a5bd88f");
            Assert.False(result.Success);

            result = _sut.Parse("5abd88f");
            Assert.False(result.Success);

            result = _sut.Parse("aa5d88f");
            Assert.False(result.Success);

            result = _sut.Parse("a888888f");
            Assert.False(result.Success);

            result = _sut.Parse("aa4b8f");
            Assert.False(result.Success);

            result = _sut.Parse("aa>b8f");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_FailsParse_WhenProductCodeIsAnythingBut1Letter()
        {
            var result = _sut.Parse("aaa588f");
            Assert.False(result.Success);

            result = _sut.Parse("aaa8888f");
            Assert.False(result.Success);

            result = _sut.Parse("aaa,888f");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_FailsParse_WhenLastLetterIsAnythingButFOrNCaseInsensitive()
        {
            var result = _sut.Parse("aaab588f");
            Assert.True(result.Success);

            result = _sut.Parse("aaab8888n");
            Assert.True(result.Success);

            result = _sut.Parse("aaab8888F");
            Assert.True(result.Success);

            result = _sut.Parse("aaab8888N");
            Assert.True(result.Success);


            result = _sut.Parse("aaab8888c");
            Assert.False(result.Success);

            result = _sut.Parse("aaab88888");
            Assert.False(result.Success);

            result = _sut.Parse("aaab8888/");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_ReturnsParsedProducts_WhenMessageTypeIsStockCount()
        {
            var result = _sut.Parse(SimpleStockCountMessage);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockCount));
            CollectionAssert.AreEquivalent(result.ParsedProducts,
                                           new List<IParsedProduct>
                                               {
                                                   new ParsedProduct {ProductCode = "A", ProductGroupCode = "MAL", StockLevel = 88, ClientIdentifier = "F"}
                                               });
        }

        [Test]
        public void Parse_FailsParsingAsap_WhenWhatAppearsToBeAStockCountMessageIsInvalid()
        {
            var result = _sut.Parse(InvalidStockCountMessage);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WithAnInvalidStringFormatForAStockCountMessage()
        {
            var result = _sut.Parse(StockCountMesageWithInvalidContents);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_ReturnsParsedProducts_WhenMessageTypeIsReceivedStock()
        {
            var result = _sut.Parse(SimpleReceivedMessage);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            Assert.That(result.MessageType, Is.EqualTo(MessageType.ReceivedStock));
            CollectionAssert.AreEquivalent(result.ParsedProducts,
                                           new List<IParsedProduct>
                                               {
                                                   new ParsedProduct {ProductCode = "A", ProductGroupCode = "MAL", StockLevel = 88, ClientIdentifier = "F"}
                                               });
        }

        [Test]
        public void Parse_FailsParsingAsap_WhenWhatAppearsToBeAReceivedStockMessageIsInvalid()
        {
            var result = _sut.Parse(InvalidReceivedMessage);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WithAnInvalidStringFormatForAReceivedStockMessage()
        {
            var result = _sut.Parse(ReceivedMesageWithInvalidContents);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }
    }
}
