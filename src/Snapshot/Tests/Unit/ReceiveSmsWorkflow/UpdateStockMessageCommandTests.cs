using Core.Persistence;
using Domain;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.ReceiveSmsUseCase.SmsMessageCommands;
using Web.Services.Configuration;
using Web.Services.SendEmail;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class UpdateStockMessageCommandTests
    {
        private UpdateStockMessageCommand _sut;
        private Mock<IUpdateStockService> _updateProductStockServiceMock;
        private Mock<ISendSmsService> _sendSmsServiceMock;
        private Mock<ISaveOrUpdateCommand<Alert>> _saveAlertCmdMock;
        private Mock<IEmailSendingService> _sendEmailServiceMock;
        private Mock<IQueryService<Alert>> _alertQueryServiceMock;
        private Mock<IConfigurationService> _configurationServiceMock;

        private Mock<Outpost> _outpostMock;
        private ReceivedSmsInputModel _inputModel;

        [SetUp]
        public void PerTestSetup()
        {
            _updateProductStockServiceMock = new Mock<IUpdateStockService>();
            _sendSmsServiceMock = new Mock<ISendSmsService>();
            _saveAlertCmdMock = new Mock<ISaveOrUpdateCommand<Alert>>();
            _sendEmailServiceMock = new Mock<IEmailSendingService>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _alertQueryServiceMock = new Mock<IQueryService<Alert>>();
            _sut = new UpdateStockMessageCommand(_updateProductStockServiceMock.Object, _sendSmsServiceMock.Object, _saveAlertCmdMock.Object,
                                                 _sendEmailServiceMock.Object, _configurationServiceMock.Object, _alertQueryServiceMock.Object);

            _inputModel = new ReceivedSmsInputModel {Sender = "123"};
        }

        [Test]
        public void ExecutingTheCommand_SendsEmailToCentralAccountWithDetailsfromConfigurationFile_WhenSellerMakes2ConsecutiveMistakesInSmsMessage()
        {
            _alertQueryServiceMock.Setup(s => s.Query())
                                  .Returns(
                                      new List<Alert>
                                          {
                                              new Alert
                                                  {
                                                      AlertType = AlertType.StockLevel,
                                                      Created = new DateTime(2013, 1, 3),
                                                      OutpostName = "a",
                                                      Contact = "123"
                                                  },
                                              new Alert {AlertType = AlertType.Error, Created = new DateTime(2013, 1, 7), OutpostName = "a", Contact = "123"},
                                              new Alert
                                                  {
                                                      AlertType = AlertType.StockLevel,
                                                      Created = new DateTime(2013, 1, 5),
                                                      OutpostName = "a",
                                                      Contact = "123"
                                                  }
                                          }.AsQueryable());

            _configurationServiceMock.SetupGet(s => s.Keys).Returns(new ConfigurationKeys());
            _configurationServiceMock.SetupGet(s => s["SendEmail.Host"]).Returns("a@a.com");
            _configurationServiceMock.SetupGet(s => s["SendEmail.Port"]).Returns("25");
            _configurationServiceMock.SetupGet(s => s["SendEmail.From"]).Returns("b@b.com");
            _configurationServiceMock.SetupGet(s => s["SendEmail.Password"]).Returns("pass");
            _configurationServiceMock.SetupGet(s => s["SendEmail.To"]).Returns("me@me.com");
            _configurationServiceMock.SetupGet(s => s["SendEmail.CC"]).Returns("you@you.com");


            _sut.Execute(new ReceivedSmsInputModel {Sender = "123"}, new SmsParseResult {Success = false}, new Outpost {Name = "a"});

            _sendEmailServiceMock.Verify(
                s => s.SendEmail(It.Is<MailMessage>(m => m.To.FirstOrDefault(adr => adr.Address == "me@me.com") != null &&
                                                         m.CC.FirstOrDefault(adr => adr.Address == "you@you.com") != null &&
                                                         m.From.Address == "b@b.com"),
                                 It.Is<SmtpServerDetails>(c => c.FromAddress == "b@b.com" && c.FromPassword == "pass" && c.Port == 25 && c.Host == "a@a.com")));
        }

        [Test]
        public void ExecutingTheCommand_DoNotAllowUpdatesOfStock_WhenSenderIsKnownButIsNotActive()
        {
            SetupKnownSender(false);

            _sut.Execute(_inputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockUpdate}, _outpostMock.Object);

            _updateProductStockServiceMock.Verify(s => s.UpdateProductStocksForOutpost(It.IsAny<ISmsParseResult>(), It.IsAny<Guid>()), Times.Never());
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
        public void
            ExecutingTheCommand_SendsWarningMessageBackToSender_WhenSenderIsMatchedWithAnInactivePhoneNumberExistingInTheSystemAndCorrectUpdateStockMessegeIsParsed
            ()
        {
            SetupKnownSender(isSenderActive: false);

            _sut.Execute(_inputModel, new SmsParseResult {Success = true, MessageType = MessageType.StockUpdate}, _outpostMock.Object);

            _sendSmsServiceMock.Verify(s => s.SendSmsMessage(It.IsAny<string>(), _inputModel.Sender));
        }

        [Test]
        public void ExecutingTheCommand_UpdatesStockForProducts_WhenMessagePrasedSuccesfully()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};

            _sut.Execute(_inputModel, parseResult, _outpostMock.Object);

            _updateProductStockServiceMock.Verify(s => s.UpdateProductStocksForOutpost(parseResult, _outpostMock.Object.Id));
        }

        private void SetupKnownSender(bool isSenderActive = true, bool isWarehouse = false)
        {
            var contact = new Contact {ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = _inputModel.Sender, IsMainContact = isSenderActive};
            _outpostMock = new Mock<Outpost>();
            _outpostMock.Setup(o => o.Id).Returns(Guid.NewGuid());
            _outpostMock.Setup(o => o.Contacts).Returns(new List<Contact> {contact});
            _outpostMock.SetupGet(o => o.Client).Returns(new Client());
            _outpostMock.SetupGet(o => o.Name).Returns("n");
            _outpostMock.SetupGet(o => o.IsWarehouse).Returns(isWarehouse);
        }
    }
}