using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController_Test
{
    public class Build_ServicesAndController
    {
        public OutpostStockLevelController controller;

        IQueryService<Product> queryProduct;
        IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        IQueryService<OutpostHistoricalStockLevel> queryOutpostStockLevelHistorical;
        IQueryService<ProductGroup> queryProductGroup;
        IQueryService<Outpost> queryOutpost;
        IQueryService<Country> queryCountry;
        IQueryService<Region> queryRegion;
        IQueryService<District> queryDistrict;

        ISaveOrUpdateCommand<OutpostStockLevel> saveOrUpdateOutpostStockLevel;
        ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveOrUpdateOutpostStockLevelHistorical;

        public void BuildControllerAndServices()
        {
            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryProduct = MockRepository.GenerateMock<IQueryService<Product>>();
            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryOutpostStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
            queryOutpostStockLevelHistorical = MockRepository.GenerateMock<IQueryService<OutpostHistoricalStockLevel>>();
            saveOrUpdateOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            saveOrUpdateOutpostStockLevelHistorical = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostHistoricalStockLevel>>();

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
        public void ExpectQueryRegionAndReturn(Region region)
        {
            queryRegion.Expect(it => it.Query()).Return(new Region[] { region }.AsQueryable());
        }
        public void ExpectQueryDistrictAndReturn(District district)
        {
            queryDistrict.Expect(it => it.Query()).Return(new District[] { district }.AsQueryable());
        }
        public void ExpectQueryCountryAndReturn(Country country)
        {
            queryCountry.Expect(it => it.Query()).Return(new Country[] { country }.AsQueryable());
        }

        public void ExpectQueryOutpostAndReturn(List<Outpost> outposts)
        {
            queryOutpost.Expect(it => it.Query()).Return(outposts.AsQueryable());
        }
        public void ExpectLoadOutpostAndReturn(Outpost outpost)
        {
            queryOutpost.Expect(it => it.Load(outpost.Id)).Return(outpost);
        }
        public void ExpectLoadProductGroupAndReturn(ProductGroup productGroup)
        {
            queryProductGroup.Expect(it => it.Load(productGroup.Id)).Return(productGroup);
        }

        public void ExpectLoadProductAndReturn(Product product)
        {
            queryProduct.Expect(it => it.Load(product.Id)).Return(product);
        }
        public void ExpectQueryOutpostStockLevelAndReturn(List<OutpostStockLevel> outpostStockLevels)
        {
            queryOutpostStockLevel.Expect(it => it.Query()).Return(outpostStockLevels.AsQueryable());
        }
        public void ExpectQueryOutpostHistoricalStockLevelAndReturn(List<OutpostHistoricalStockLevel> outpostStockLevelHistorical)
        {
            queryOutpostStockLevelHistorical.Expect(it => it.Query()).Return(outpostStockLevelHistorical.AsQueryable()); 
        }
        public void ExpectQueryProductCountAndReturn(Product product)
        {
            queryProduct.Expect(call => call.Query()).Repeat.Once().Return(new Product[] { product }.AsQueryable());
        }
    }
}
