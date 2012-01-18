using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Controllers;
using Domain;
using Core.Persistence;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.StockAdministration
{
    [TestFixture]
    public class OutpostStockLevelController_Tests
    {
        Guid outpostId1;
        Outpost outpost1;
        const String OUTPOSTNAME1 = "outpost1";

        Guid outpostId2;
        Outpost outpost2;
        const String OUTPOSTNAME2 = "outpost2";

        Guid outpostId3;
        Outpost outpost3;
        const String OUTPOSTNAME3 = "outpost3";

        Guid outpostId4;
        Outpost outpost4;
        const string OUTPOSTNAME4 = "outpost4";

        Guid outpostId5;
        Outpost outpost5;
        const String OUTPOSTNAME5 = "outpost5";

        Guid outpostId6;
        Outpost outpost6;
        const String OUTPOSTNAME6 = "outpost6";

        Guid outpostId7;
        Outpost outpost7;
        const String OUTPOSTNAME7 = "outpost7";

        Guid outpostId8;
        Outpost outpost8;
        const String OUTPOSTNAME8 = "outpost8";

        Guid outpostId9;
        Outpost outpost9;
        const String OUTPOSTNAME9 = "outpost9";

        Guid outpostId10;
        Outpost outpost10;
        const String OUTPOSTNAME10 = "outpost10";

        Guid outpostId11;
        Outpost outpost11;
        const String OUTPOSTNAME11 = "outpost11";

        Guid outpostId12;
        Outpost outpost12;
        const String OUTPOSTNAME12 = "outpost12";

        Guid outpostId13;
        Outpost outpost13;
        const String OUTPOSTNAME13 = "outpost13";

        Guid outpostId14;
        Outpost outpost14;
        const String OUTPOSTNAME14 = "outpost14";

        Guid outpostId15;
        Outpost outpost15;
        const String OUTPOSTNAME15 = "outpost15";

        Guid outpostId16;
        Outpost outpost16;
        const String OUTPOSTNAME16 = "outpost16";

        Guid outpostId17;
        Outpost outpost17;
        const String OUTPOSTNAME17 = "outpost17";

        Guid outpostId18;
        Outpost outpost18;
        const String OUTPOSTNAME18 = "outpost18";

        Guid outpostId19;
        Outpost outpost19;
        const String OUTPOSTNAME19 = "outpost19";

        Guid outpostId20;
        Outpost outpost20;
        const String OUTPOSTNAME20 = "outpost20";

        Guid districtId1;
        District district1;

        Guid districtId2;
        District district2;

        Guid districtId3;
        District district3;

        Guid districtId4;
        District district4;

        Guid regionId1;
        Region region1;

        Guid regionId2;
        Region region2;

        Guid countryId1;
        Country country1;

        OutpostStockLevelController controller;

        IQueryService<Product> queryProduct;
        IQueryService<OutpostStockLevel> queryOutpostStockLevel;
        IQueryService<OutpostStockLevelHistorical> queryOutpostStockLevelHistorical;
        IQueryService<ProductGroup> queryProductGroup;
        IQueryService<Outpost> queryOutpost;

        ISaveOrUpdateCommand<OutpostStockLevel> saveOrUpdateOutpostStockLevel;
        ISaveOrUpdateCommand<OutpostStockLevelHistorical> saveOrUpdateOutpostStockLevelHistorical;



        [SetUp]
        public void BeforeAll()
        {
            BuildControllerAndServices();

        }

        public void BuildControllerAndServices()
        {
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
        }

        public void StubCountry()
        {

        }

    }
}
