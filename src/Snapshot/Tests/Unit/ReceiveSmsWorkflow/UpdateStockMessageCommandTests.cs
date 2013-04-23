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

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class UpdateStockMessageCommandTests
    {
        private UpdateStockMessageCommand _sut;
        private Mock<IUpdateStockService> _updateProductStockServiceMock;
        private Mock<ISendSmsService> _sendSmsServiceMock;
        private Mock<ISaveOrUpdateCommand<Alert>> _saveAlertCmdMock;
        private Mock<IPreconfiguredEmailService> _sendEmailServiceMock;
        private Mock<Outpost> _outpostMock;
        private ReceivedSmsInputModel _inputModel;
        private Mock<IQueryService<RawSmsReceived>> _rawSmsQueryServiceMock;

        [SetUp]
        public void PerTestSetup()
        {
            _rawSmsQueryServiceMock = new Mock<IQueryService<RawSmsReceived>>();
            _updateProductStockServiceMock = new Mock<IUpdateStockService>();
            _sendSmsServiceMock = new Mock<ISendSmsService>();
            _saveAlertCmdMock = new Mock<ISaveOrUpdateCommand<Alert>>();
            _sendEmailServiceMock = new Mock<IPreconfiguredEmailService>();
            new Mock<IQueryService<Alert>>();
            _sut = new UpdateStockMessageCommand(_updateProductStockServiceMock.Object, _sendSmsServiceMock.Object, _saveAlertCmdMock.Object,
                                                 _sendEmailServiceMock.Object, _rawSmsQueryServiceMock.Object);

            _inputModel = new ReceivedSmsInputModel { Sender = "123" };
        }

        [Test]
        public void ExecutingTheCommand_SendsEmailToCentralAccountWithDetailsfromConfigurationFile_WhenSellerMakes2ConsecutiveMistakesInSmsMessage()
        {
            _rawSmsQueryServiceMock.Setup(s => s.Query()).Returns(new List<RawSmsReceived>
                {
                    new RawSmsReceived {ParseSucceeded = false, Created = new DateTime(2013, 4, 5), Sender = "1234567"},//the most recent for given sender is the msg just received
                    new RawSmsReceived {ParseSucceeded = true, Created = new DateTime(2013, 4, 6), Sender = "789"},
                    new RawSmsReceived {ParseSucceeded = false, Created = new DateTime(2013, 4, 3), Sender = "1234567"},//the second most recent for given sender is failure
                    new RawSmsReceived {ParseSucceeded = true, Created = new DateTime(2013, 4, 1), Sender = "1234567"},
                }.AsQueryable());


            _sendEmailServiceMock.Setup(s => s.CreatePartialMailMessageFromConfig()).Returns(new MailMessage());


            _sut.Execute(new ReceivedSmsInputModel {Sender = "1234567"}, new SmsParseResult {Success = false}, new Outpost {Name = "abcdefg"});

            _sendEmailServiceMock.Verify(s => s.SendEmail(It.Is<MailMessage>(m => m.Body.Contains("abcdefg") && m.Body.Contains("1234567"))));

        }

        [Test]
        public void ExecutingTheCommand_DoNotAllowUpdatesOfStock_WhenSenderIsKnownButIsNotActive()
        {
            SetupKnownSender(false);

            _sut.Execute(_inputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockUpdate}, _outpostMock.Object);

            _updateProductStockServiceMock.Verify(s => s.UpdateProductStocksForOutpost(It.IsAny<ISmsParseResult>(), It.IsAny<Guid>(), StockUpdateMethod.SMS),
                                                  Times.Never());
        }



        [Test]
        public void ExecutingTheCommand_SavesAnErrorAlert_WhenMessageIsIncorrect()
        {
            SetupKnownSender();

            _sut.Execute(_inputModel, new SmsParseResult {Success = false}, _outpostMock.Object);

            _saveAlertCmdMock.Verify(cmd => cmd.Execute(It.Is<Alert>(a => a.AlertType == AlertType.Error && a.Client == _outpostMock.Object.Client &&
                                                                          a.LowLevelStock == "-" && a.ProductGroupName == "-" &&
                                                                          a.Contact == _inputModel.Sender &&
                                                                          a.LastUpdate == null && a.OutpostId == _outpostMock.Object.Id &&
                                                                          a.OutpostName == _outpostMock.Object.Name)));
        }

        [Test]
        public void ExecutingTheCommand_DoesNotSavesAnErrorAlert_WhenMessageIsCorrect()
        {
            SetupKnownSender();

            _sut.Execute(_inputModel, new SmsParseResult {Success = true}, _outpostMock.Object);

            _saveAlertCmdMock.Verify(cmd => cmd.Execute(It.IsAny<Alert>()), Times.Never());
        }

        [Test]
        public void
            ExecutingTheCommand_SendsWarningMessageBackToSender_WhenSenderIsMatchedWithAnInactivePhoneNumberExistingInTheSystemAndCorrectUpdateStockMessegeIsParsed
            ()
        {
            SetupKnownSender(isSenderActive: false);

            _sut.Execute(_inputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockUpdate}, _outpostMock.Object);

            _sendSmsServiceMock.Verify(s => s.SendSms(_inputModel.Sender, It.IsAny<string>(), false));
        }

        [Test]
        public void ExecutingTheCommand_UpdatesStockForProducts_WhenMessageParsedSuccesfully()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};

            _sut.Execute(_inputModel, parseResult, _outpostMock.Object);

            _updateProductStockServiceMock.Verify(s => s.UpdateProductStocksForOutpost(parseResult, _outpostMock.Object.Id, StockUpdateMethod.SMS));
        }

        [Test]
        public void ExecutingTheCommand_SendWarningSmsBackToSender_WhenThereExistParsedProductGroupsOrProductsCodesThatDoNotExistInTheSystem()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};
            _updateProductStockServiceMock.Setup(s => s.UpdateProductStocksForOutpost(parseResult, _outpostMock.Object.Id, StockUpdateMethod.SMS))
                                          .Returns(new StockUpdateResult {Success = false, FailedProducts = new List<IParsedProduct>()});

            _sut.Execute(_inputModel, parseResult, _outpostMock.Object);

            _sendSmsServiceMock.Verify(s => s.SendSms(_inputModel.Sender, It.IsAny<string>(), true));
        }

        [Test]
        public void ExecutingTheCommand_SendsErrorMessageBackToSender_WhenTheParsignOfTheMessageFailed()
        {
            SetupKnownSender();

            _sut.Execute(_inputModel, new SmsParseResult {Success = false}, _outpostMock.Object);

            _sendSmsServiceMock.Verify(s => s.SendSms(It.Is<string>(snd => snd == _inputModel.Sender), It.IsAny<string>(), true));
        }

        private void SetupKnownSender(bool isSenderActive = true, bool isWarehouse = false)
        {
            var contact = new Contact {ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = _inputModel.Sender, IsMainContact = isSenderActive};
            _outpostMock = new Mock<Outpost>();
            _outpostMock.Setup(o => o.Id).Returns(Guid.NewGuid());
            _outpostMock.Setup(o => o.Contacts).Returns(new List<Contact> {contact});
            _outpostMock.SetupGet(o => o.Client).Returns(new Client());
            _outpostMock.SetupGet(o => o.Name).Returns("MyOutpostName");
            _outpostMock.SetupGet(o => o.IsWarehouse).Returns(isWarehouse);
        }
    }
}