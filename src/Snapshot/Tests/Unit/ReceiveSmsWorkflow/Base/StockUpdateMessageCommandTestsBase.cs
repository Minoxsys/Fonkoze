using Core.Domain;
using Core.Persistence;
using Domain;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.SmsMessageCommands;
using Web.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Tests.Unit.ReceiveSmsWorkflow.Base
{
    [TestFixture]
    public abstract class StockUpdateMessageCommandTestsBase
    {
        #region Setup

        protected StockUpdateMessageCommandBase Sut;
        protected Mock<IUpdateStockService> UpdateProductStockServiceMock;
        protected Mock<ISendSmsService> SendSmsServiceMock;
        protected Mock<ISaveOrUpdateCommand<Alert>> SaveAlertCmdMock;
        protected Mock<IPreconfiguredEmailService> SendEmailServiceMock;
        protected Mock<Outpost> OutpostMock;
        protected ReceivedSmsInputModel InputModel;
        protected Mock<IQueryService<RawSmsReceived>> RawSmsQueryServiceMock;
        protected Mock<IQueryService<Product>> ProductQueryServiceMock;
        protected Mock<ISaveOrUpdateCommand<ProductSale>> SaveProductSaleCmdMock;

        [SetUp]
        public void PerTestSetup()
        {
            RawSmsQueryServiceMock = new Mock<IQueryService<RawSmsReceived>>();
            UpdateProductStockServiceMock = new Mock<IUpdateStockService>();
            SendSmsServiceMock = new Mock<ISendSmsService>();
            SaveAlertCmdMock = new Mock<ISaveOrUpdateCommand<Alert>>();
            SendEmailServiceMock = new Mock<IPreconfiguredEmailService>();
            ProductQueryServiceMock = new Mock<IQueryService<Product>>();
            SaveProductSaleCmdMock = new Mock<ISaveOrUpdateCommand<ProductSale>>();
            Sut = CreateConcreteCommand();

            InputModel = new ReceivedSmsInputModel {Sender = "123"};
        }

        protected abstract StockUpdateMessageCommandBase CreateConcreteCommand();
       

        #endregion

        [Test]
        public abstract void ExecutingTheCommand_UpdatesStockForProducts_WhenMessageParsedSuccesfully();

        [Test]
        public void ExecutingTheCommand_SendsEmailToCentralAccount_WhenSellerMakes2ConsecutiveMistakesInSmsMessage()
        {
            SetupKnownSender();
            SetupTwoConsecutiveIncorrectMessages();
            SendEmailServiceMock.Setup(s => s.CreatePartialMailMessageFromConfig()).Returns(new MailMessage());

            Sut.Execute(InputModel, new SmsParseResult {Success = false}, OutpostMock.Object);

            SendEmailServiceMock.Verify(
                s => s.SendEmail(It.Is<MailMessage>(m => m.Body.Contains(OutpostMock.Object.Name) && m.Body.Contains(InputModel.Sender))));
        }

        [Test]
        public void ExecutingTheCommand_SendsSmsToDistrictManager_WhenSellerMakes2ConsecutiveMistakesInSmsMessage()
        {
            SetupKnownSender();
            SetupTwoConsecutiveIncorrectMessages();
            SendEmailServiceMock.Setup(s => s.CreatePartialMailMessageFromConfig()).Returns(new MailMessage());

            Sut.Execute(InputModel, new SmsParseResult {Success = false}, OutpostMock.Object);

            SendSmsServiceMock.Verify(
                s => s.SendSms(OutpostMock.Object.District.DistrictManager.PhoneNumber, It.Is<string>(m => m.Contains(OutpostMock.Object.Name)), true));
        }

        [Test]
        public void ExecutingTheCommand_DoNotAllowUpdatesOfStock_WhenSenderIsKnownButIsNotActive()
        {
            SetupKnownSender(false);
            var sut = CreatePartialMockedSut();
            sut.Object.Execute(InputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockSale}, OutpostMock.Object);

            UpdateProductStockServiceMock.Verify(s => s.UpdateProductStocksForOutpost(It.IsAny<ISmsParseResult>(), It.IsAny<Guid>(), StockUpdateMethod.SMS),
                                                 Times.Never());
        }

        [Test]
        public void ExecutingTheCommand_SavesAnErrorAlert_WhenMessageIsIncorrect()
        {
            SetupKnownSender();

            Sut.Execute(InputModel, new SmsParseResult {Success = false}, OutpostMock.Object);

            SaveAlertCmdMock.Verify(cmd => cmd.Execute(It.Is<Alert>(a => a.AlertType == AlertType.Error && a.Client == OutpostMock.Object.Client &&
                                                                          a.LowLevelStock == "-" && a.ProductGroupName == "-" &&
                                                                          a.Contact == InputModel.Sender &&
                                                                          a.LastUpdate == null && a.OutpostId == OutpostMock.Object.Id &&
                                                                          a.OutpostName == OutpostMock.Object.Name)));
        }

        [Test]
        public void ExecutingTheCommand_DoesNotSavesAnErrorAlert_WhenMessageIsCorrect()
        {
            SetupKnownSender();

            Sut.Execute(InputModel, new SmsParseResult {Success = true}, OutpostMock.Object);

            SaveAlertCmdMock.Verify(cmd => cmd.Execute(It.IsAny<Alert>()), Times.Never());
        }

        [Test]
        public void
            ExecutingTheCommand_SendsWarningMessageBackToSender_WhenSenderIsMatchedWithAnInactivePhoneNumberExistingInTheSystemAndCorrectUpdateStockMessegeIsParsed
            ()
        {
            SetupKnownSender(isSenderActive: false);

            Sut.Execute(InputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockSale}, OutpostMock.Object);

            SendSmsServiceMock.Verify(s => s.SendSms(InputModel.Sender, It.IsAny<string>(), false));
        }



        [Test]
        public void ExecutingTheCommand_SendWarningSmsBackToSender_WhenThereExistParsedProductGroupsOrProductsCodesThatDoNotExistInTheSystem()
        {
            SetupKnownSender();
            var sut = CreatePartialMockedSut();
            var parseResult = new SmsParseResult {Success = true};
            sut.Setup(s => s.UpdateProductStocksForOutpost(parseResult, OutpostMock.Object))
               .Returns(new StockUpdateResult {Success = false, FailedProducts = new List<IParsedProduct>()});

            sut.Object.Execute(InputModel, parseResult, OutpostMock.Object);

            SendSmsServiceMock.Verify(s => s.SendSms(InputModel.Sender, It.IsAny<string>(), true));
        }

        private Mock<StockUpdateMessageCommandBase> CreatePartialMockedSut()
        {
            return new Mock<StockUpdateMessageCommandBase>(new object[]
                {
                    UpdateProductStockServiceMock.Object, SendSmsServiceMock.Object, SaveAlertCmdMock.Object, SendEmailServiceMock.Object,
                    RawSmsQueryServiceMock.Object
                })
                {
                    CallBase = true
                };
        }

        [Test]
        public void ExecutingTheCommand_SendsErrorMessageBackToSender_WhenTheParsignOfTheMessageFailed()
        {
            SetupKnownSender();

            Sut.Execute(InputModel, new SmsParseResult {Success = false}, OutpostMock.Object);

            SendSmsServiceMock.Verify(s => s.SendSms(It.Is<string>(snd => snd == InputModel.Sender), It.IsAny<string>(), true));
        }

        [Test]
        public void ExecutingTheCommand_SendConfirmationBackToSender_WhenStockUpdateSuccessfull()
        {
            SetupKnownSender();
            var sut = CreatePartialMockedSut();
            var parseResult = new SmsParseResult { Success = true };
            sut.Setup(s => s.UpdateProductStocksForOutpost(parseResult, OutpostMock.Object))
               .Returns(new StockUpdateResult { Success = true, FailedProducts = new List<IParsedProduct>() });

            sut.Object.Execute(InputModel, parseResult, OutpostMock.Object);

            SendSmsServiceMock.Verify(
                s => s.SendSms(It.Is<string>(snd => snd == InputModel.Sender), "Stock updated successfully! Thank you for your message.", true));
        }

        #region Helpers

        private void SetupTwoConsecutiveIncorrectMessages()
        {
            RawSmsQueryServiceMock.Setup(s => s.Query()).Returns(new List<RawSmsReceived>
                {
                    new RawSmsReceived {ParseSucceeded = false, Created = new DateTime(2013, 4, 5), Sender = InputModel.Sender},
                    //the most recent for given sender is the msg just received
                    new RawSmsReceived {ParseSucceeded = true, Created = new DateTime(2013, 4, 6), Sender = "789"},
                    new RawSmsReceived {ParseSucceeded = false, Created = new DateTime(2013, 4, 3), Sender = InputModel.Sender},
                    //the second most recent for given sender is failure
                    new RawSmsReceived {ParseSucceeded = true, Created = new DateTime(2013, 4, 1), Sender = InputModel.Sender},
                }.AsQueryable());
        }

        protected void SetupKnownSender(bool isSenderActive = true, bool isWarehouse = false)
        {
            var contact = new Contact {ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = InputModel.Sender, IsMainContact = isSenderActive};
            OutpostMock = new Mock<Outpost>();
            OutpostMock.Setup(o => o.Id).Returns(Guid.NewGuid());
            OutpostMock.Setup(o => o.Contacts).Returns(new List<Contact> {contact});
            OutpostMock.SetupGet(o => o.Client).Returns(new Client());
            OutpostMock.SetupGet(o => o.Name).Returns("MyOutpostName");
            OutpostMock.SetupGet(o => o.IsWarehouse).Returns(isWarehouse);
            OutpostMock.SetupGet(o => o.District).Returns(new District {DistrictManager = new User {PhoneNumber = "0000"}});
            OutpostMock.Setup(o => o.GetDistrictManagersPhoneNumberAsString()).Returns("0000");
        }

        #endregion
    }
}