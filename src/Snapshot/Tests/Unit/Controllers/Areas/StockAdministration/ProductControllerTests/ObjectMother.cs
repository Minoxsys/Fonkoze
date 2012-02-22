using System;
using Rhino.Mocks;
using Domain;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Web.Areas.StockAdministration.Models.Product;
using System.Linq;
using System.Collections.Generic;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductControllerTests
{
    public class ObjectMother
    {
        const string PRODUCTGROUP_NAME = "stockgroup1";
        const string PRODUCTGROUP_DESCRIPTION = "description23";

        const string PRODUCT_NAME = "StockItem1";
        const string PRODUCT_DESCRIPTION = "Description1";
        const string PRODUCT_SMSREFERENCE_CODE = "004";
        const int PRODUCT_LOWERLIMIT = 3;
        const int PRODUCT_UPPERLIMIT = 1000;
        
        public ProductGroup productGroup;
        public Product product;
        public Product product2;
        public OutpostStockLevel outpostStockLevel;
        public OutpostHistoricalStockLevel outpostHystoricalStockLevel;

        Guid outpostHystoricalStockLevelId;
        Guid outpostStockLevelId;
        Guid productId;
        Guid productId2;
        Guid productGroupId;

        public ProductController controller;

        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        public IQueryService<ProductGroup> queryProductGroup;
        public ISaveOrUpdateCommand<Product> saveOrUpdateProduct;
        public IDeleteCommand<Product> deleteProduct;
        public IQueryService<Product> queryService;
        public IQueryService<OutpostHistoricalStockLevel> queryHistoricalOutpostStockLevel;

        
        public void Init()
        {
            BuildControllerAndServices();
            StubProductGroup();
            StubProduct();
            StubProduct2();
            StubOutpostStockLevel();
            StubOutpostHystoricalStockLevel();
        }

        internal void StubProduct()
        {
            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(b => b.Id).Return(productId);
            product.Name = PRODUCT_NAME;
            product.Description = PRODUCT_DESCRIPTION;
            product.ProductGroup = productGroup;
            product.LowerLimit = PRODUCT_LOWERLIMIT;
            product.UpperLimit = PRODUCT_UPPERLIMIT;
            product.SMSReferenceCode = PRODUCT_SMSREFERENCE_CODE;

        }
        internal void StubProduct2()
        {
            productId2 = Guid.NewGuid();
            product2 = MockRepository.GeneratePartialMock<Product>();
            product2.Stub(b => b.Id).Return(productId2);
            product2.Name = PRODUCT_NAME;
            product2.Description = PRODUCT_DESCRIPTION;
            product2.ProductGroup = productGroup;
            product2.LowerLimit = PRODUCT_LOWERLIMIT;
            product2.UpperLimit = PRODUCT_UPPERLIMIT;
            product2.SMSReferenceCode = PRODUCT_SMSREFERENCE_CODE;

        }
        internal void StubOutpostStockLevel()
        {
            outpostStockLevelId = Guid.NewGuid();
            outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            outpostStockLevel.Stub(b => b.Id).Return(outpostStockLevelId);
            outpostStockLevel.Product = product;
 
        }
        internal void StubOutpostHystoricalStockLevel()
        {
            outpostHystoricalStockLevelId = Guid.NewGuid();
            outpostHystoricalStockLevel = MockRepository.GeneratePartialMock<OutpostHistoricalStockLevel>();
            outpostHystoricalStockLevel.Stub(it => it.Id).Return(outpostHystoricalStockLevelId);
            outpostHystoricalStockLevel.ProductId = product.Id;
        }
        internal void StubProductGroup()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(b => b.Id).Return(productGroupId);
            productGroup.Name = PRODUCTGROUP_NAME;
            productGroup.Description = PRODUCTGROUP_DESCRIPTION;

        }

        internal void BuildControllerAndServices()
        {
            controller = new ProductController();

            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveOrUpdateProduct = MockRepository.GenerateMock<ISaveOrUpdateCommand<Product>>();
            deleteProduct = MockRepository.GenerateMock<IDeleteCommand<Product>>();
            queryService = MockRepository.GenerateMock<IQueryService<Product>>();
            queryHistoricalOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostHistoricalStockLevel>>();
            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();

            controller.QueryOutpostStockLevel = queryOutpostStockLevel;
            controller.QueryProductGroup = queryProductGroup;
            controller.SaveOrUpdateProduct = saveOrUpdateProduct;
            controller.DeleteProduct = deleteProduct;
            controller.QueryService = queryService;
            controller.QueryOutpostStockLevelHystorical = queryHistoricalOutpostStockLevel;

        }

        internal IQueryable<Product> PageOfProductData(ProductIndexModel indexModel)
        {
            List<Product> productPageList = new List<Product>();

            for (int i = indexModel.Start.Value; i < indexModel.Limit.Value; i++)
            {
                productPageList.Add(new Product
                {
                    Name = String.Format("Product{0}", i),
                    Description = product.Description,
                    ProductGroup = productGroup
                });
            }
            return productPageList.AsQueryable();
        }
    }
}
