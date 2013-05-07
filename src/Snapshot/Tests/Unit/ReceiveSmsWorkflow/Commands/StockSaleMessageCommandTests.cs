﻿using NUnit.Framework;
using Tests.Unit.ReceiveSmsWorkflow.Base;
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

        public override void ExecutingTheCommand_UpdatesStockForProducts_WhenMessageParsedSuccesfully()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};

            Sut.Execute(InputModel, parseResult, OutpostMock.Object);

            UpdateProductStockServiceMock.Verify(s => s.DecrementProductStocksForOutpost(parseResult, OutpostMock.Object.Id, StockUpdateMethod.SMS));
        }
    }
}