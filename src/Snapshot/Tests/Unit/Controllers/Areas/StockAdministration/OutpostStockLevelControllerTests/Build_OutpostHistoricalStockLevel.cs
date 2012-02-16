using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController_Test
{
    public class Build_OutpostHistoricalStockLevel
    {
        public List<Outpost> outposts;

        Guid countryId;
        public Country country;
        const String COUNTRY_NAME = "country";

        Guid regionId;
        public Region region;
        const String REGION_NAME = "region";

        Guid districtId;
        public District district;
        const String DISTRICT_NAME = "district";

        public List<OutpostHistoricalStockLevel> outpostHistoricalStockLevels;
        public List<ProductGroup> productGroups;
        public List<Product> products;

        public Build_OutpostHistoricalStockLevel StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = COUNTRY_NAME;
            return this;
        }
        public Build_OutpostHistoricalStockLevel StubRegionForCountry()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
            return this;
        }

        public Build_OutpostHistoricalStockLevel StubDistrictForRegion()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
            return this;
        }

        public Build_OutpostHistoricalStockLevel StubOutpostListWithPreviuosCountryRegionAndDistrict()
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
            return this;
        }
        public void StubOutpostHistoricalStockLevelList()
        {
            outpostHistoricalStockLevels = new List<OutpostHistoricalStockLevel>();

            for (int i = 0; i < 20; i++)
            {
                var outpostHistoricalStockLevel = new OutpostHistoricalStockLevel();


                var outpostStockLevelHistoricalId = Guid.NewGuid();
                outpostHistoricalStockLevel = MockRepository.GeneratePartialMock<OutpostHistoricalStockLevel>();
                outpostHistoricalStockLevel.Stub(it => it.Id).Return(outpostStockLevelHistoricalId);
                //from 0-9 assign 0-9 outposts
                if (i < 10)
                {
                    outpostHistoricalStockLevel.OutpostId = outposts[i].Id;


                } //from 10 -19 assign outposts with index from 0-9
                else
                {
                    outpostHistoricalStockLevel.OutpostId = outposts[i - 10].Id;

                }
                outpostHistoricalStockLevels.Add(outpostHistoricalStockLevel);

            }

            for (int i = 0; i < outpostHistoricalStockLevels.Count; i++)
            {
                outpostHistoricalStockLevels[i].StockLevel = 10;


                outpostHistoricalStockLevels[i].PrevStockLevel = 2;


                outpostHistoricalStockLevels[i].UpdateMethod = "sms";


                if (i < 5)
                {
                    outpostHistoricalStockLevels[i].ProdGroupId = productGroups[0].Id;

                }
                if ((i >= 5) && (i < 10))
                {
                    outpostHistoricalStockLevels[i].ProdGroupId = productGroups[1].Id;

                }
                if ((i >= 10) && (i < 15))
                {
                    outpostHistoricalStockLevels[i].ProdGroupId = productGroups[2].Id;

                }
                if ((i >= 15) && (i < 20))
                {
                    outpostHistoricalStockLevels[i].ProdGroupId = productGroups[3].Id;

                }
            }

            outpostHistoricalStockLevels[0].ProductId = products[1].Id;
            outpostHistoricalStockLevels[1].ProductId = products[0].Id;
            outpostHistoricalStockLevels[2].ProductId = products[1].Id;
            outpostHistoricalStockLevels[3].ProductId = products[0].Id;
            outpostHistoricalStockLevels[4].ProductId = products[1].Id;


            outpostHistoricalStockLevels[5].ProductId = products[2].Id;
            outpostHistoricalStockLevels[6].ProductId = products[3].Id;
            outpostHistoricalStockLevels[7].ProductId = products[2].Id;
            outpostHistoricalStockLevels[8].ProductId = products[3].Id;
            outpostHistoricalStockLevels[9].ProductId = products[2].Id;


            outpostHistoricalStockLevels[10].ProductId = products[4].Id;
            outpostHistoricalStockLevels[11].ProductId = products[5].Id;
            outpostHistoricalStockLevels[12].ProductId = products[4].Id;
            outpostHistoricalStockLevels[13].ProductId = products[5].Id;
            outpostHistoricalStockLevels[14].ProductId = products[4].Id;


            outpostHistoricalStockLevels[15].ProductId = products[6].Id;
            outpostHistoricalStockLevels[16].ProductId = products[7].Id;
            outpostHistoricalStockLevels[17].ProductId = products[6].Id;
            outpostHistoricalStockLevels[18].ProductId = products[6].Id;
            outpostHistoricalStockLevels[19].ProductId = products[7].Id;

        }
        public Build_OutpostHistoricalStockLevel StubProductGroupList()
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
            return this;

        }

        public Build_OutpostHistoricalStockLevel StubProductList()
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
            return this;
        }
    }
}
