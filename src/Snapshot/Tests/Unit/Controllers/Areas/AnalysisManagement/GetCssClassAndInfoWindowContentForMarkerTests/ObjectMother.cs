using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Persistence;
using Domain;
using MvcContrib.TestHelper.Fakes;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GetCssClassAndInfoWindowContentForMarkerTests
{
    public class ObjectMother
    {
        public LocationReportController controller;

        public IQueryService<OutpostStockLevel> queryStockLevel;

        public Client client;
        public Guid clientId;

        public Guid countryId;
        public Country country;
        public Guid regionId;
        public Region region;
        public Guid districtId;
        public District district;
        public Guid outpostId;
        public Outpost outpost;
        public Guid stockLevelId;
      
       
        public Guid productId;
        public Product product;

        public Guid oslRedId;
        public Guid oslAmberId;
        public Guid oslGreenId;
        public OutpostStockLevel oslRed;
        public OutpostStockLevel oslAmber;
        public OutpostStockLevel oslGreen;

        public List<OutpostStockLevel> oslNoStockLevelRecordsList;
        public List<OutpostStockLevel> oslRedList;
        public List<OutpostStockLevel> oslAmberList;
        public List<OutpostStockLevel> oslGreenList;

       

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();                      
        }
        private void MockServices()
        {
            queryStockLevel = MockRepository.GenerateMock<IQueryService<OutpostStockLevel>>();
        }
        private void Setup_Controller()
        {
            controller = new LocationReportController();
            controller.QueryStockLevel = queryStockLevel;
        }
        private void SetUp_StubData()
        {

            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = "";

            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = "Country";
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Region";
            region.Country = country;
            region.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = "District";
            district.Region = region;
            district.Client = client;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "District";
            outpost.Country = country;
            outpost.Region = region;
            outpost.District = district;
            outpost.Client = client;

            oslNoStockLevelRecordsList = new List<OutpostStockLevel>();
            oslRedList = new List<OutpostStockLevel>();
            oslAmberList = new List<OutpostStockLevel>();
            oslGreenList = new List<OutpostStockLevel>();

            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(c => c.Id).Return(productId);
            product.LowerLimit = 10;

            oslRedId = Guid.NewGuid();
            oslRed = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            oslRed.Stub(c => c.Id).Return(oslRedId);
            oslRed.Client = client;
            oslRed.Outpost = outpost;
            oslRed.Product = product;
            oslRed.StockLevel = 1;

            oslRedList.Add(oslRed);

            oslAmberId = Guid.NewGuid();
            oslAmber = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            oslAmber.Stub(c => c.Id).Return(oslRedId);
            oslAmber.Client = client;
            oslAmber.Outpost = outpost;
            oslAmber.Product = product;
            oslAmber.StockLevel = 11;

            oslAmberList.Add(oslAmber);

            oslGreenId = Guid.NewGuid();
            oslGreen = MockRepository.GeneratePartialMock<OutpostStockLevel>();
            oslGreen.Stub(c => c.Id).Return(oslRedId);
            oslGreen.Client = client;
            oslGreen.Outpost = outpost;
            oslGreen.Product = product;
            oslGreen.StockLevel = 15;

            oslGreenList.Add(oslGreen);

            
             




        }
    }
}
