using System.Collections.Generic;
using Domain;
using Moq;
using System.Linq;
using NUnit.Framework;
using Tests.Unit.ReceiveSmsWorkflow.Base;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.SmsMessageCommands;
using Web.Services.StockUpdates;

namespace Tests.Unit.ReceiveSmsWorkflow.Commands
{
    [TestFixture]
    public class StockSaleMessageCommandTests : StockUpdateMessageCommandTestsBase
    {
        protected override StockUpdateMessageCommandBase CreateConcreteCommand()
        {
            return new StockSaleMessageCommand(UpdateProductStockServiceMock.Object, SendSmsServiceMock.Object, SaveAlertCmdMock.Object,
                                               SendEmailServiceMock.Object,
                                               RawSmsQueryServiceMock.Object, ProductQueryServiceMock.Object, SaveProductSaleCmdMock.Object);
        }

        [Test]
        public override void ExecutingTheCommand_UpdatesStockForProducts_WhenMessageParsedSuccesfully()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};

            Sut.Execute(InputModel, parseResult, OutpostMock.Object);

            UpdateProductStockServiceMock.Verify(s => s.DecrementProductStocksForOutpost(parseResult, OutpostMock.Object.Id, StockUpdateMethod.SMS));
        }

        [Test]
        public void DoAfterStockUpdate_Saves2ProductSale_WhenSmsContains2DifferentProducts()
        {
            //Arrange
            SetupKnownSender();
            List<IParsedProduct> l = new List<IParsedProduct>();
            l.Add(new ParsedProduct(){ProductGroupCode="ll",ProductCode="er",StockLevel=2,ClientIdentifier="f"});
            l.Add(new ParsedProduct(){ProductGroupCode="ll",ProductCode="be",StockLevel=2,ClientIdentifier="n"});
            var parseResult = new SmsParseResult { ParsedProducts = l };
            ProductQueryServiceMock.Setup(s => s.Query()).Returns(new Product[] { new Product() { SMSReferenceCode = "er" }, new Product() { SMSReferenceCode = "be" } }.AsQueryable());
            //Act    
            Sut.DoAfterStockUpdate(parseResult, OutpostMock.Object);
            //Assert
            SaveProductSaleCmdMock.Verify(cmd => cmd.Execute(It.Is<ProductSale>(ps => ps.Outpost == OutpostMock.Object
                                                                            && ps.Product.SMSReferenceCode == "er"
                                                                            && ps.Quantity == 2 && ps.ClientIdentifier == "f")));
            SaveProductSaleCmdMock.Verify(cmd => cmd.Execute(It.Is<ProductSale>(ps => ps.Outpost == OutpostMock.Object
                                                                           && ps.Product.SMSReferenceCode == "be"
                                                                           && ps.Quantity == 2 && ps.ClientIdentifier == "n")));


        }


       
    }
}
