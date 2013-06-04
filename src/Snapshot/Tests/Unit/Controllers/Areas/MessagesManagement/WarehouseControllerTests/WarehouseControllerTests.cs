using System.Web.Mvc;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using Tests.Utils;
using Web.Areas.MessagesManagement.Controllers;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;
using Web.Services;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.WarehouseControllerTests
{
    [TestFixture]
    public class WarehouseControllerTests
    {
        private WarehouseController _sut;
        private Mock<IRawSmsMeesageQueryHelpersService> _rawSmsMeesageQueryHelpersServiceMock;


        [SetUp]
        public void PerTestSetup()
        {
            _rawSmsMeesageQueryHelpersServiceMock = new Mock<IRawSmsMeesageQueryHelpersService>();
            _sut = new WarehouseController {SmsQueryService = _rawSmsMeesageQueryHelpersServiceMock.Object};
        }

        [Test]
        public void Overview_ReturnsTheViewCalledOverview()
        {
            var viewResult = _sut.Overview() as ViewResult;

            Assert.NotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("Overview"));
        }

        [Test]
        public void GetMessagesFromWarehouse_DelegatesQueryToHelperService_WithWarehouseSpecificParameter()
        {
            var inputModel = new IndexTableInputModel();

            _sut.GetMessagesFromWarehouse(inputModel);

            _rawSmsMeesageQueryHelpersServiceMock.Verify(service => service.GetMessagesFromOutpost(inputModel, OutpostType.Warehouse, It.IsAny<System.Guid>()));
        }

        [Test]
        public void GetMessagesFromWarehouse_ReturnsAsJsonTheOutputFromTheHelperService()
        {
            _rawSmsMeesageQueryHelpersServiceMock.Setup(service => service.GetMessagesFromOutpost(It.IsAny<IndexTableInputModel>(), OutpostType.Warehouse, It.IsAny<System.Guid>()))
                                                 .Returns(new MessageIndexOuputModel {TotalItems = 1});

            var result = _sut.GetMessagesFromWarehouse(new IndexTableInputModel());

            Assert.That(result.GetValueFromJsonResultForModel<MessageIndexOuputModel, int>(m => m.TotalItems), Is.EqualTo(1));
        }
    }
}
