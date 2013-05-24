using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.Services;

namespace Tests.Unit.Services
{
    [TestFixture]
    public class RetrieveAllDistrictsServiceTests
    {
        private RetrieveAllDistrictsService _sut;
        private Mock<IQueryService<District>> _districtQueryMock;

        [SetUp]
        public void PerTestSetup()
        {
            _districtQueryMock = new Mock<IQueryService<District>>();
            _sut = new RetrieveAllDistrictsService(_districtQueryMock.Object);
        }

        [Test]
        public void GetAllDistrictsForOneClient_GetAListWithOneEntityModelItem_WhenGuidIsEmpty()
        {
            _districtQueryMock.Setup(s => s.Query())
                .Returns(new List<District>().AsQueryable());

            var ret = _sut.GetAllDistrictsForOneClient(Guid.Empty);

            CollectionAssert.AllItemsAreInstancesOfType(ret, typeof(Web.Areas.StockAdministration.Models.OutpostStockLevel.EntityModel));
            Assert.AreEqual(ret.Count, 1);
        }
    }
}
