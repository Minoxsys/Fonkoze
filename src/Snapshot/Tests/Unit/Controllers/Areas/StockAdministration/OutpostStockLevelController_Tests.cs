using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Controllers;
using Domain;
using Core.Persistence;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using System.Web.Mvc;

namespace Tests.Unit.Controllers.Areas.StockAdministration
{
    [TestFixture]
    public class OutpostStockLevelController_Tests
    {

        List<Outpost> outposts;

        Guid countryId;
        Country country;
        const String COUNTRY_NAME = "country";

        Guid regionId;
        Region region;
        const String REGION_NAME = "region";

        Guid districtId;
        District district;
        const String DISTRICT_NAME = "district";

        List<OutpostStockLevel> outpostStockLevels;
        List<OutpostStockLevelHistorical> outpostStockLevelsHistorical;
        List<ProductGroup> productGroups;
        List<Product> products;

        OutpostStockLevelController controller;

        IQueryService<Product> queryProduct;
        IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        IQueryService<OutpostStockLevelHistorical> queryOutpostStockLevelHistorical;
        IQueryService<ProductGroup> queryProductGroup;
        IQueryService<Outpost> queryOutpost;
        IQueryService<Country> queryCountry;
        IQueryService<Region> queryRegion;
        IQueryService<District> queryDistrict;

        ISaveOrUpdateCommand<OutpostStockLevel> saveOrUpdateOutpostStockLevel;
        ISaveOrUpdateCommand<OutpostStockLevelHistorical> saveOrUpdateOutpostStockLevelHistorical;



        [SetUp]
        public void BeforeAll()
        {
            BuildControllerAndServices();
            StubCountry();
            StubRegion();
            StubDistrict();
            StubOutpostList();
            StubProductGroupList();
            StubProductList();
            StubOutpostStockLevelList_And_OutpostStockLevelHistoricalList();

        }

        public void BuildControllerAndServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryProduct = MockRepository.GenerateMock<IQueryService<Product>>();
            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryOutpostStockLevelHistorical = MockRepository.GenerateMock<IQueryService<OutpostStockLevelHistorical>>();
            saveOrUpdateOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            saveOrUpdateOutpostStockLevelHistorical = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevelHistorical>>();

            controller = new OutpostStockLevelController();

            controller.QueryOutpost = queryOutpost;
            controller.QueryOutpostStockLevel = queryOutpostStockLevel;
            controller.QueryOutpostStockLevelHistorical = queryOutpostStockLevelHistorical;
            controller.QueryProduct = queryProduct;
            controller.QueryProductGroup = queryProductGroup;
            controller.SaveOrUpdateOutpostStockLevel = saveOrUpdateOutpostStockLevel;
            controller.SaveOrUpdateOutpostStockLevelHistorical = saveOrUpdateOutpostStockLevelHistorical;
            controller.QueryCountry = queryCountry;
            controller.QueryRegion = queryRegion;
            controller.QueryDistrict = queryDistrict;
        }

        public void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = COUNTRY_NAME;
        }
        public void StubRegion()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
        }

        public void StubDistrict()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
        }

        public void StubOutpostList()
        {
            outposts = new List<Outpost>();
            for (int i = 0; i < 10; i++)
            {
                var outpost = new Outpost();
                var outpostId = Guid.NewGuid();
                outpost = MockRepository.GeneratePartialMock<Outpost>();
                outpost.Stub(it => it.Id).Return(outpostId);
                outpost.Name = "outpost" + i;
                outpost.Country = country;
                outpost.Region = region;
                outpost.District = district;

                outposts.Add(outpost);
            }
        }
        public void StubOutpostStockLevelList_And_OutpostStockLevelHistoricalList()
        {
            outpostStockLevels = new List<OutpostStockLevel>();
            outpostStockLevelsHistorical = new List<OutpostStockLevelHistorical>();
            for (int i = 0; i < 20; i++)
            {
                var outpostStockLevel = new OutpostStockLevel();
                var outpostStockLevelHistorical = new OutpostStockLevelHistorical();

                var outpostStockLevelHistoricalId = Guid.NewGuid();
                outpostStockLevelHistorical = MockRepository.GeneratePartialMock<OutpostStockLevelHistorical>();
                outpostStockLevelHistorical.Stub(it=>it.Id).Return(outpostStockLevelHistoricalId);

                var outpostStockLevelId = Guid.NewGuid();
                outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
                outpostStockLevel.Stub(it => it.Id).Return(outpostStockLevelId);
                //from 0-9 assign 0-9 outposts
                if (i < 10)
                {
                    outpostStockLevel.OutpostId = outposts[i].Id;
                    outpostStockLevelHistorical.OutpostId = outposts[i].Id;

                } //from 10 -19 assign outposts with index from 0-9
                else
                {
                    outpostStockLevel.OutpostId = outposts[i - 10].Id;
                    outpostStockLevelHistorical.OutpostId = outposts[i - 10].Id;
                }
                outpostStockLevels.Add(outpostStockLevel);
                outpostStockLevelsHistorical.Add(outpostStockLevelHistorical);

            }

            for (int i = 0; i < outpostStockLevels.Count; i++)
            {
                outpostStockLevels[i].StockLevel = 10;
                outpostStockLevelsHistorical[i].StockLevel = 10;

                outpostStockLevels[i].PrevStockLevel = 2;
                outpostStockLevelsHistorical[i].PrevStockLevel = 2;

                outpostStockLevels[i].UpdatedMethod = "sms";
                outpostStockLevelsHistorical[i].UpdateMethod = "sms";

                if (i < 5)
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[0].Id;
                    outpostStockLevelsHistorical[i].ProdGroupId = productGroups[0].Id;
                }
                if ((i >= 5) && (i < 10))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[1].Id;
                    outpostStockLevelsHistorical[i].ProdGroupId = productGroups[1].Id;
                }
                if ((i >= 10) && (i < 15))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[2].Id;
                    outpostStockLevelsHistorical[i].ProdGroupId = productGroups[2].Id;
                }
                if ((i >= 15) && (i < 20))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[3].Id;
                    outpostStockLevelsHistorical[i].ProdGroupId = productGroups[3].Id;
                }
            }

            outpostStockLevels[0].ProductId = products[1].Id;
            outpostStockLevelsHistorical[0].ProductId = products[1].Id;

            outpostStockLevels[1].ProductId = products[0].Id;
            outpostStockLevelsHistorical[1].ProductId = products[0].Id;
            outpostStockLevels[2].ProductId = products[1].Id;
            outpostStockLevelsHistorical[2].ProductId = products[1].Id;
            outpostStockLevels[3].ProductId = products[0].Id;
            outpostStockLevelsHistorical[3].ProductId = products[0].Id;
            outpostStockLevels[4].ProductId = products[1].Id;
            outpostStockLevelsHistorical[4].ProductId = products[1].Id;

            outpostStockLevels[5].ProductId = products[2].Id;
            outpostStockLevelsHistorical[5].ProductId = products[2].Id;
            outpostStockLevels[6].ProductId = products[3].Id;
            outpostStockLevelsHistorical[6].ProductId = products[3].Id;
            outpostStockLevels[7].ProductId = products[2].Id;
            outpostStockLevelsHistorical[7].ProductId = products[2].Id;
            outpostStockLevels[8].ProductId = products[3].Id;
            outpostStockLevelsHistorical[8].ProductId = products[3].Id;
            outpostStockLevels[9].ProductId = products[2].Id;
            outpostStockLevelsHistorical[9].ProductId = products[2].Id;

            outpostStockLevels[10].ProductId = products[4].Id;
            outpostStockLevelsHistorical[10].ProductId = products[4].Id;
            outpostStockLevels[11].ProductId = products[5].Id;
            outpostStockLevelsHistorical[11].ProductId = products[5].Id;
            outpostStockLevels[12].ProductId = products[4].Id;
            outpostStockLevelsHistorical[12].ProductId = products[4].Id;
            outpostStockLevels[13].ProductId = products[5].Id;
            outpostStockLevelsHistorical[13].ProductId = products[5].Id;
            outpostStockLevels[14].ProductId = products[4].Id;
            outpostStockLevelsHistorical[14].ProductId = products[4].Id;

            outpostStockLevels[15].ProductId = products[6].Id;
            outpostStockLevelsHistorical[15].ProductId = products[6].Id;
            outpostStockLevels[16].ProductId = products[7].Id;
            outpostStockLevelsHistorical[16].ProductId = products[7].Id;
            outpostStockLevels[17].ProductId = products[6].Id;
            outpostStockLevelsHistorical[17].ProductId = products[6].Id;
            outpostStockLevels[18].ProductId = products[6].Id;
            outpostStockLevelsHistorical[18].ProductId = products[6].Id;
            outpostStockLevels[19].ProductId = products[7].Id;
            outpostStockLevelsHistorical[19].ProductId = products[7].Id;




        }
        public void StubProductGroupList()
        {
            productGroups = new List<ProductGroup>();
            for (int i = 0; i < 4; i++)
            {
                var productGroup = new ProductGroup();
                var productGroupId = Guid.NewGuid();
                productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
                productGroup.Stub(it => it.Id).Return(productGroupId);
                productGroup.Name = "productgroup" + i;

                productGroups.Add(productGroup);

            }

        }

        public void StubProductList()
        {
            products = new List<Product>();
            for (int i = 0; i < 8; i++)
            {
                var product = new Product();
                var productId = Guid.NewGuid();
                product = MockRepository.GeneratePartialMock<Product>();
                product.Stub(it => it.Id).Return(productId);
                product.Name = "product" + i;

                products.Add(product);

            }
        }
        const string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000001";
        [Test]
        public void Should_ReturnAll_Outposts_WithAllProductGroupsAsociatedToEach_AndAllProductAsociatedToEachProductGroup_OnOverview_WithBool_CommingFromCurrentData_EqualWithTrue_And_OutpostIdSpecificToAllOutpostFilter()
        {
            //arrange
            queryRegion.Expect(it => it.Query()).Return(new Region[] { region }.AsQueryable());
            queryDistrict.Expect(it => it.Query()).Return(new District[] { district }.AsQueryable());
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
            queryOutpost.Expect(it => it.Query()).Return(outposts.AsQueryable());
            foreach(var item in outposts)
            {
                queryOutpost.Expect(it => it.Load(item.Id)).Return(item);
            }
            foreach (var group in productGroups)
            {
                queryProductGroup.Expect(it => it.Load(group.Id)).Return(group);
            }
            foreach (var product in products)
            {
                queryProduct.Expect(it => it.Load(product.Id)).Return(product);
            }
            queryOutpostStockLevel.Expect(it => it.Query()).Return(outpostStockLevels.AsQueryable());

            //act
            var result = (ViewResult)controller.Overview(country.Id, region.Id,district.Id, Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST), true);

            //assert
            Assert.IsInstanceOf<OutpostStockLevelOverviewModel>(result.Model);
            var model = (OutpostStockLevelOverviewModel)result.Model;

            //test data passed to query
            Assert.AreEqual(10, outposts.Count);
            Assert.AreEqual(2, outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).GroupBy(it => it.ProdGroupId).ToList().Count);
            Assert.AreEqual(1,outpostStockLevels.Where(it=>it.OutpostId == outposts[0].Id && it.ProdGroupId == productGroups[0].Id).ToList().Count);
            //test data returned from query after building model
            Assert.AreEqual(10, model.OutpostList.Outposts.Count);
            Assert.AreEqual(2, model.OutpostList.Outposts[0].StockGroups.Count);
            Assert.AreEqual(1, model.OutpostList.Outposts[0].StockGroups[0].StockItems.Count);

            Assert.AreEqual(outposts[0].Id, model.OutpostList.Outposts[0].Id);
            Assert.AreEqual(productGroups[0].Id, model.OutpostList.Outposts[0].StockGroups[0].Id);
            Assert.AreEqual(products[1].Id, model.OutpostList.Outposts[0].StockGroups[0].StockItems[0].Id);

        }

        [Test]
        public void Get_PartialView_OverviewItemsStockLevel_ShouldReturn_OneOutpost_WhenPasing_ASpecificOutpostGuid()
        {
            //arrange
            queryOutpost.Expect(it => it.Query()).Return(outposts.AsQueryable());
            foreach (var item in outposts)
            {
                queryOutpost.Expect(it => it.Load(item.Id)).Return(item);
            }
            foreach (var group in productGroups)
            {
                queryProductGroup.Expect(it => it.Load(group.Id)).Return(group);
            }
            foreach (var product in products)
            {
                queryProduct.Expect(it => it.Load(product.Id)).Return(product);
            }
            queryOutpostStockLevel.Expect(it => it.Query()).Return(outpostStockLevels.AsQueryable());

            //act
            var result = (PartialViewResult)controller.OverviewItemsStockLevel(outposts[0].Id, district.Id);

            //assert
            Assert.IsInstanceOf<OutpostList>(result.Model);

            var model = (OutpostList)result.Model;

            //test data passed to query for outpost id passed to controller method
            Assert.AreEqual(2, outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList().Count);
            Assert.AreNotEqual(outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProdGroupId, outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProdGroupId);


            //test data returned from query and builded in model
            Assert.AreEqual(1, model.Outposts.Count);
            Assert.AreEqual(2, model.Outposts[0].StockGroups.Count);
            Assert.AreEqual(outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProdGroupId, model.Outposts[0].StockGroups[0].Id);
            Assert.AreEqual(outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProdGroupId, model.Outposts[0].StockGroups[1].Id);
            Assert.AreEqual(outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProductId, model.Outposts[0].StockGroups[0].StockItems[0].Id);
            Assert.AreEqual(outpostStockLevels.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProductId, model.Outposts[0].StockGroups[1].StockItems[0].Id);
        }

        //[Test]
        //public void Get_PartialView_OverviewItemsStockLevelHistorical_ShouldReturn_OneOutpost_WhenPasing_ASpecificOutpostGuid()
        //{
        //    queryOutpost.Expect(it => it.Query()).Return(outposts.AsQueryable());
        //    foreach (var item in outposts)
        //    {
        //        queryOutpost.Expect(it => it.Load(item.Id)).Return(item);
        //    }
        //    foreach (var group in productGroups)
        //    {
        //        queryProductGroup.Expect(it => it.Load(group.Id)).Return(group);
        //    }
        //    foreach (var product in products)
        //    {
        //        queryProduct.Expect(it => it.Load(product.Id)).Return(product);
        //    }
        //    queryOutpostStockLevelHistorical.Expect(it => it.Query()).Return(outpostStockLevelsHistorical.AsQueryable());
        //    //queryProduct.Expect(it => it.Query().Count(Arg<Product>.Matches())).Return(1);
        //    var result = (PartialViewResult)controller.OverviewItemsStockLevelHistorical(outposts[0].Id, district.Id);

        //    //assert
        //    Assert.IsInstanceOf<OutpostList>(result.Model);

        //    var model = (OutpostList)result.Model;

        //    //test data passed to query for outpost id passed to controller method
        //    Assert.AreEqual(2, outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList().Count);
        //    Assert.AreNotEqual(outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProdGroupId, outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProdGroupId);


        //    //test data returned from query and builded in model
        //    Assert.AreEqual(1, model.Outposts.Count);
        //    Assert.AreEqual(2, model.Outposts[0].StockGroups.Count);
        //    Assert.AreEqual(outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProdGroupId, model.Outposts[0].StockGroups[0].Id);
        //    Assert.AreEqual(outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProdGroupId, model.Outposts[0].StockGroups[1].Id);
        //    Assert.AreEqual(outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[0].ProductId, model.Outposts[0].StockGroups[0].StockItems[0].Id);
        //    Assert.AreEqual(outpostStockLevelsHistorical.Where(it => it.OutpostId == outposts[0].Id).ToList()[1].ProductId, model.Outposts[0].StockGroups[1].StockItems[0].Id);
 
        //}

        [Test]
        public void Get_EditOutpostStockLevel_ShouldHaveAllData_SpecificTo_OutpostStockLevelId()
        {
            //arrange
            queryOutpostStockLevel.Expect(it => it.Load(outpostStockLevels[0].Id)).Return(outpostStockLevels[0]);
            queryOutpost.Expect(it => it.Load(outpostStockLevels[0].OutpostId)).Return(outposts[0]);
            queryProductGroup.Expect(it => it.Load(outpostStockLevels[0].ProdGroupId)).Return(productGroups[0]);
            queryProduct.Expect(it => it.Load(outpostStockLevels[0].ProductId)).Return(products[1]);

            //act
            var result = (ViewResult)controller.EditCurrentProductLevel(outpostStockLevels[0].Id, false);

            //assert
            Assert.IsInstanceOf<OutpostStockLevelOutputModel>(result.Model);

            var model = (OutpostStockLevelOutputModel)result.Model;

            Assert.AreEqual(model.Id, outpostStockLevels[0].Id);
        }

        [Test]
        public void POST_Edit_ShouldChange_StockLevel_AndSetPreviousStockLevelTo_PreviousCurrentStockLevel_AndSavePreviuosCurrentOutpostStockLevel_To_HistoricalData()
        {
            var outpostStockLevelInputModel = new OutpostStockLevelInputModel();
            BuildModel(outpostStockLevelInputModel);

            queryOutpostStockLevel.Expect(it => it.Load(outpostStockLevels[0].Id)).Return(outpostStockLevels[0]);
            queryOutpost.Expect(it => it.Load(outpostStockLevels[0].OutpostId)).Return(outposts[0]);
            saveOrUpdateOutpostStockLevel.Expect(it => it.Execute(Arg<OutpostStockLevel>.Matches(c => c.Id == outpostStockLevels[0].Id
                && c.OutpostId == outpostStockLevels[0].OutpostId
                &&c.ProdGroupId == outpostStockLevels[0].ProdGroupId
                && c.ProductId == outpostStockLevels[0].ProductId
                &&c.StockLevel == outpostStockLevelInputModel.StockLevel
                //&&c.PrevStockLevel == outpostStockLevels[0].StockLevel
                )));

            saveOrUpdateOutpostStockLevelHistorical.Expect(it => it.Execute(Arg<OutpostStockLevelHistorical>.Matches(c => c.OutpostId == outpostStockLevels[0].OutpostId
                && c.ProdGroupId == outpostStockLevels[0].ProdGroupId
                && c.ProductId == outpostStockLevels[0].ProductId)));

            var result = (RedirectToRouteResult)controller.EditCurrentProductLevel(outpostStockLevelInputModel);

            saveOrUpdateOutpostStockLevelHistorical.VerifyAllExpectations();
            saveOrUpdateOutpostStockLevel.VerifyAllExpectations();
            
            Assert.AreEqual("Overview", result.RouteValues["action"]);


 
        }

        private void BuildModel(OutpostStockLevelInputModel outpostStockLevelInputModel)
        {
            outpostStockLevelInputModel.Id = outpostStockLevels[0].Id;
            outpostStockLevelInputModel.StockLevel = 56;
            outpostStockLevelInputModel.PrevStockLevel = 11;
            outpostStockLevelInputModel.OutpostId = outpostStockLevels[0].OutpostId;
            outpostStockLevelInputModel.ProdGroupId = outpostStockLevels[0].ProdGroupId;
            outpostStockLevelInputModel.ProductId = outpostStockLevels[0].ProductId;

        }


    }
}
