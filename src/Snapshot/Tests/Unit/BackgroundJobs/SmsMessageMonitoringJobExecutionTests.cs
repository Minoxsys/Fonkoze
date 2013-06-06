using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.BackgroundJobs.BackgroundJobsServices;
using Web.Services;
using Web.Services.Helper;

namespace Tests.Unit.BackgroundJobs
{
    [TestFixture]
    class SmsMessageMonitoringJobExecutionTests
    {
        private SmsMessageMonitoringJobExecutionService _sut;
        Mock<IQueryService<RawSmsReceived>> _rawSmsQueryService;
        Mock<ISendSmsService> _sendSmsService;
        Mock<ISenderInformationService> _senderInformationService;

        [SetUp]
        public void PerTestSetup()
        {
            _rawSmsQueryService = new Mock<IQueryService<RawSmsReceived>>();
            _sendSmsService = new Mock<ISendSmsService>();
            _senderInformationService = new Mock<ISenderInformationService>();

            _sut = new SmsMessageMonitoringJobExecutionService(_rawSmsQueryService.Object, _sendSmsService.Object, _senderInformationService.Object);
        }

        [Test]
        public void ExecuteJob_SendsSMS_WhenOutpostAndRawSmsAreValid()
        {
            // Arrange
            var rawSmsList = CreateRawSmsList();
            var outpost = CreateOutpost();
            _rawSmsQueryService.Setup(s => s.Query()).Returns(rawSmsList.AsQueryable());
            _senderInformationService.Setup(s => s.GetOutpostWithActiveSender(It.IsAny<string>())).Returns(outpost);

            // Act
            _sut.ExecuteJob();

            // Assert
            _sendSmsService.Verify(v => v.SendSms((It.Is<string>(phNo => phNo== outpost.District.DistrictManager.PhoneNumber)),
                It.Is<string>(msg => msg == ReturnComposedMessage(rawSmsList.FirstOrDefault())), true));
        }


        [Test]
        public void ExecuteJob_DoesNothing_WhenOutpostIsNull()
        {
            // Arrange
            _rawSmsQueryService.Setup(s => s.Query()).Returns(CreateRawSmsList().AsQueryable());

            // Act
            _sut.ExecuteJob();

            // Assert
            _sendSmsService.Verify(v => v.SendSms(It.IsAny<string>(), It.IsAny<string>(), true), Times.Never());
        }

        private IList<RawSmsReceived> CreateRawSmsList()
        {
            return new List<RawSmsReceived>
            {
                new RawSmsReceived
                {
                    Created = DateTime.Now.AddHours(-7),
                    Sender = "TestSender"
                }
            };
        }

        #region Helpers
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

        private string ReturnComposedMessage(RawSmsReceived sms)
        {
            return string.Format("Seller: TestName(322424). Last message sent invalid and no follow-up in over 4 hours. Message sent at {0}", sms.Created);
        }
        #endregion
    }
}
