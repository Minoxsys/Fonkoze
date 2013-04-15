using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.ReceiveSmsUseCase.SmsMessageCommands;

namespace Tests.Unit.ReceiveSmsWorkflow
{
    [TestFixture]
    public class ActivationMessageCommandTests
    {
        private ActivationMessageCommand _sut;
        private Mock<IContactMethodsService> _contactMehtodsServiceMock;

        [SetUp]
        public void PerTestSetup()
        {
            _contactMehtodsServiceMock = new Mock<IContactMethodsService>();
            _sut = new ActivationMessageCommand(_contactMehtodsServiceMock.Object);
        }

        [Test]
        public void ExecutingTheCommand_ActivatesSenderPhoneNumber_WhenMessageTypeIsActivationAndSuccesfullParse()
        {
            var outpost = new Outpost();
           
            _sut.Execute(new ReceivedSmsInputModel {Sender = "123"}, new SmsParseResult {Success = true, MessageType = MessageType.Activation}, outpost);

            _contactMehtodsServiceMock.Verify(s => s.ActivatePhoneNumber("123", outpost));
        }

        [Test]
        public void ExecutingTheCommand_DoesNothing_WhenFailedParse()
        {
            _sut.Execute(new ReceivedSmsInputModel(), new SmsParseResult { Success = false }, new Outpost());

            _contactMehtodsServiceMock.Verify(s => s.ActivatePhoneNumber(It.IsAny<string>(), It.IsAny<Outpost>()), Times.Never());
        }

    }
}
