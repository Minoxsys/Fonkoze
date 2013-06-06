using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.BackgroundJobs.BackgroundJobsServices;
using Web.Services;
using Web.Services.Configuration;
using Web.Services.SendEmail;

namespace Tests.Unit.BackgroundJobs
{
    [TestFixture]
    class OutpostInactivityJobExecutionTests
    {
        OutpostInactivityJobExecutionService _sut;

        Mock<IPreconfiguredEmailService> _preconfiguredEmailService;
        Mock<IConfigurationService> _configurationService;
        Mock<IQueryService<OutpostStockLevel>> _queryOutpostStockLevel;
        Mock<ISendSmsService> _sendSmsService;
        Mock<ISaveOrUpdateCommand<Alert>> _alertSaveOrUpdateCommand;

        [SetUp]
        public void PerTestSetup()
        {
            _preconfiguredEmailService = new Mock<IPreconfiguredEmailService>();
            _configurationService = new Mock<IConfigurationService>();
            _queryOutpostStockLevel = new Mock<IQueryService<OutpostStockLevel>>();
            _sendSmsService = new Mock<ISendSmsService>();
            _alertSaveOrUpdateCommand = new Mock<ISaveOrUpdateCommand<Alert>>();

            _sut = new OutpostInactivityJobExecutionService(_queryOutpostStockLevel.Object, _preconfiguredEmailService.Object,
                _configurationService.Object, _sendSmsService.Object, _alertSaveOrUpdateCommand.Object);
        }

        [Test]
        public void ExecuteJob_DoesNothing_WhenThereAreNoInactiveOutposts()
        {
            // Arrange
            _queryOutpostStockLevel.Setup(s => s.Query()).Returns(new List<OutpostStockLevel>().AsQueryable());

            // Act
            _sut.ExecuteJob();

            // Assert
            _preconfiguredEmailService.Verify(s => s.SendEmail(It.IsAny<MailMessage>()), Times.Never());
            _sendSmsService.Verify(s => s.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            _alertSaveOrUpdateCommand.Verify(s=>s.Execute(It.IsAny<Alert>()), Times.Never());
        }

        [Test]
        public void ExecuteJob_NotifiesCentralAccount_WhenThereAreInactiveOutposts()
        {
            //Arrange
            var mailMessage = InitializeServices();

            //Act
            _sut.ExecuteJob();

            //Assert
            _preconfiguredEmailService.Verify(s => s.SendEmail(It.Is<MailMessage>(mail => mail == mailMessage)));
        }

        [Test]
        public void ExecuteJob_NotifiesDistrictManagers_WhenThereAreInactiveOutposts()
        {
            //Arrange
            InitializeServices();

            //Act
            _sut.ExecuteJob();

            //Assert
            _sendSmsService.Verify(s => s.SendSms(It.Is<string>(phNo => phNo == "232131"), It.IsAny<string>(), It.Is<bool>(saveMsg => saveMsg == true)));
        }

        [Test]
        public void ExecuteJob_PostsAlerts_WhenThereAreInactiveOutposts()
        {
            //Arrange
            InitializeServices();

            //Act
            _sut.ExecuteJob();

            //Assert
            _alertSaveOrUpdateCommand.Verify(s => s.Execute(It.IsAny<Alert>()));
        }

        #region Helpers
        private MailMessage InitializeServices()
        {
            var mailMessage = new MailMessage();

            _queryOutpostStockLevel.Setup(s => s.Query()).Returns(new List<OutpostStockLevel> { CreateOutpostStockLevel() }.AsQueryable());
            _configurationService.Setup(s => s.Keys).Returns(new ConfigurationKeys());
            _preconfiguredEmailService.Setup(s => s.CreatePartialMailMessageFromConfig()).Returns(mailMessage);

            return mailMessage;
        }

        private OutpostStockLevel CreateOutpostStockLevel()
        {
            return new OutpostStockLevel
            {
                Outpost = CreateOutpost(),
                Updated = DateTime.MinValue,
            };
        }

        private Outpost CreateOutpost()
        {
            return new Outpost
            {
                Name = "TestName",
                DetailMethod = "322424",
                District = CreateDistrict()
            };
        }

        private District CreateDistrict()
        {
            return new District
            {
                DistrictManager = CreateDistrictManager()
            };
        }

        private Core.Domain.User CreateDistrictManager()
        {
            return new Core.Domain.User
            {
                PhoneNumber = "232131"
            };
        }
        #endregion   
    }
}
