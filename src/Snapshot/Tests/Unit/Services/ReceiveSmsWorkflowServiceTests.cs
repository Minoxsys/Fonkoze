using Core.Persistence;
using Domain;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class ReceiveSmsWorkflowServiceTests
    {
        private ReceiveSmsWorkflowService _sut;
        private Mock<ISaveOrUpdateCommand<RawSmsReceived>> _saveRawSmsCommandMock;
        private Mock<ISendSmsService> _sendSmsServiceMock;
        private Mock<IQueryService<Outpost>> _outpostsQueryServiceMock;
        private Mock<IQueryService<Contact>> _contactQueryServiceMock;
        private Mock<ISmsTextParserService> _smsTextParserServiceMock;
        private Mock<IUpdateStockService> _updateStockServiceMock;
        private Mock<ISaveOrUpdateCommand<Alert>> _saveAlertCommandMock;
        private ReceivedSmsInputModel _inputModel;
        private Mock<Outpost> _outpostMock;

        [SetUp]
        public void PerTestSetup()
        {
            _saveRawSmsCommandMock = new Mock<ISaveOrUpdateCommand<RawSmsReceived>>();
            _sendSmsServiceMock = new Mock<ISendSmsService>();
            _outpostsQueryServiceMock = new Mock<IQueryService<Outpost>>();
            _contactQueryServiceMock = new Mock<IQueryService<Contact>>();
            _smsTextParserServiceMock = new Mock<ISmsTextParserService>();
            _updateStockServiceMock = new Mock<IUpdateStockService>();
            _saveAlertCommandMock = new Mock<ISaveOrUpdateCommand<Alert>>();
            _sut = new ReceiveSmsWorkflowService(_saveRawSmsCommandMock.Object, _sendSmsServiceMock.Object, _outpostsQueryServiceMock.Object,
                                         _contactQueryServiceMock.Object, _smsTextParserServiceMock.Object, _updateStockServiceMock.Object, _saveAlertCommandMock.Object);

            _inputModel = CreateReceivedSmsInputModel();
        }

        [Test]
        public void ProcessSms_SavesTheFirstDraftOfTheSmsDataReceivedAsIsSettingReceivedDate()
        {
            _sut.ProcessSms(_inputModel);

            _saveRawSmsCommandMock.Verify(
                cmd => cmd.Execute(
                    It.Is<RawSmsReceived>(sms => sms.Sender == _inputModel.Sender && sms.Content == _inputModel.Content && sms.ReceivedDate != DateTime.MinValue)));
        }

        [Test]
        public void ProcessSms_SendsWarningMessageBackToSenderAndReturns_WhenSenderIsUnknow()
        {
            _sut.ProcessSms(_inputModel);

            _sendSmsServiceMock.Verify(s => s.SendSmsMessage(It.IsAny<string>(), _inputModel.Sender));
            _smsTextParserServiceMock.Verify(s => s.Parse(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void ProcessSms_UpdatesMessageDetailsInDbWithParsingResults()
        {
            SetupKnownSender();
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(new SmsParseResult {Message = "a", Success = true});

            _sut.ProcessSms(_inputModel);

            _saveRawSmsCommandMock.Verify(cmd => cmd.Execute(It.Is<RawSmsReceived>(sms => sms.ParseSucceeded && sms.ParseErrorMessage == "a")));
        }

        [Test]
        public void ProcessSms_UpdatesStockForProducts_WhenMessagePrasedSuccesfully()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = true};
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(parseResult);

            _sut.ProcessSms(_inputModel);

            _updateStockServiceMock.Verify(s => s.UpdateProductStocks(parseResult));
        }

        [Test]
        public void ProcessSms_SavesAnErrorAlert_WhenMessageIsIncorrect()
        {
            SetupKnownSender();
            var parseResult = new SmsParseResult {Success = false};
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(parseResult);

            _sut.ProcessSms(_inputModel);

            _saveAlertCommandMock.Verify(cmd => cmd.Execute(It.Is<Alert>(a => a.AlertType == AlertType.Error && a.Client == _outpostMock.Object.Client &&
                                                                              a.LowLevelStock == "-" && a.ProductGroupName == "-" &&
                                                                              a.Contact == _inputModel.Sender &&
                                                                              a.LastUpdate == null && a.OutpostId == _outpostMock.Object.Id &&
                                                                              a.OutpostName == _outpostMock.Object.Name)));
        }

        private ReceivedSmsInputModel CreateReceivedSmsInputModel()
        {
            return new ReceivedSmsInputModel {Sender = "123", Content = "a"};
        }

        private void SetupKnownSender()
        {
            var contact = new Contact { ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = _inputModel.Sender };
            _outpostMock = new Mock<Outpost>();
            _outpostMock.Setup(o => o.Id).Returns(Guid.NewGuid());
            _outpostMock.Setup(o => o.Contacts).Returns(new List<Contact> { contact });
            _outpostMock.SetupGet(o => o.Client).Returns(new Client());
            _outpostMock.SetupGet(o => o.Name).Returns("n");

            _contactQueryServiceMock.Setup(qs => qs.Query())
                                    .Returns(new List<Contact> { contact }.AsQueryable());
            _outpostsQueryServiceMock.Setup(qs => qs.Query())
                                     .Returns(new List<Outpost> { _outpostMock.Object }.AsQueryable());
        }
    }
}
