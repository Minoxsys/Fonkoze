using System;
using System.Linq;
using System.Collections.Generic;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.Models.Parsing;
using Web.Services.StockUpdates;
using Web.WarehouseMgmtUseCase.Services;

namespace Tests.Unit.WarehouseManagementWorkflow
{
    [TestFixture]
    public class AssigningExistingProductsToWarehouseServiceTests
    {
        private AssigningExistingProductsToWarehouseService _sut;
        Mock<IQueryService<Product>> _productQueryServiceMock;
        Mock<IQueryService<Outpost>> _outpostQueryServiceMock;
        Mock<ISaveOrUpdateCommand<OutpostStockLevel>> _saveOutpostStockLevelCommandMock;
        private Guid _outpostId;

        [SetUp]
        public void PerTestSetup()
        {
            _outpostId = Guid.NewGuid();
            _productQueryServiceMock = new Mock<IQueryService<Product>>();
            _outpostQueryServiceMock = new Mock<IQueryService<Outpost>>();
            _saveOutpostStockLevelCommandMock = new Mock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            _sut = new AssigningExistingProductsToWarehouseService(_productQueryServiceMock.Object, _outpostQueryServiceMock.Object, _saveOutpostStockLevelCommandMock.Object);
        }

        [Test]
        public void AssigningProductsToWarehouse_SaveOutpostStockLevelToPersistence_WhenIEnumerableHasOneValidProduct()
        {
            var parsedProductList = new List<IParsedProduct>();
            parsedProductList.Add(new ParsedProduct
            {
                ProductGroupCode = GetOneValidProduct().ProductGroup.ReferenceCode,
                ProductCode = GetOneValidProduct().SMSReferenceCode,
                StockLevel = 5
            });

            _productQueryServiceMock.Setup(s => s.Query())
                .Returns(GetIQueryableWithOneValidProduct());

            _sut.AssigningProductsToWarehouse(parsedProductList, _outpostId);

            _saveOutpostStockLevelCommandMock.Verify(s => s.Execute(It.Is<OutpostStockLevel>(x => x.Product.Id == GetOneValidProduct().Id)));
        }

        [Test]
        public void AssigningProductsToWarehouse_DontSaveOutpostStockLevelToPersistence_WhenIEnumerableHasOneProductThatDoesntExistsInTheSpecifiedProductGroup()
        {
            var parsedProductList = new List<IParsedProduct>();
            parsedProductList.Add(new ParsedProduct
            {
                ProductGroupCode = "NULL",
                ProductCode = GetOneValidProduct().SMSReferenceCode,
                StockLevel = 5
            });

            _productQueryServiceMock.Setup(s => s.Query())
                .Returns(GetIQueryableWithOneValidProduct());

            _sut.AssigningProductsToWarehouse(parsedProductList, _outpostId);

            _saveOutpostStockLevelCommandMock.Verify(s => s.Execute(It.Is<OutpostStockLevel>(x => x.Product.Id == GetOneValidProduct().Id)), Times.Never());
        }

        [Test]
        public void AssigningProductsToWarehouse_DontSaveOutpostStockLevelToPersistence_WhenIEnumerableHasOneProductThatDoesntExistsInTheSystem()
        {
            var parsedProductList = new List<IParsedProduct>();
            parsedProductList.Add(new ParsedProduct
            {
                ProductGroupCode = "ALL",
                ProductCode = "NULL",
                StockLevel = 5
            });

            _productQueryServiceMock.Setup(s => s.Query())
                .Returns(GetIQueryableWithOneValidProduct());

            _sut.AssigningProductsToWarehouse(parsedProductList, _outpostId);

            _saveOutpostStockLevelCommandMock.Verify(s => s.Execute(It.Is<OutpostStockLevel>(x => x.Product.Id == GetOneValidProduct().Id)), Times.Never());
        }

        [Test]
        public void AssigningProductsToWarehouse_DontSaveOutpostStockLevelToPersistence_WhenIEnumerableIsEmpty()
        {
            _productQueryServiceMock.Setup(s => s.Query())
                .Returns(GetIQueryableWithOneValidProduct());

            _sut.AssigningProductsToWarehouse(new List<IParsedProduct>(), _outpostId);

            _saveOutpostStockLevelCommandMock.Verify(s => s.Execute(It.Is<OutpostStockLevel>(x => x.Product.Id == GetOneValidProduct().Id)), Times.Never());
        }

        #region helpers
        private IQueryable<Product> GetIQueryableWithOneValidProduct()
        {
            var productList = new List<Product>();
            productList.Add(GetOneValidProduct());
            return productList.AsQueryable();
        }

        private Product GetOneValidProduct()
        {
            var p = new Product();

            p.Name = "newP";
            p.SMSReferenceCode = "PP";
            p.ProductGroup = new ProductGroup();
            p.ProductGroup.ReferenceCode = "ALL";

            return p;
        }
        #endregion
    }
}
