using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController_Test
{
    public class Build_OutpostStockLevel
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

        public List<OutpostStockLevel> outpostStockLevels;
        public List<ProductGroup> productGroups;
        public List<Product> products;

        public Build_OutpostStockLevel StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = COUNTRY_NAME;
            return this;
        }
        public Build_OutpostStockLevel StubRegionForCountry()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
            return this;
        }

        public Build_OutpostStockLevel StubDistrictForRegion()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
            return this;
        }

        public Build_OutpostStockLevel StubOutpostListWithPreviuosCountryRegionAndDistrict()
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
        public void StubOutpostStockLevelList()
        {
            outpostStockLevels = new List<OutpostStockLevel>();
           
            for (int i = 0; i < 20; i++)
            {
                var outpostStockLevel = new OutpostStockLevel();
              

                var outpostStockLevelHistoricalId = Guid.NewGuid();
                

                var outpostStockLevelId = Guid.NewGuid();
                outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
                outpostStockLevel.Stub(it => it.Id).Return(outpostStockLevelId);
                //from 0-9 assign 0-9 outposts
                if (i < 10)
                {
                    outpostStockLevel.OutpostId = outposts[i].Id;
                    

                } //from 10 -19 assign outposts with index from 0-9
                else
                {
                    outpostStockLevel.OutpostId = outposts[i - 10].Id;
                   
                }
                outpostStockLevels.Add(outpostStockLevel);
               
            }

            for (int i = 0; i < outpostStockLevels.Count; i++)
            {
                outpostStockLevels[i].StockLevel = 10;
               

                outpostStockLevels[i].PrevStockLevel = 2;
                

                outpostStockLevels[i].UpdatedMethod = "sms";
               

                if (i < 5)
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[0].Id;
                    
                }
                if ((i >= 5) && (i < 10))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[1].Id;
                    
                }
                if ((i >= 10) && (i < 15))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[2].Id;
                    
                }
                if ((i >= 15) && (i < 20))
                {
                    outpostStockLevels[i].ProdGroupId = productGroups[3].Id;
                    
                }
            }

            outpostStockLevels[0].ProductId = products[1].Id;      
            outpostStockLevels[1].ProductId = products[0].Id;           
            outpostStockLevels[2].ProductId = products[1].Id;            
            outpostStockLevels[3].ProductId = products[0].Id;
            outpostStockLevels[4].ProductId = products[1].Id;
            

            outpostStockLevels[5].ProductId = products[2].Id;            
            outpostStockLevels[6].ProductId = products[3].Id;            
            outpostStockLevels[7].ProductId = products[2].Id;            
            outpostStockLevels[8].ProductId = products[3].Id;            
            outpostStockLevels[9].ProductId = products[2].Id;


            outpostStockLevels[10].ProductId = products[4].Id;            
            outpostStockLevels[11].ProductId = products[5].Id;            
            outpostStockLevels[12].ProductId = products[4].Id;            
            outpostStockLevels[13].ProductId = products[5].Id;            
            outpostStockLevels[14].ProductId = products[4].Id;
            

            outpostStockLevels[15].ProductId = products[6].Id;            
            outpostStockLevels[16].ProductId = products[7].Id;            
            outpostStockLevels[17].ProductId = products[6].Id;            
            outpostStockLevels[18].ProductId = products[6].Id;            
            outpostStockLevels[19].ProductId = products[7].Id;
       
        }
        public Build_OutpostStockLevel StubProductGroupList()
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

        public Build_OutpostStockLevel StubProductList()
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
