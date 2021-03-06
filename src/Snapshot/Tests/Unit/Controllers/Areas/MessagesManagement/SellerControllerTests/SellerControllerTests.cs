﻿using Domain.Enums;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using Tests.Utils;
using Web.Areas.MessagesManagement.Controllers;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;
using Web.Services;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SellerControllerTests
{
    [TestFixture]
    public class SellerControllerTests
    {
        private SellerController _sut;
        private Mock<IRawSmsMeesageQueryHelpersService> _rawSmsMeesageQueryHelpersServiceMock;


        [SetUp]
        public void PerTestSetup()
        {
            _rawSmsMeesageQueryHelpersServiceMock = new Mock<IRawSmsMeesageQueryHelpersService>();
            _sut = new SellerController {SmsQueryService = _rawSmsMeesageQueryHelpersServiceMock.Object};
        }

        [Test]
        public void Overview_ReturnsTheViewCalledOverview()
        {
            var viewResult = _sut.Overview() as ViewResult;

            Assert.NotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("Overview"));
        }

        [Test]
        public void GetMessagesFromSeller_DelegatesQueryToHelperService_WithSellerSpecificParameter()
        {
            var inputModel = new IndexTableInputModel();

            _sut.GetMessagesFromSeller(inputModel);

            _rawSmsMeesageQueryHelpersServiceMock.Verify(service => service.GetMessagesFromOutpost(inputModel, OutpostType.Seller, It.IsAny<System.Guid>()));
        }

        [Test]
        public void GetMessagesFromSeller_ReturnsAsJsonTheOutputFromTheHelperService()
        {
            _rawSmsMeesageQueryHelpersServiceMock.Setup(service => service.GetMessagesFromOutpost(It.IsAny<IndexTableInputModel>(), OutpostType.Seller, It.IsAny<System.Guid>()))
                                                 .Returns(new StoreOutputModel<MessageModel> { TotalItems = 1 });

            var result = _sut.GetMessagesFromSeller(new IndexTableInputModel());

            Assert.That(result.GetValueFromJsonResultForModel<StoreOutputModel<MessageModel>, int>(m => m.TotalItems), Is.EqualTo(1));
        }
    }
}
