using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services.MessageParsers;
using Web.ReceiveSmsUseCase.Services.MessageParsers.Fonkoze;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class FonkozeSmsTextParserServiceTests
    {
        private ISmsTextParserService _sut;

        private const string ActivateMessage = "activate";
        private const string ValidMessageOneProduct = "AA88F";
        private const string ValidMessageTwoProducts = "AL9999N VC9F";
        private const string InvalidMessageThreeProducts = "BG9999N LYjF VC9F";
        private const string MultipleSpaces = "   BG9999N    LY5F    VC9F     ";

        private const string SimpleStockCountMessage = "SC LA88F";
        private const string InvalidStockCountMessage = "S LB88F";
        private const string StockCountMesageWithInvalidContents = "SC asdfghjg";
        private const string SimpleReceivedMessage = "RD LA88F";
        private const string InvalidReceivedMessage = "R LB88F";
        private const string ReceivedMesageWithInvalidContents = "RD asdfghjg";
       

        [SetUp]
        public void PerTestSetup()
        {
            _sut = new FonkozeSmsTextParserService();
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
            //valid format for one product spec: "<2 letters><only numbers><1 letter>"
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
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "AA", StockLevel = 88, IsClientIdentifier = "F" });
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
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "AL", StockLevel = 9999, IsClientIdentifier = "N" });
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "VC", StockLevel = 9, IsClientIdentifier = "F" });
        }

        [Test]
        public void Parse_ParsingMustContinue_WhenMessageContainsMultipleSpacesBetweenProductSPecs()
        {
            var result = _sut.Parse(MultipleSpaces);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(3);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "BG", StockLevel = 9999, IsClientIdentifier = "N" });
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "LY", StockLevel = 5, IsClientIdentifier = "F" });
            result.ParsedProducts[2].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct { ProductGroupCode = "ALL", ProductCode = "VC", StockLevel = 9, IsClientIdentifier = "F" });
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
        public void Parse_FailsParse_WhenProductGroupCodeContainsAnythingBut0Characters()
        {
            var result = _sut.Parse("abd88f");
            Assert.False(result.Success);

            result = _sut.Parse("5abd88f");
            Assert.False(result.Success);

            result = _sut.Parse("aa5bd88f");
            Assert.False(result.Success);

            result = _sut.Parse("aa4bb8f");
            Assert.False(result.Success);

            result = _sut.Parse("aa>bb8f");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_FailsParse_WhenProductCodeIsAnythingBut2Letter()
        {
            var result = _sut.Parse("588f");
            Assert.False(result.Success);

            result = _sut.Parse("8888f");
            Assert.False(result.Success);

            result = _sut.Parse(".,888f");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_FailsParse_WhenLastLetterIsAnythingButFOrNCaseInsensitive()
        {
            var result = _sut.Parse("ab588f");
            Assert.True(result.Success);

            result = _sut.Parse("ab8888n");
            Assert.True(result.Success);

            result = _sut.Parse("ab8888F");
            Assert.True(result.Success);

            result = _sut.Parse("ab8888N");
            Assert.True(result.Success);


            result = _sut.Parse("ab8888c");
            Assert.False(result.Success);

            result = _sut.Parse("ab88888");
            Assert.False(result.Success);

            result = _sut.Parse("ab8888/");
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
                                                   new ParsedProduct {ProductCode = "LA", ProductGroupCode = "ALL", StockLevel = 88, IsClientIdentifier = "F"}
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
                                                   new ParsedProduct {ProductCode = "LA", ProductGroupCode = "ALL", StockLevel = 88, IsClientIdentifier = "F"}
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
