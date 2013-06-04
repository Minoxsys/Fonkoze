using Core.Persistence;
using Domain;
using Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;
using Web.Services;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class RawSmsMeesageQueryHelpersService
    {
        private IRawSmsMeesageQueryHelpersService _sut;
        private Mock<IQueryService<RawSmsReceived>> _rawSmsQueryMock;
        private Mock<IQueryService<Outpost>> _outpostQueryMock;

        [SetUp]
        public void PerTestSetup()
        {
            _rawSmsQueryMock = new Mock<IQueryService<RawSmsReceived>>();
            _outpostQueryMock = new Mock<IQueryService<Outpost>>();
            _sut = new Web.Services.RawSmsMeesageQueryHelpersService(_rawSmsQueryMock.Object, _outpostQueryMock.Object);
        }

        [Test]
        public void GetMessagesFromOutpost_DoesNotReturnWarehouseItems_WhenThereAreNoWarehouses()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender"
            };

            var pageofMessages = new List<RawSmsReceived>().AsQueryable();
            _rawSmsQueryMock.Setup(service => service.Query()).Returns(pageofMessages);

            //act
            var result = _sut.GetMessagesFromOutpost(indexModel, OutpostType.Warehouse);

            Assert.That(result.TotalItems, Is.EqualTo(0));
        }

        [Test]
        public void GetMessagesFromOutpost_ReturnsOnlySellerMessages_WhenOutpostTypeIsSeller()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
                {
                    dir = "ASC",
                    limit = 50,
                    page = 1,
                    start = 0,
                    sort = "Sender"
                };

            var pageofMessages = PageOfMixedMessagesProportion50_50(indexModel);
            _rawSmsQueryMock.Setup(service => service.Query()).Returns(pageofMessages);

            //act
            var result = _sut.GetMessagesFromOutpost(indexModel, OutpostType.Seller);

            Assert.That(result.TotalItems, Is.EqualTo(pageofMessages.Count()/2));
        }

        [Test]
        public void GetMessagesFromOutpost_ReturnsMessagesDescedingByContent_WhenContentIsTheSortingKeyAndDirectionIsDesc()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 9,
                page = 1,
                start = 0,
                sort = "Content"
            };

            var pageofMessages = PageOfMixedMessagesProportion50_50(indexModel);
            _rawSmsQueryMock.Setup(service => service.Query()).Returns(pageofMessages);

            //act
            var result = _sut.GetMessagesFromOutpost(indexModel, OutpostType.Seller);

            Assert.That(result.Messages[0].Content, Is.EqualTo("abc-8"));
        }

        [Test]
        public void GetMessagesFromOutpost_ReturnsOnlyMessagesWhereContentContainsSearchValue()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 9,
                page = 1,
                start = 0,
                sort = "Content",
                searchValue = "-8"
            };

            var pageofMessages = PageOfMixedMessagesProportion50_50(indexModel);
            _rawSmsQueryMock.Setup(service => service.Query()).Returns(pageofMessages);

            //act
            var result = _sut.GetMessagesFromOutpost(indexModel, OutpostType.Seller);

            Assert.That(result.Messages[0].Content, Is.EqualTo("abc-8"));
        }

        private IQueryable<RawSmsReceived> PageOfMixedMessagesProportion50_50(IndexTableInputModel indexTableInputModel)
        {
            var rawSmsList = new List<RawSmsReceived>();

            Debug.Assert(indexTableInputModel.start != null, "IndexTableInputModel.start != null");
            Debug.Assert(indexTableInputModel.limit != null, "IndexTableInputModel.limit != null");
            for (int i = indexTableInputModel.start.Value; i < indexTableInputModel.limit.Value; i++)
            {
                rawSmsList.Add(new RawSmsReceived
                    {
                        Content = "abc" + "-" + i,
                        OutpostType = (OutpostType) (i%2),
                        OutpostId = Guid.NewGuid(),
                        ParseErrorMessage = "Parse error no." + i,
                        ParseSucceeded = false,
                        ReceivedDate = DateTime.UtcNow.AddDays(-i),
                        Sender = "123",
                    });
            }
            return rawSmsList.AsQueryable();
        }
    }
}
