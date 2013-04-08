using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using Web.ReceiveSmsUseCase.Controller;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;

namespace Tests.Unit.Controllers.SmsReceiveEndpointControllerTest
{
    [TestFixture]
    public class SmsReceiveEndpointControllerTests
    {
        private Mock<IReceiveSmsWorkflowService> _receiveSmsServiceMock;
        private SmsReceiveEndpointController _sut;

        [SetUp]
        public void PerTestSetup()
        {
            _receiveSmsServiceMock = new Mock<IReceiveSmsWorkflowService>();
            _sut = new SmsReceiveEndpointController(_receiveSmsServiceMock.Object);
        }

        [Test]
        public void ReceiveSms_DelegatesOtherServiceToRealizeTheReceiveSmsUsecase()
        {
            //arange
            _sut = new SmsReceiveEndpointController(_receiveSmsServiceMock.Object);
            var inputModel = new ReceivedSmsInputModel {Sender = "123", Content = "a", InNumber = "7", Email = "a@a.com", Credits = "10"};

            //act
            _sut.ReceiveSms(inputModel);

            //assert
            _receiveSmsServiceMock.Verify(s => s.ProcessSms(inputModel));
        }

        [Test]
        public void ReceiveSms_AlwaysReturnsEmptyResponse()
        {
            //act
            var result = _sut.ReceiveSms(null);

            //assert
            Assert.IsInstanceOf<EmptyResult>(result);
        }
    }
}
