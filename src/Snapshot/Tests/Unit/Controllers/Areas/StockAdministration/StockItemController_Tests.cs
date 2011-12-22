using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Rhino.Mocks;
using Persistence.Queries.StockItems;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.StockItem;

namespace Tests.Unit.Controllers.Areas.StockAdministration
{
    [TestFixture]
    public class StockItemController_Tests
    {
        const string STOCKGROUP_NAME = "stockgroup1";
        const string STOCKGROUP_DESCRIPTION = "description23";

        const string STOCKITEM_NAME = "StockItem1";
        const string STOCKITEM_DESCRIPTION = "Description1";
        const string STOCKITEM_SMSREFERENCE_CODE = "004";
        const string OUTPOST_NAME = "outpost1";
        const int STOCKITEM_LOWERLIMIT = 3;
        const int STOCKITEM_UPPERLIMIT = 1000;


        const string DEFAUL_VIEW_NAME = "";

        StockGroup stockGroup;
        StockItem stockItem;
        Outpost outpost;

        Guid stockItemId;
        Guid stockGroupId;
        Guid outpostId;

        StockItemController controller;

        public IQueryService<StockGroup> queryStockGroup;
        public ISaveOrUpdateCommand<StockItem> saveOrUpdateStockItem;
        public IDeleteCommand<StockItem> deleteStockItem;
        public IQueryStockItem queryStockItem;
        public IQueryService<StockItem> queryService;
        public IQueryService<Outpost> queryOutpost;

        [SetUp]
        public void BeforeEach()
        {
            BuildControllerAndServices();
            StubStockGroup();
            StubOutpost();
            StubStockItem();
        }
        private void StubOutpost()
        {
            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(b => b.Id).Return(outpostId);
            outpost.Name = OUTPOST_NAME;
 
        }
        private void StubStockItem()
        {
            stockItemId = Guid.NewGuid();
            stockItem = MockRepository.GeneratePartialMock<StockItem>();
            stockItem.Stub(b => b.Id).Return(stockItemId);
            stockItem.Name = STOCKITEM_NAME;
            stockItem.Description = STOCKITEM_DESCRIPTION;
            stockItem.StockGroup = stockGroup;
            stockItem.LowerLimit = STOCKITEM_LOWERLIMIT;
            stockItem.UpperLimit = STOCKITEM_UPPERLIMIT;
            stockItem.SMSReferenceCode = STOCKITEM_SMSREFERENCE_CODE;
            stockItem.Outposts = new List<Outpost>();
            stockItem.Outposts.Add(outpost);
        }

        private void StubStockGroup()
        {
            stockGroupId = Guid.NewGuid();
            stockGroup = MockRepository.GeneratePartialMock<StockGroup>();
            stockGroup.Stub(b => b.Id).Return(stockGroupId);
            stockGroup.Name = STOCKGROUP_NAME;
            stockGroup.Description = STOCKGROUP_DESCRIPTION;
            
        }

        private void BuildControllerAndServices()
        {
            controller = new StockItemController();

            queryStockItem = MockRepository.GenerateMock<IQueryStockItem>();
            queryStockGroup = MockRepository.GenerateMock<IQueryService<StockGroup>>();
            saveOrUpdateStockItem = MockRepository.GenerateMock<ISaveOrUpdateCommand<StockItem>>();
            deleteStockItem = MockRepository.GenerateMock<IDeleteCommand<StockItem>>();
            queryService = MockRepository.GenerateMock<IQueryService<StockItem>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();

            controller.QueryStockItem = queryStockItem;
            controller.QueryStockGroup = queryStockGroup;
            controller.SaveOrUpdateStockItem = saveOrUpdateStockItem;
            controller.DeleteStockItem = deleteStockItem;
            controller.QueryService = queryService;
            controller.QueryOutposts = queryOutpost;

        }

        [Test]
        public void Should_ReturnData_From_StockItemQueryService_on_Overview()
        {
            //assert
            queryStockItem.Expect(call => call.GetAll()).Return(new StockItem[] { stockItem }.AsQueryable());

            //act
            var result = (ViewResult)controller.Overview();

            //Assert
            queryStockItem.VerifyAllExpectations();
            Assert.AreEqual(DEFAUL_VIEW_NAME, result.ViewName);

        }

        [Test]
        public void Should_Display_Model_WithLoadedOutpostAndStockGroups_When_GET_Create()
        {
            //assert
            queryStockGroup.Expect(call => call.Query()).Return(new StockGroup[] { stockGroup }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { outpost }.AsQueryable());

            //act
            var result = controller.Create() as ViewResult;

            //assert
            queryStockGroup.VerifyAllExpectations();
            queryOutpost.VerifyAllExpectations();

            Assert.IsInstanceOf<StockItemOutputModel>(result.Model);

            var model = (StockItemOutputModel)result.Model;

            Assert.AreEqual(model.OutpostList.Count, 1);
            Assert.AreEqual(model.StockGroups.Count, 1);

        }

        [Test]
        public void Should_Save_StockItem_When_POST_Save_Succedes()
        {
            //arrange
            var model = new StockItemInputModel();

            BuildStockItemInputModel(model);

            saveOrUpdateStockItem.Expect(it => it.Execute(Arg<StockItem>.Matches(c => c.Name == model.Name && c.Id == model.Id)));
            queryStockGroup.Expect(it => it.Load(stockGroupId)).Return(stockGroup);
            

            //act
            var result = (RedirectToRouteResult)controller.Create(model);

            //assert
            saveOrUpdateStockItem.VerifyAllExpectations();
            queryStockGroup.VerifyAllExpectations();

            Assert.AreEqual("Overview", result.RouteValues["Action"]);
        }

        [Test]
        public void Should_RedirectToCreateView_WithStockItemOutputModelLoaded_WhenModelStateIsNotValid_On_POST_Create()
        {
            //araange
            controller.ModelState.AddModelError("Name", "Name is required");

            queryStockGroup.Expect(call => call.Query()).Return(new StockGroup[] { stockGroup }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { outpost }.AsQueryable());

            var model = new StockItemInputModel();
            BuildStockItemInputModel(model);

            //act
            var result = (ViewResult)controller.Create(model);

            //assert
            queryStockGroup.VerifyAllExpectations();
            queryOutpost.VerifyAllExpectations();
            Assert.AreEqual(result.ViewName, "Create");
            Assert.IsInstanceOf<StockItemOutputModel>(result.Model);

            var viewModel = (StockItemOutputModel)result.Model;

            Assert.AreEqual(viewModel.Id, model.Id);
            Assert.AreEqual(viewModel.StockGroup.Id, model.StockGroup.Id);
            Assert.AreEqual(viewModel.StockGroups.Count, 1);
            Assert.AreEqual(viewModel.StockGroups[0].Selected, true);


        }

        [Test]
        public void Should_Load_A_Completed_Edit_Page_When_GET_Edit()
        {
            //arrange			
            queryService.Expect(it => it.Load(stockItem.Id)).Return(stockItem);
            queryStockGroup.Expect(call => call.Query()).Return(new StockGroup[] { stockGroup }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { outpost }.AsQueryable());
            //act
            var result = (ViewResult)controller.Edit(stockItem.Id);

            //assert
            Assert.IsInstanceOf<StockItemOutputModel>(result.Model);
            var viewModel = result.Model as StockItemOutputModel;
            Assert.AreEqual(STOCKITEM_NAME, viewModel.Name);
            Assert.AreEqual(stockItem.Id, viewModel.Id);
            Assert.AreEqual(viewModel.StockGroups[0].Text, stockGroup.Name);
        }

        [Test]
        public void Should_redirect_to_Overview_when_POST_Edit_succeedes()
        {
            //arrange
            var model = new StockItemInputModel();
            BuildStockItemInputModel(model);

            queryStockGroup.Expect(call => call.Load(stockGroup.Id)).Return(stockGroup);
            saveOrUpdateStockItem.Expect(it => it.Execute(Arg<StockItem>.Matches(c => c.Name == STOCKITEM_NAME && c.Id == stockItem.Id && c.StockGroup.Id == stockGroup.Id)));
            queryOutpost.Expect(it => it.Load(outpost.Id)).Return(outpost);

            // Act
            var redirectResult = (RedirectToRouteResult)controller.Edit(model);

            // Assert
            saveOrUpdateStockItem.VerifyAllExpectations();
            queryStockGroup.VerifyAllExpectations();
            queryOutpost.VerifyAllExpectations();
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }

        [Test]
        public void Should_Redirect_To_Edit_When_POST_Edit_Fails_BecauseOfModelStateNotValid()
        {
            //arrange
            controller.ModelState.AddModelError("Name", "Field required");
            queryStockGroup.Expect(call => call.Query()).Return(new StockGroup[] { new StockGroup { Name = "Romania" } }.AsQueryable());
            queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { outpost }.AsQueryable());

            var model = new StockItemInputModel();
            BuildStockItemInputModel(model);

            //act
            var viewResult = (ViewResult)controller.Edit(model);

            //assert
            queryStockGroup.VerifyAllExpectations();
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<StockItemOutputModel>(viewResult.Model);

            var viewModel = (StockItemOutputModel)viewResult.Model;

            Assert.AreEqual(viewModel.Id, model.Id);
            Assert.AreEqual(viewModel.StockGroup.Id, model.StockGroup.Id);
        }

        [Test]
        public void Should_Redirect_ToOverview_When_POST_DeleteSuccedeed()
        {
            //arrange
            queryService.Expect(call => call.Load(stockItem.Id)).Return(stockItem);
            deleteStockItem.Expect(call => call.Execute(stockItem));

            //act
            var result = (RedirectToRouteResult)controller.Delete(stockItem.Id);

            //assert
            queryService.VerifyAllExpectations();
            deleteStockItem.VerifyAllExpectations();
            Assert.AreEqual(result.RouteValues["Action"], "Overview");
 
        }
        private void BuildStockItemInputModel(StockItemInputModel model)
        {
            model.Description = STOCKITEM_DESCRIPTION;
            model.Name = STOCKITEM_NAME;
            model.LowerLimit = STOCKITEM_LOWERLIMIT;
            model.UpperLimit = STOCKITEM_UPPERLIMIT;
            model.SMSReferenceCode = STOCKITEM_SMSREFERENCE_CODE;
            model.Id = stockItemId;
            model.StockGroup = new StockItemInputModel.StockGroupInputModel();
            model.StockGroup.Id = stockGroupId;
            model.Outpost = new StockItemOutputModel.OutpostModel();
            model.Outpost.Id = outpost.Id;
        }

        
    }
}
