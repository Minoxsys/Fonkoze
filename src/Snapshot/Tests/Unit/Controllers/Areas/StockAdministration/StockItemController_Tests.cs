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
using Web.Areas.StockAdministration.Models.Product;

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

        ProductGroup stockGroup;
        Product stockItem;
        Outpost outpost;

        Guid stockItemId;
        Guid stockGroupId;
        Guid outpostId;

        ProductsController controller;

 
        
    }
}
