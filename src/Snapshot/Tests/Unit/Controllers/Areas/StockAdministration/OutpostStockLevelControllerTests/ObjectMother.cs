using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;
using Core.Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController
{
    public class ObjectMother
    {
        public Web.Areas.StockAdministration.Controllers.OutpostStockLevelController controller;

        public IQueryService<Product> queryProduct;
        public IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        public IQueryService<OutpostHistoricalStockLevel> queryOutpostStockLevelHistorical;
        public IQueryService<ProductGroup> queryProductGroup;
        public IQueryService<Outpost> queryOutpost;
        public IQueryService<Country> queryCountry;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;

        public ISaveOrUpdateCommand<OutpostStockLevel> saveOrUpdateOutpostStockLevel;
        public ISaveOrUpdateCommand<OutpostHistoricalStockLevel> saveOrUpdateOutpostStockLevelHistorical;
        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;

        public List<Outpost> outposts;
        public Client client;
        public Guid clientId;
        public User user;

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
        const string USERNAME = "admin";
        public string GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST = "00000000-0000-0000-0000-000000000001";
       
        public void Init()
        {
            BuildControllerAndServices();
            StubCountry();
            StubUserAndItsClient();
            StubRegionForCountry();
            StubDistrictForRegion();
            StubOutpostListWithPreviuosCountryRegionAndDistrict();
            StubProductList();
            StubProductGroupList();
            StubOutpostStockLevelList();
            

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
            queryOutpostStockLevelHistorical = MockRepository.GenerateMock<IQueryService<OutpostHistoricalStockLevel>>();
            saveOrUpdateOutpostStockLevel = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostStockLevel>>();
            saveOrUpdateOutpostStockLevelHistorical = MockRepository.GenerateMock<ISaveOrUpdateCommand<OutpostHistoricalStockLevel>>();

            controller = new Web.Areas.StockAdministration.Controllers.OutpostStockLevelController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);

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

        internal void StubCountry()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(b => b.Id).Return(countryId);
            country.Name = COUNTRY_NAME;
            
        }
        internal void StubUserAndItsClient()
        {

            queryClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            this.client = MockRepository.GeneratePartialMock<Client>();
            clientId = Guid.NewGuid();

            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Minoxsys";

            this.user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USERNAME;
            user.Password = "asdf";

            queryClient.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            controller.QueryClients = queryClient;
            controller.QueryUsers = queryUsers;

        }
        internal void StubRegionForCountry()
        {
            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(b => b.Id).Return(regionId);
            region.Name = REGION_NAME;
            region.Country = country;
            
        }

        internal void StubDistrictForRegion()
        {
            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(b => b.Id).Return(districtId);
            district.Name = DISTRICT_NAME;
            district.Region = region;
            
        }

        internal void StubOutpostListWithPreviuosCountryRegionAndDistrict()
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
        internal void StubOutpostStockLevelList()
        {
            outpostStockLevels = new List<OutpostStockLevel>();

            for (int i = 0; i < 20; i++)
            {
                var outpostStockLevel = new OutpostStockLevel();
                
                 var outpostStockLevelId = Guid.NewGuid();
                outpostStockLevel = MockRepository.GeneratePartialMock<OutpostStockLevel>();
                outpostStockLevel.Stub(it => it.Id).Return(outpostStockLevelId);
                //from 0-9 assign 0-9 outposts
                if (i < 10)
                {
                    outpostStockLevel.OutpostId = outposts[i].Id;
                    outpostStockLevel.Client = client;

                } //from 10 -19 assign outposts with index from 0-9
                else
                {
                    outpostStockLevel.OutpostId = outposts[i - 10].Id;
                    outpostStockLevel.Client = client;
                }
                outpostStockLevels.Add(outpostStockLevel);

            }

            for (int i = 0; i < outpostStockLevels.Count; i++)
            {
                outpostStockLevels[i].StockLevel = 10;


                outpostStockLevels[i].PrevStockLevel = 2;


                outpostStockLevels[i].UpdateMethod = "sms";


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
        internal void StubProductGroupList()
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
        internal void StubProductList()
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

        
    }
}
