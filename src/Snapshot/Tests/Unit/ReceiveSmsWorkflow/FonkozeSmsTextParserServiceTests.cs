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
        #region Setup

        private ISmsTextParserService _sut;

        private const string ActivateMessage = "activate";
        private const string ValidMessageOneProduct = "AA88F";
        private const string ValidMessageTwoProducts = "AL9999N VC9F";
        private const string InvalidMessageThreeProducts = "BG9999N LYjF VC9F";
        private const string MultipleSpaces = "   BG9999N    LY5F    VC9F     ";

        private const string SimpleStockCountMessage = "SC LA88";
        private const string InvalidStockCountMessage = "S LB88";
        private const string StockCountMesageWithInvalidContents = "SC asdfghjg";
        private const string SimpleReceivedMessage = "RD LA88";
        private const string InvalidReceivedMessage = "R LB88";
        private const string ReceivedMesageWithInvalidContents = "RD asdfghjg";

        private const string ValidMessageOneProduct2 = "MALAA88F";
        private const string ValidMessageTwoProducts2 = "ALBGG9999N HIVCC9F";
        private const string InvalidMessageThreeProducts2 = "ALBGG9999N MALYjF HIVCC9F";
        private const string MultipleSpaces2 = "   ALBGG9999N    MALYY5F    HIVCC9F     ";

        private const string SimpleStockCountMessage2 = "SC MALAA88";
        private const string InvalidStockCountMessage2 = "S MALB88";
        private const string StockCountMesageWithInvalidContents2 = "SC asdfghjg";
        private const string SimpleReceivedMessage2 = "RD MALAA88";
        private const string InvalidReceivedMessage2 = "R MALBB88";
        private const string ReceivedMesageWithInvalidContents2 = "RD asdfghjg";


        [SetUp]
        public void PerTestSetup()
        {
            _sut = new FonkozeSmsTextParserService();
        }

        #endregion

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

        #region Without group code

        [Test]
        public void Parse_ReturnsOneParsedProduct_WhenMessageContainsAValidSPecificationForOneProduct()
        {
            var result = _sut.Parse(ValidMessageOneProduct);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "AA", StockLevel = 88, ClientIdentifier = "F"});
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
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "AL", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "VC", StockLevel = 9, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ParsingMustContinue_WhenMessageContainsMultipleSpacesBetweenProductSPecs()
        {
            var result = _sut.Parse(MultipleSpaces);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(3);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "BG", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "LY", StockLevel = 5, ClientIdentifier = "F"});
            result.ParsedProducts[2].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALL", ProductCode = "VC", StockLevel = 9, ClientIdentifier = "F"});
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

            result = _sut.Parse(SimpleReceivedMessage);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.ReceivedStock));
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
                                                   new ParsedProduct {ProductCode = "LA", ProductGroupCode = "ALL", StockLevel = 88, ClientIdentifier = string.Empty}
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
                                                   new ParsedProduct {ProductCode = "LA", ProductGroupCode = "ALL", StockLevel = 88, ClientIdentifier = string.Empty}
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

        [Test]
        public void Parse_FailsParsing_WhenReceivedStockMessageTypeContainsClientIndentifier()
        {
            var result = _sut.Parse("RD AA88F");

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.MessageType == MessageType.ReceivedStock);
        }

        [Test]
        public void Parse_ParsingIsSuccessfull_WhenReceivedStockMessageTypeDoesNotContainClientIndentifier()
        {
            var result = _sut.Parse("RD AA88");
         
            Assert.IsTrue(result.MessageType == MessageType.ReceivedStock);
            Assert.IsTrue(result.Success);
            CollectionAssert.AreEquivalent(
                new List<ParsedProduct> {new ParsedProduct {ProductCode = "AA", ProductGroupCode = "ALL", StockLevel = 88, ClientIdentifier = string.Empty}},
                result.ParsedProducts);
        }

        #endregion

        #region With group code in input

        [Test]
        public void Parse_ReturnsOneParsedProduct_WhenMessageContainsAValidSPecificationForOneProduct_WithGroupCode()
        {
            var result = _sut.Parse(ValidMessageOneProduct2);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "MAL", ProductCode = "AA", StockLevel = 88, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ReturnsTwoParsedProducts_WhenMessageContainsTwoValidSpecificationsForProducts_WithGroupCode()
        {
            //valid format for more than one product spec: "<product spec> <product spec> <product spec> etc." (whith one blanck between each)
            var result = _sut.Parse(ValidMessageTwoProducts2);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(2);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALB", ProductCode = "GG", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "HIV", ProductCode = "CC", StockLevel = 9, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ParsingMustContinue_WhenMessageContainsMultipleSpacesBetweenProductSPecs_WithGroupCode()
        {
            var result = _sut.Parse(MultipleSpaces2);

            Assert.IsTrue(result.Success);
            result.ParsedProducts.Should().NotBeEmpty().And.HaveCount(3);
            result.ParsedProducts[0].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "ALB", ProductCode = "GG", StockLevel = 9999, ClientIdentifier = "N"});
            result.ParsedProducts[1].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "MAL", ProductCode = "YY", StockLevel = 5, ClientIdentifier = "F"});
            result.ParsedProducts[2].ShouldHave()
                                    .AllProperties()
                                    .EqualTo(new ParsedProduct {ProductGroupCode = "HIV", ProductCode = "CC", StockLevel = 9, ClientIdentifier = "F"});
        }

        [Test]
        public void Parse_ParsingResultMessageTypeMustBeSetProperly_WithGroupCode()
        {
            var result = _sut.Parse(MultipleSpaces2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ValidMessageOneProduct2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ValidMessageTwoProducts2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(InvalidMessageThreeProducts2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockSale));

            result = _sut.Parse(ActivateMessage);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.Activation));

            result = _sut.Parse(SimpleStockCountMessage2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockCount));

            result = _sut.Parse(SimpleReceivedMessage2);
            Assert.That(result.MessageType, Is.EqualTo(MessageType.ReceivedStock));
        }

        [Test]
        public void Parse_FailsParse_WhenProductGroupCodeContainsAnythingBut3Letters_WithGroupCode()
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
        public void Parse_FailsParse_WhenProductCodeIsAnythingBut1Letter_WithGroupCode()
        {
            var result = _sut.Parse("aaa588f");
            Assert.False(result.Success);

            result = _sut.Parse("aaa8888f");
            Assert.False(result.Success);

            result = _sut.Parse("aaa,888f");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_FailsParse_WhenLastLetterIsAnythingButFOrNCaseInsensitive_WithGroupCode()
        {
            var result = _sut.Parse("aaabb588f");
            Assert.True(result.Success);

            result = _sut.Parse("aaabb8888n");
            Assert.True(result.Success);

            result = _sut.Parse("aaabb8888F");
            Assert.True(result.Success);

            result = _sut.Parse("aaabb8888N");
            Assert.True(result.Success);


            result = _sut.Parse("aaabb8888c");
            Assert.False(result.Success);

            result = _sut.Parse("aaabb88888");
            Assert.False(result.Success);

            result = _sut.Parse("aaabb8888/");
            Assert.False(result.Success);
        }

        [Test]
        public void Parse_ReturnsParsedProducts_WhenMessageTypeIsStockCount_WithGroupCode()
        {
            var result = _sut.Parse(SimpleStockCountMessage2);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            Assert.That(result.MessageType, Is.EqualTo(MessageType.StockCount));
            CollectionAssert.AreEquivalent(result.ParsedProducts,
                                           new List<IParsedProduct>
                                               {
                                                   new ParsedProduct {ProductCode = "AA", ProductGroupCode = "MAL", StockLevel = 88, ClientIdentifier = string.Empty}
                                               });
        }

        [Test]
        public void Parse_FailsParsingAsap_WhenWhatAppearsToBeAStockCountMessageIsInvalid_WithGroupCode()
        {
            var result = _sut.Parse(InvalidStockCountMessage2);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WithAnInvalidStringFormatForAStockCountMessage_WithGroupCode()
        {
            var result = _sut.Parse(StockCountMesageWithInvalidContents2);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_ReturnsParsedProducts_WhenMessageTypeIsReceivedStock_WithGroupCode()
        {
            var result = _sut.Parse(SimpleReceivedMessage2);

            Assert.IsTrue(result.Success);
            Assert.That(result.ParsedProducts.Count, Is.EqualTo(1));
            Assert.That(result.MessageType, Is.EqualTo(MessageType.ReceivedStock));
            CollectionAssert.AreEquivalent(result.ParsedProducts,
                                           new List<IParsedProduct>
                                               {
                                                   new ParsedProduct {ProductCode = "AA", ProductGroupCode = "MAL", StockLevel = 88, ClientIdentifier = string.Empty}
                                               });
        }

        [Test]
        public void Parse_FailsParsingAsap_WhenWhatAppearsToBeAReceivedStockMessageIsInvalid_WithGroupCode()
        {
            var result = _sut.Parse(InvalidReceivedMessage2);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WithAnInvalidStringFormatForAReceivedStockMessage_WithGroupCode()
        {
            var result = _sut.Parse(ReceivedMesageWithInvalidContents2);

            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid message format."));
        }

        [Test]
        public void Parse_FailsParsing_WhenReceivedStockMessageTypeContainsClientIndentifier_WithGroupCode()
        {
            var result = _sut.Parse("RD MALAA88F");

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.MessageType == MessageType.ReceivedStock);
        }

        [Test]
        public void Parse_ParsingIsSuccessfull_WhenReceivedStockMessageTypeDoesNotContainClientIndentifier_WithGroupCode()
        {
            var result = _sut.Parse("RD MALAA88");

            Assert.IsTrue(result.MessageType == MessageType.ReceivedStock);
            Assert.IsTrue(result.Success);
            CollectionAssert.AreEquivalent(
                new List<ParsedProduct> { new ParsedProduct { ProductCode = "AA", ProductGroupCode = "MAL", StockLevel = 88, ClientIdentifier = string.Empty } },
                result.ParsedProducts);
        }

        #endregion
    }
}
