using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Rhino.Mocks;
using Persistence.Queries.Products;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.Product;

namespace Tests.Unit.Controllers.Areas.StockAdministration
{
    [TestFixture]
    public class StockItemController_Tests
    {
        const string PRODUCTGROUP_NAME = "stockgroup1";
        const string PRODUCTGROUP_DESCRIPTION = "description23";

        const string PRODUCT_NAME = "StockItem1";
        const string PRODUCT_DESCRIPTION = "Description1";
        const string PRODUCT_SMSREFERENCE_CODE = "004";
        const int PRODUCT_LOWERLIMIT = 3;
        const int PRODUCT_UPPERLIMIT = 1000;


        const string DEFAUL_VIEW_NAME = "";

        ProductGroup productGroup;
        Product product;
       
        Guid productId;
        Guid productGroupId;
       
        ProductController controller;

        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;

        public IQueryService<ProductGroup> queryProductGroup;
        public ISaveOrUpdateCommand<Product> saveOrUpdateProduct;
        public IDeleteCommand<Product> deleteProduct;
        public IQueryService<Product> queryService;
       
        [SetUp]
        public void BeforeEach()
        {
            BuildControllerAndServices();
            StubProductGroup();
            StubProduct();
        }
       
        private void StubProduct()
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

        private void StubProductGroup()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(b => b.Id).Return(productGroupId);
            productGroup.Name = PRODUCTGROUP_NAME;
            productGroup.Description = PRODUCTGROUP_DESCRIPTION;
            
        }

        private void BuildControllerAndServices()
        {
            controller = new ProductController();

            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveOrUpdateProduct = MockRepository.GenerateMock<ISaveOrUpdateCommand<Product>>();
            deleteProduct = MockRepository.GenerateMock<IDeleteCommand<Product>>();
            queryService = MockRepository.GenerateMock<IQueryService<Product>>();

            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();

            controller.QueryOutpostStockLevel = queryOutpostStockLevel;
            controller.QueryProductGroup = queryProductGroup;
            controller.SaveOrUpdateProduct = saveOrUpdateProduct;
            controller.DeleteProduct = deleteProduct;
            controller.QueryService = queryService;
            
        }

        [Test]
        public void Should_Return_AllProductGroups_AndNoProducta_From_QueryService_on_Overview_WhenProductGroupId_IsNull()
        {
            //assert
            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { productGroup }.AsQueryable());
           
            //act
            var result = (ViewResult)controller.Overview(null,1);

            //Assert
            queryProductGroup.VerifyAllExpectations();
            Assert.AreEqual(DEFAUL_VIEW_NAME, result.ViewName);
            Assert.IsInstanceOf<ProductOverviewModel>(result.Model);

            var model = (ProductOverviewModel)result.Model;

            Assert.AreEqual(0, model.Products.Count);
            Assert.AreEqual(1, model.ProductGroups.Count);

        }

        [Test]
        public void Should_Return_AllProductGroups_AndProductsSpecificToProductGroupId_From_QueryService_on_Overview_WhenProductGroupId_IsNotNull()
        {
            //assert
            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { productGroup }.AsQueryable());
            queryService.Expect(call => call.Query()).Return(new Product[] { product }.AsQueryable());

            //act
            var result = (ViewResult)controller.Overview(productGroup.Id,1);

            //Assert
            queryProductGroup.VerifyAllExpectations();
            queryService.VerifyAllExpectations();

            Assert.AreEqual(DEFAUL_VIEW_NAME, result.ViewName);
            Assert.IsInstanceOf<ProductOverviewModel>(result.Model);

            var model = (ProductOverviewModel)result.Model;

            Assert.AreEqual(1, model.Products.Count);
            Assert.AreEqual(1, model.ProductGroups.Count);

        }
        [Test]
        public void Should_Display_Model_WithLoadedProductGroups_When_GET_Create()
        {
            //assert
            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { productGroup }.AsQueryable());
           

            //act
            var result = controller.Create() as ViewResult;

            //assert
            queryProductGroup.VerifyAllExpectations();
           
            Assert.IsInstanceOf<ProductOutputModel>(result.Model);

            var model = (ProductOutputModel)result.Model;

           Assert.AreEqual(model.ProductGroups.Count, 1);

        }

        [Test]
        public void Should_Save_StockItem_When_POST_Save_Succedes()
        {
            //arrange
            var model = new ProductInputModel();

            BuildStockItemInputModel(model);

            saveOrUpdateProduct.Expect(it => it.Execute(Arg<Product>.Matches(c => c.Name == model.Name && c.Id == model.Id)));
            queryProductGroup.Expect(it => it.Load(productGroupId)).Return(productGroup);
            

            //act
            var result = (RedirectToRouteResult)controller.Create(model);

            //assert
            saveOrUpdateProduct.VerifyAllExpectations();
            queryProductGroup.VerifyAllExpectations();

            Assert.AreEqual("Overview", result.RouteValues["Action"]);
        }

        [Test]
        public void Should_RedirectToCreateView_WithStockItemOutputModelLoaded_WhenModelStateIsNotValid_On_POST_Create()
        {
            //araange
            controller.ModelState.AddModelError("Name", "Name is required");

            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { productGroup }.AsQueryable());
          

            var model = new ProductInputModel();
            BuildStockItemInputModel(model);

            //act
            var result = (ViewResult)controller.Create(model);

            //assert
            queryProductGroup.VerifyAllExpectations();
          
            Assert.AreEqual(result.ViewName, "Create");
            Assert.IsInstanceOf<ProductOutputModel>(result.Model);

            var viewModel = (ProductOutputModel)result.Model;

            Assert.AreEqual(viewModel.Id, model.Id);
            Assert.AreEqual(viewModel.ProductGroup.Id, model.ProductGroup.Id);
            Assert.AreEqual(viewModel.ProductGroups.Count, 1);
            Assert.AreEqual(viewModel.ProductGroups[0].Selected, true);


        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryService.Expect(it => it.Load(product.Id)).Return(product);
            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { productGroup }.AsQueryable());
           
            //act
            var result = (ViewResult)controller.Edit(product.Id);

            //assert
            Assert.IsInstanceOf<ProductOutputModel>(result.Model);
            var viewModel = result.Model as ProductOutputModel;
            Assert.AreEqual(PRODUCT_NAME, viewModel.Name);
            Assert.AreEqual(product.Id, viewModel.Id);
            Assert.AreEqual(viewModel.ProductGroups[0].Text, productGroup.Name);
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new ProductInputModel();
            BuildStockItemInputModel(model);

            queryProductGroup.Expect(call => call.Load(productGroup.Id)).Return(productGroup);
            saveOrUpdateProduct.Expect(it => it.Execute(Arg<Product>.Matches(c => c.Name == PRODUCT_NAME && c.Id == product.Id && c.ProductGroup.Id == productGroup.Id)));
           
            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(model);

            // Assert
            saveOrUpdateProduct.VerifyAllExpectations();
            queryProductGroup.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");
            queryProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { new ProductGroup { Name = "Romania" } }.AsQueryable());
           
            var model = new ProductInputModel();
            BuildStockItemInputModel(model);

            //act
            var viewResult = (ViewResult)controller.Edit(model);

            //assert
            queryProductGroup.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<ProductOutputModel>(viewResult.Model);

            var viewModel = (ProductOutputModel)viewResult.Model;

            Assert.AreEqual(viewModel.Id, model.Id);
            Assert.AreEqual(viewModel.ProductGroup.Id, model.ProductGroup.Id);
        }

        [Test]
        public void Should_Redirect_ToOverview_When_POST_DeleteSuccedeed()
        {
            //arrange
            queryService.Expect(call => call.Load(product.Id)).Return(product);

            queryOutpostStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { }.AsQueryable());
           
            deleteProduct.Expect(call => call.Execute(product));

            //act
            var result = (RedirectToRouteResult)controller.Delete(product.Id);

            //assert
            queryService.VerifyAllExpectations();
            deleteProduct.VerifyAllExpectations();
            Assert.AreEqual(result.RouteValues["Action"], "Overview");
 
        }
        private void BuildStockItemInputModel(ProductInputModel model)
        {
            model.Description = PRODUCT_DESCRIPTION;
            model.Name = PRODUCT_NAME;
            model.LowerLimit = PRODUCT_LOWERLIMIT;
            model.UpperLimit = PRODUCT_UPPERLIMIT;
            model.SMSReferenceCode = PRODUCT_SMSREFERENCE_CODE;
            model.Id = productId;
            model.ProductGroup = new ProductInputModel.ProductGroupInputModel();
            model.ProductGroup.Id = productGroupId;
           
        }

        
    }
}
