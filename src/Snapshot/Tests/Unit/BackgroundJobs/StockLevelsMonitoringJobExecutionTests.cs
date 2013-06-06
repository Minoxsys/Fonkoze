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
using Web.Services.SendEmail;

namespace Tests.Unit.BackgroundJobs
{
    [TestFixture]
    class StockLevelsMonitoringJobExecutionTests
    {
        StockLevelsMonitoringJobExecutionService _sut;

        Mock<ISaveOrUpdateCommand<Alert>> _saveOrUpdateCommand;
        Mock<IQueryService<Alert>> _queryAlerts;
        Mock<IQueryService<OutpostStockLevel>> _queryOutpostStockLevel;
        Mock<IQueryService<Client>> _queryClients;
        Mock<IPreconfiguredEmailService> _preconfiguredEmailService;
        Mock<ISendSmsService> _sendSmsService;

        [SetUp]
        public void PerTestSetup()
        {
            _saveOrUpdateCommand = new Mock<ISaveOrUpdateCommand<Alert>>();
            _queryAlerts = new Mock<IQueryService<Alert>>();
            _queryOutpostStockLevel = new Mock<IQueryService<OutpostStockLevel>>();
            _queryClients = new Mock<IQueryService<Client>>();
            _preconfiguredEmailService = new Mock<IPreconfiguredEmailService>();
            _sendSmsService = new Mock<ISendSmsService>();

            _sut = new StockLevelsMonitoringJobExecutionService(_saveOrUpdateCommand.Object, _queryAlerts.Object, _queryOutpostStockLevel.Object,
                _queryClients.Object, _preconfiguredEmailService.Object, _sendSmsService.Object);
        }

        [Test]
        public void ExecuteJob_SavesOrUpdatesAlert_WhenValidOutpostStockLevelAndAlertDoesNotExist()
        {
            // Arrange
            InitializeServices();
            // Act
            _sut.ExecuteJob();

            // Assert
            _saveOrUpdateCommand.Verify(v => v.Execute(It.Is<Alert>(outpost => outpost.OutpostName == "TestName")));
        }

        [Test]
        public void ExecuteJob_SendEmailToCentralAccount_WhenValidOutpostStockLevelAndAlertDoesNotExist()
        {
            // Arrange
            var mailMessage = InitializeServices();
            // Act
            _sut.ExecuteJob();

            // Assert
            _preconfiguredEmailService.Verify(v => v.SendEmail(It.Is<MailMessage>(m => m == mailMessage)));
        }

        [Test]
        public void ExecuteJob_SendSmsMessageToDistrictManager_WhenValidOutpostStockLevelAndAlertDoesNotExist()
        {
            // Arrange
            InitializeServices();
            // Act
            _sut.ExecuteJob();

            // Assert
            _sendSmsService.Verify(v => v.SendSms(It.Is<string>(phno => phno == "232131"), It.Is<string>(m => m == BuildSmsMessage()), true));
        }

        [Test]
        public void ExecuteJob_DoesNothing_WhenValidOutpostStockLevelAndAlertExists()
        {
            // Arrange
            InitializeServices();
            _queryAlerts.Setup(s => s.Query()).Returns(new List<Alert>{ new Alert {OutpostStockLevelId=Guid.Empty, Created = DateTime.Now }}.AsQueryable());
            // Act
            _sut.ExecuteJob();

            // Assert
            _sendSmsService.Verify(v => v.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
        }

        #region Helpers
        private string BuildSmsMessage()
        {
            return "LOW STOCK: Seller: TestName \r\nProduct:  \r\nStock: 1 \r\n \r\nContact: 322424";
        }

        private MailMessage InitializeServices()
        {
            var mailMessage = new MailMessage();

            _queryOutpostStockLevel.Setup(s => s.Query()).Returns(new List<OutpostStockLevel> { CreateOutpostStockLevel() }.AsQueryable());
            _preconfiguredEmailService.Setup(s => s.CreatePartialMailMessageFromConfig()).Returns(mailMessage);

            return mailMessage;
        }

        private OutpostStockLevel CreateOutpostStockLevel()
        {
            return new OutpostStockLevel
            {
                Created = DateTime.MinValue,
                Client = new Client(),
                ProductGroup = new ProductGroup(),
                Outpost = CreateOutpost(),
                Updated = DateTime.Now.AddMinutes(-30),
                Product = CreateProduct(),
                StockLevel = 1
            };
        }

        private Product CreateProduct()
        {
            return new Product
            {
                LowerLimit = 5
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
