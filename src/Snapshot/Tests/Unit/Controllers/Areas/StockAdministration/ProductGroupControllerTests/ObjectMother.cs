using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.ProductGroup;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    public class ObjectMother
    {

        public ProductGroupController controller;

        public IQueryService<Product> queryProduct;
        public IQueryService<ProductGroup> queryProductGroup;
        public ISaveOrUpdateCommand<ProductGroup> saveCommand;
        public IDeleteCommand<ProductGroup> deleteCommand;

        public ProductGroup productGroup;
        public Product product;

        public Guid productGroupId;
        public Guid productId;

        private const string PRODUCTGROUP_NAME = "Malaria";
        private const string PRODUCTGROUP_DESCRIPTION = "Descriere pentru malaria";
        private const string PRODUCTGROUP_REFERENCECODE = "MAL";
        private const string PRODUCT_NAME = "Produs1";
        
        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryProduct = MockRepository.GenerateMock<IQueryService<Product>>();
            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<ProductGroup>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<ProductGroup>>();
        }

        private void Setup_Controller()
        {
            controller = new ProductGroupController();

            controller.QueryService = queryProductGroup;
            controller.QueryProduct = queryProduct;
            controller.SaveOrUpdateProductGroup = saveCommand;
            controller.DeleteCommand = deleteCommand;
        }

        private void SetUp_StubData()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = PRODUCTGROUP_NAME;
            productGroup.Description = PRODUCTGROUP_DESCRIPTION;
            productGroup.ReferenceCode = PRODUCTGROUP_REFERENCECODE;

            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(c => c.Id).Return(productId);
            product.Name = PRODUCT_NAME;
            product.ProductGroup = productGroup;
        }

        public IQueryable<ProductGroup> PageOfProductGroupData(ProductGroupIndexModel indexModel)
        {
            List<ProductGroup> productGroupPageList = new List<ProductGroup>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                productGroupPageList.Add(new ProductGroup
                {
                    Name = "Malaria" + i,
                    Description = i + " " + PRODUCTGROUP_DESCRIPTION
                });
            }
            return productGroupPageList.AsQueryable();
        }


        

    }
}
