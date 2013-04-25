using Core.Persistence;
using Domain;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.ReceiveSmsUseCase.SmsMessageCommands;
using Web.Services;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class ReceiveSmsWorkflowServiceTests
    {
        private ReceiveSmsWorkflowService _sut;
        private Mock<ISaveOrUpdateCommand<RawSmsReceived>> _saveRawSmsCommandMock;
        private Mock<ISendSmsService> _sendSmsServiceMock;
        private Mock<ISmsTextParserService> _smsTextParserServiceMock;
        private ReceivedSmsInputModel _inputModel;
        private Mock<Outpost> _outpostMock;
        private Mock<ISmsCommandFactory> _smsMessageCommandFactoryMock;
        private Mock<ISenderInformationService> _senderInformationServiceMock;

        [SetUp]
        public void PerTestSetup()
        {
            _senderInformationServiceMock = new Mock<ISenderInformationService>();
            _saveRawSmsCommandMock = new Mock<ISaveOrUpdateCommand<RawSmsReceived>>();
            _sendSmsServiceMock = new Mock<ISendSmsService>();
            _smsTextParserServiceMock = new Mock<ISmsTextParserService>();
            _smsMessageCommandFactoryMock = new Mock<ISmsCommandFactory>();

            _sut = new ReceiveSmsWorkflowService(_saveRawSmsCommandMock.Object, _sendSmsServiceMock.Object, _smsTextParserServiceMock.Object, _smsMessageCommandFactoryMock.Object,
                                                 _senderInformationServiceMock.Object);

            _inputModel = CreateReceivedSmsInputModel();
        }

        [Test]
        public void ProcessSms_SendsWarningMessageBackToSenderAndReturns_WhenSenderIsUnknow()
        {
            _sut.ProcessSms(_inputModel);

            _sendSmsServiceMock.Verify(s => s.SendSms(_inputModel.Sender, It.IsAny<string>(), false));
            _smsTextParserServiceMock.Verify(s => s.Parse(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void ProcessSms_SavesTheSmsWithAnErrorMessage_WhenTheSenderIsUnknown()
        {
            _sut.ProcessSms(_inputModel);

            _saveRawSmsCommandMock.Verify(
                cmd => cmd.Execute(It.Is<RawSmsReceived>(received => received.ParseSucceeded == false && received.ParseErrorMessage == "Sender is unknown.")));
        }

        [Test]
        public void ProcessSms_SavesTheOutpostTypeInTheRawSmsEntry_WhenSenderIsKnown()
        {
            SetupKnownSender(true, true);
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(new SmsParseResult());
            _smsMessageCommandFactoryMock.Setup(f => f.CreateSmsMessageCommand(It.IsAny<MessageType>())).Returns(new NullObjectCommand());

            _sut.ProcessSms(_inputModel);

            _saveRawSmsCommandMock.Verify(cmd => cmd.Execute(It.Is<RawSmsReceived>(sms => sms.OutpostType == OutpostType.Warehouse)));
        }

        [Test]
        public void ProcessSms_ExecutesAMessageCommand_AfterParsing()
        {
            SetupKnownSender(true, true);
            var dummyParseResult = new SmsParseResult();
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(dummyParseResult);
            var smsCommandMock = new Mock<ISmsMessageCommand>();
            _smsMessageCommandFactoryMock.Setup(f => f.CreateSmsMessageCommand(It.IsAny<MessageType>())).Returns(smsCommandMock.Object);

            _sut.ProcessSms(_inputModel);

            smsCommandMock.Verify(c => c.Execute(_inputModel, dummyParseResult, _outpostMock.Object));
        }

        [Test]
        public void ProcessSms_SavesAllTheNecessaryInformation_WhenSavingTheRawSmsInDb()
        {
            SetupKnownSender(true, true);
            _smsTextParserServiceMock.Setup(s => s.Parse(_inputModel.Content)).Returns(new SmsParseResult { Success = false, Message = "a" });
            _smsMessageCommandFactoryMock.Setup(f => f.CreateSmsMessageCommand(It.IsAny<MessageType>())).Returns(new NullObjectCommand());

            _sut.ProcessSms(_inputModel);
            _saveRawSmsCommandMock.Verify(c => c.Execute(It.Is<RawSmsReceived>(m => m.OutpostId == _outpostMock.Object.Id &&
                                                                                    m.OutpostType == OutpostType.Warehouse && m.Sender == _inputModel.Sender &&
                                                                                    m.Content == _inputModel.Content && m.ReceivedDate != DateTime.MinValue &&
                                                                                    m.ParseSucceeded == false && m.ParseErrorMessage == "a")));
        }

        #region Helpers

        private void SetupKnownSender(bool isSenderActive = true, bool isWarehouse = false)
        {
            var contact = new Contact {ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE, ContactDetail = _inputModel.Sender, IsMainContact = isSenderActive};
            _outpostMock = new Mock<Outpost>();
            _outpostMock.Setup(o => o.Id).Returns(Guid.NewGuid());
            _outpostMock.Setup(o => o.Contacts).Returns(new List<Contact> {contact});
            _outpostMock.SetupGet(o => o.Client).Returns(new Client());
            _outpostMock.SetupGet(o => o.Name).Returns("n");
            _outpostMock.SetupGet(o => o.IsWarehouse).Returns(isWarehouse);

            _senderInformationServiceMock.Setup(s => s.GetOutpostWithActiveSender(It.IsAny<string>()))
                                         .Returns(_outpostMock.Object);
        }

        private ReceivedSmsInputModel CreateReceivedSmsInputModel()
        {
            return new ReceivedSmsInputModel {Sender = "123", Content = "a"};
        }

        #endregion
    }
}
