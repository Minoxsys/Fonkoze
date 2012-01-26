using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController_Test
{
    [TestFixture]
    public class Method_Overview
    {
        Build_OutpostStockLevel build_OutpostStockLevel = new Build_OutpostStockLevel();
        Build_OutpostHistoricalStockLevel build_OutpostHistoricalStocklevel = new Build_OutpostHistoricalStockLevel();
        Build_ServicesAndController build_ServicesAndController = new Build_ServicesAndController();
        const string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000001";

        [Test]
        public void Should_ReturnAll_Outposts_WithAllProductGroupsAsociatedToEach_AndAllProductAsociatedToEachProductGroup_OnOverview_WithBool_CommingFromCurrentData_EqualWithTrue_And_OutpostIdSpecificToAllOutpostFilter()
        {
            //arrange
            build_OutpostStockLevel.StubCountry()
                .StubRegionForCountry()
                .StubDistrictForRegion()
                .StubOutpostListWithPreviuosCountryRegionAndDistrict()
                .StubProductGroupList()
                .StubProductList()
                .StubOutpostStockLevelList();

            build_ServicesAndController.BuildControllerAndServices();

            build_ServicesAndController.ExpectQueryRegionAndReturn(build_OutpostStockLevel.region);
            build_ServicesAndController.ExpectQueryDistrictAndReturn(build_OutpostStockLevel.district);
            build_ServicesAndController.ExpectQueryCountryAndReturn(build_OutpostStockLevel.country);
            build_ServicesAndController.ExpectQueryOutpostAndReturn(build_OutpostStockLevel.outposts);

            foreach (var item in build_OutpostStockLevel.outposts)
            {
                build_ServicesAndController.ExpectLoadOutpostAndReturn(item);
            }
            foreach (var group in build_OutpostStockLevel.productGroups)
            {
                build_ServicesAndController.ExpectLoadProductGroupAndReturn(group);
            }
            foreach (var product in build_OutpostStockLevel.products)
            {
                build_ServicesAndController.ExpectLoadProductAndReturn(product);
            }
            build_ServicesAndController.ExpectQueryOutpostStockLevelAndReturn(build_OutpostStockLevel.outpostStockLevels);
           
            //act
            var result = (ViewResult)build_ServicesAndController.controller.Overview(build_OutpostStockLevel.country.Id, build_OutpostStockLevel.region.Id, build_OutpostStockLevel.district.Id, Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST), true);

            //assert
            Assert.IsInstanceOf<OutpostStockLevelOverviewModel>(result.Model);
            var model = (OutpostStockLevelOverviewModel)result.Model;

            //test data passed to query
            Assert.AreEqual(10, build_OutpostStockLevel.outposts.Count);
            Assert.AreEqual(2, build_OutpostStockLevel.outpostStockLevels.Where(it => it.OutpostId == build_OutpostStockLevel.outposts[0].Id).GroupBy(it => it.ProdGroupId).ToList().Count);
            Assert.AreEqual(1, build_OutpostStockLevel.outpostStockLevels.Where(it => it.OutpostId == build_OutpostStockLevel.outposts[0].Id && it.ProdGroupId == build_OutpostStockLevel.productGroups[0].Id).ToList().Count);
            //test data returned from query after building model
            Assert.AreEqual(10, model.OutpostList.Outposts.Count);
            Assert.AreEqual(2, model.OutpostList.Outposts[0].StockGroups.Count);
            Assert.AreEqual(1, model.OutpostList.Outposts[0].StockGroups[0].StockItems.Count);

            Assert.AreEqual(build_OutpostStockLevel.outposts[0].Id, model.OutpostList.Outposts[0].Id);
            Assert.AreEqual(build_OutpostStockLevel.productGroups[0].Id, model.OutpostList.Outposts[0].StockGroups[0].Id);
            Assert.AreEqual(build_OutpostStockLevel.products[1].Id, model.OutpostList.Outposts[0].StockGroups[0].StockItems[0].Id);

        }
        [Test]
        public void Should_ReturnAll_Outposts_WithAllProductGroupsAsociatedToEach_AndAllProductAsociatedToEachProductGroup_OnOverview_WithBool_CommingFromCurrentData_EqualWithFalse_And_OutpostIdSpecificToAllOutpostFilter()
        {
            build_OutpostHistoricalStocklevel.StubCountry()
                .StubRegionForCountry()
                .StubDistrictForRegion()
                .StubOutpostListWithPreviuosCountryRegionAndDistrict()
                .StubProductGroupList()
                .StubProductList()
                .StubOutpostHistoricalStockLevelList();

            build_ServicesAndController.BuildControllerAndServices();

            build_ServicesAndController.ExpectQueryRegionAndReturn(build_OutpostHistoricalStocklevel.region);
            build_ServicesAndController.ExpectQueryDistrictAndReturn(build_OutpostHistoricalStocklevel.district);
            build_ServicesAndController.ExpectQueryCountryAndReturn(build_OutpostHistoricalStocklevel.country);
            build_ServicesAndController.ExpectQueryOutpostAndReturn(build_OutpostHistoricalStocklevel.outposts);

            foreach (var item in build_OutpostHistoricalStocklevel.outposts)
            {
                build_ServicesAndController.ExpectLoadOutpostAndReturn(item);
                foreach (var product in build_OutpostHistoricalStocklevel.products)
                {
                    build_ServicesAndController.ExpectQueryProductCountAndReturn(product);
                }
            }
            foreach (var group in build_OutpostHistoricalStocklevel.productGroups)
            {
                build_ServicesAndController.ExpectLoadProductGroupAndReturn(group);
            }
            foreach (var product in build_OutpostHistoricalStocklevel.products)
            {
                build_ServicesAndController.ExpectLoadProductAndReturn(product);
               
            }
            build_ServicesAndController.ExpectQueryOutpostHistoricalStockLevelAndReturn(build_OutpostHistoricalStocklevel.outpostHistoricalStockLevels);
           
            //act
            var result = (ViewResult)build_ServicesAndController.controller.Overview(build_OutpostHistoricalStocklevel.country.Id, build_OutpostHistoricalStocklevel.region.Id, build_OutpostHistoricalStocklevel.district.Id, Guid.Parse(GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST), false);

            //assert
            Assert.IsInstanceOf<OutpostStockLevelOverviewModel>(result.Model);
            var model = (OutpostStockLevelOverviewModel)result.Model;

            //test data passed to query
            Assert.AreEqual(10, build_OutpostHistoricalStocklevel.outposts.Count);
            Assert.AreEqual(2, build_OutpostHistoricalStocklevel.outpostHistoricalStockLevels.Where(it => it.OutpostId == build_OutpostHistoricalStocklevel.outposts[0].Id).GroupBy(it => it.ProdGroupId).ToList().Count);
            
            //test data returned from query after building model
            Assert.AreEqual(10, model.OutpostList.Outposts.Count);
            Assert.AreEqual(2, model.OutpostList.Outposts[0].StockGroups.Count);                      

            Assert.AreEqual(build_OutpostHistoricalStocklevel.outposts[0].Id, model.OutpostList.Outposts[0].Id);
            Assert.AreEqual(build_OutpostHistoricalStocklevel.productGroups[0].Id, model.OutpostList.Outposts[0].StockGroups[0].Id);
            
            
        }
    }
}
