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
        public void GetMessagesFromOutpost_ReturnsOnlySellerMessages_WhenOutpostTypeIsSeller()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
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
            var indexModel = new MessagesIndexModel
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
            var indexModel = new MessagesIndexModel
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

        private IQueryable<RawSmsReceived> PageOfMixedMessagesProportion50_50(MessagesIndexModel indexModel)
        {
            var rawSmsList = new List<RawSmsReceived>();

            Debug.Assert(indexModel.start != null, "indexModel.start != null");
            Debug.Assert(indexModel.limit != null, "indexModel.limit != null");
            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
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
