using System.Net.Mail;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.Services.Configuration;
using Web.Services.SendEmail;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class PreconfiguredEmailServiceTests
    {
        private PreconfiguredEmailService _sut;
        private Mock<IConfigurationService> _configurationServiceMock;
        private Mock<IEmailSendingService> _emailSendingServiceMock;
        private Mock<ISaveOrUpdateCommand<ApplicationActivity>> _saveActivityCmdMock;

        [SetUp]
        public void PerTestSetup()
        {
            _emailSendingServiceMock = new Mock<IEmailSendingService>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _saveActivityCmdMock = new Mock<ISaveOrUpdateCommand<ApplicationActivity>>();
            _sut = new PreconfiguredEmailService(_emailSendingServiceMock.Object, _configurationServiceMock.Object,_saveActivityCmdMock.Object);
        }

        [Test]
        public void AterEachEmailSentSuccesfully_ActivityIsRecordedInDB_WithSummaryMessage()
        {
            _configurationServiceMock.Setup(s => s.Keys).Returns(new ConfigurationKeys());
            _configurationServiceMock.Setup(s => s[It.IsAny<string>()]).Returns(string.Empty);
            _emailSendingServiceMock.Setup(s => s.SendEmail(It.IsAny<MailMessage>(), It.IsAny<SmtpServerDetails>())).Returns(true);

            var result = _sut.SendEmail(new MailMessage());

            _saveActivityCmdMock.Verify(s => s.Execute(It.Is<ApplicationActivity>(a => !string.IsNullOrEmpty(a.Message))));
            Assert.IsTrue(result);
        }
    }
}
