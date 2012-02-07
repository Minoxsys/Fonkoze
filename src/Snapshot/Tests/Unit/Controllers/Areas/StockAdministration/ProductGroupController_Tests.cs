using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Controllers;
using Domain;
using Core.Persistence;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.Product;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Core.Domain;
using Persistence.Queries.Countries;



namespace Tests.Unit.Controllers.Areas.StockAdministration
{
    [TestFixture]
    public class ProductGroupController_Tests
    {
        const string DEFAULT_VIEW_NAME = "";
        const string PRODUCTGROUP_NAME = "Malaria";
        const string NEW_PRODUCTGROUP_NAME = "Fever";
        const string PRODUCT_NAME = "Yellow";
        const string NEW_PRODUCT_NAME = "Blue";


        ProductGroupController controller;
        IQueryService<ProductGroup> queryService;
        ISaveOrUpdateCommand<ProductGroup> saveCommand;
        IDeleteCommand<ProductGroup> deleteCommand;
        IQueryService<ProductGroup> queryProductGroups;
        IQueryService<Product> queryProduct;
        IQueryService<Client> queryClient;


        ProductGroup entity;
        Product product;
        Guid productId;
        Guid entityId;

        [SetUp]
        public void BeforeEach()
        {
            SetUpServices();
            SetUpController();
            StubProduct();
            StubEntity();
        }

        private void SetUpServices()
        {
            queryService = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<ProductGroup>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<ProductGroup>>();
            //queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            queryProduct = MockRepository.GenerateMock<IQueryService<Product>>();

        }

        private void SetUpController()
        {
            controller = new ProductGroupController();

            controller.QueryService = queryService;
            controller.SaveOrUpdateProductGroup = saveCommand;
            controller.DeleteCommand = deleteCommand;
            //controller.q = queryClient;
           // controller.QueryProduct = queryRegion;
        }

        private void StubEntity()
        {
            entityId = Guid.NewGuid();
            entity = MockRepository.GeneratePartialMock<ProductGroup>();
            entity.Stub(b => b.Id).Return(entityId);
            entity.Name = PRODUCTGROUP_NAME;
        }

        private void StubProduct()
        {
            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(b => b.Id).Return(productId);
            product.Name = "Yellow";
            //product.Country = entity;

        }

        // [Test]
        //public void Should_Return_DataSpecificToCountryId_From_QueryService_in_Overview()
        //{
        //    // Arrange		
            
        //    queryService.Expect(call => call.Query()).Repeat.Once().Return(new ProductGroup[] { entity }.AsQueryable());
        //    //queryProduct.Expect(call => call.Query()).Return(new Product[] { product }.AsQueryable());
        //    //queryDistrict.Expect(call => call.Query()).Return(new District[] { district }.AsQueryable());

        //    // Act
        //    var viewResult = (ViewResult)controller.Overview();

        //    // Assert
        //    queryService.VerifyAllExpectations();
        //    //queryProduct.VerifyAllExpectations();

        //    Assert.IsNotNull(viewResult.Model);
        //    var viewModel = (ProductGroupOverviewModel)viewResult.Model;
        //    //Assert.AreEqual(entity.Name, viewModel.Countries[0].Name);
        //    //Assert.AreEqual(viewModel.Countries[0].Value, entity.Country.Id.ToString());
        //    //Assert.AreEqual(viewModel.Regions[0].DistrictNo, 1);
        //    Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        //}

        //[Test]
        //public void Should_Return_AllExistentCountries_AndNoRegions_WhenCountryIdIsNull_FromQueryService_in_Overview()
        //{ 
        //    //arrange
        //    queryCountry.Expect(call => call.Query()).Return(new Country[] { country }.AsQueryable());
          

        //    // Act
        //    var viewResult = (ViewResult)controller.(Guid.Empty);


        //    // Assert
        //   queryRegion.VerifyAllExpectations();

        //    Assert.IsNotNull(viewResult.Model);
        //    var viewModel = (RegionOverviewModel)viewResult.Model;
        //    Assert.IsNotNull(viewModel.Countries); 
        //    Assert.IsEmpty(viewModel.Regions);
        //    Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        //}

        [Test]
        public void Should_Display_Empty_Model_When_GET_Create()
        {
            ////assert
           
            ////act
            //var result = controller.Create() as ViewResult;

            ////assert
            //Assert.IsNull(result.Model);
            
        }

        [Test]
        public void Should_Save_Country_When_Save_Succedes()
        {
            ////arrange
            //var model = new ProductGroupInputModel();
            //model = BuildProductGroupWithName(PRODUCTGROUP_NAME);
            //saveCommand.Expect(it => it.Execute(Arg<ProductGroup>.Matches(c => c.Name == PRODUCTGROUP_NAME)));
            //queryClient.Expect(it => it.Load(Guid.Empty)).Return(new Client { Name = "client" });

            ////act
            //var result = (RedirectToRouteResult)controller.Create(new ProductGroupInputModel() { Name = PRODUCTGROUP_NAME });

            ////assert
            //saveCommand.VerifyAllExpectations();
            //Assert.AreEqual("Overview", result.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Create_When_POST_Create_Fails_BecauseOf_ModelStateNotValid()
        {
            ////arrange
            //controller.ModelState.AddModelError("Name", "Field required");

            //queryService.Expect(call => call.Query()).Return(new ProductGroup[] { entity }.AsQueryable());

            //var countryInputModel = SetProductGroupInputModelData_ToPassToCreateMethod();

            ////act
            //var viewResult = (ViewResult)controller.Create(countryInputModel);

            ////assert
            //Assert.AreEqual("Create", viewResult.ViewName);
            //Assert.IsInstanceOf<ProductGroupOutputModel>(viewResult.Model);

            //var model = (ProductGroupOutputModel)viewResult.Model;
            //Assert.AreEqual(model.Id, entity.Id);            

        }

        private ProductGroupInputModel SetProductGroupInputModelData_ToPassToCreateMethod()
        {
            var productGroupInputModel = new ProductGroupInputModel();
            productGroupInputModel.Id = entity.Id;
            return productGroupInputModel;
        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            //queryProduct.Expect(call => call.Query()).Return(new ProductGroup[] { entity }.AsQueryable());
            queryService.Expect(it => it.Load(entity.Id)).Return(entity);
            //act
            var result = (ViewResult)controller.Edit(entity.Id);

            //assert
            Assert.IsInstanceOf<ProductGroupOutputModel>(result.Model);
            var viewModel = result.Model as ProductGroupOutputModel;
            Assert.AreEqual(PRODUCTGROUP_NAME, viewModel.Name);
            Assert.AreEqual(entity.Id, viewModel.Id);
            //Assert.AreEqual(viewModel.Products[0].Text, entity.Name);
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new ProductGroupInputModel();
            model = BuildProductGroupWithName(NEW_PRODUCTGROUP_NAME);

            queryService.Expect(call => call.Load(entity.Id)).Return(entity);
            saveCommand.Expect(it => it.Execute(Arg<ProductGroup>.Matches(c => c.Name == NEW_PRODUCTGROUP_NAME && c.Id == entity.Id)));

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(new ProductGroupInputModel() { Id = entity.Id, Name = NEW_PRODUCTGROUP_NAME});

            // Assert
            saveCommand.VerifyAllExpectations();
            //queryProduct.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            controller.ModelState.AddModelError("Name", "Field required");
            
            //queryService.Expect(call => call.Query()).Return(new ProductGroup[] { new ProductGroup { Name = "Yellow fever" } }.AsQueryable());

            var viewResult = (ViewResult)controller.Edit(new ProductGroupInputModel());

            queryService.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
        }



        //[Test]
        //public void Should_GoTo_Overview_WhenDeleteARegion_AndDisplayTempDataError_If_ThereAreDistrictsAsociated()
        //{
        //    //arrange
        //    //queryProduct.Expect(call => call.Query()).Repeat.Once().Return(new Product[] { product }.AsQueryable());
        //    queryService.Expect(call => call.Load(entity.Id)).Return(entity);

        //    //act
        //    var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);

        //    //assert
        //    queryService.VerifyAllExpectations();
        //    //queryDistrict.VerifyAllExpectations();

        //   //Assert.That(controller.TempData.ContainsKey("error"));
        //   //Assert.That(controller.TempData.ContainsValue("The Product Group " +entity.Name + " has products associated, so it can not be deleted"));
            
        //}

        //[Test]
        //public void Should_goto_Overview_when_Delete_AndThereAre_NoDistrictAsociated()
        //{
        //    //arrange
        //    queryService.Expect(call => call.Load(entity.Id)).Return(entity);
        //    deleteCommand.Expect(call => call.Execute(Arg<ProductGroup>.Matches(b => b.Id == entity.Id)));

        //    // Act
        //    var redirectResult = (RedirectToRouteResult)controller.Delete(entity.Id);

        //    // Assert
        //    deleteCommand.VerifyAllExpectations();
        //    Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        //}
        
        private ProductGroupInputModel BuildProductGroupWithName(string name)
        {
        
            var model = new ProductGroupInputModel();
            model.Name = name;
            model.Id = entityId;

            return model;
        }
               
    }
}
