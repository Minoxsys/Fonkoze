using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Areas.StockAdministration.Models.OutpostStockLevel;
using Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.OutpostStockLevelController_Test
{
    [TestFixture]
    public class Method_Overview
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.Init();

        }

        [Test]
        public void Should_Return_OutpostStockLevelTreeData_With_AllOutposts_BecauseOf_GUID_FORALLOUTPOST_PROVIDED()
        {
            var overviewModel = new OverviewInputModel() {
                OutpostId = Guid.Parse(objectMother.GUID_FOR_ALL_OPTION_ON_OUTPOST_LIST),
                DistrictId = objectMother.district.Id
            };

            objectMother.queryOutpost.Expect(it => it.Query()).Return(objectMother.outposts.AsQueryable());
            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevels.AsQueryable());
            foreach (var productGroup in objectMother.productGroups)
            {
                objectMother.queryProductGroup.Expect(it => it.Load(productGroup.Id)).Return(productGroup);
            }
            foreach (var product in objectMother.products)
            {
                objectMother.queryProduct.Expect(it => it.Load(product.Id)).Return(product);
            }

            var result = objectMother.controller.GetOutpostStockLevelData(overviewModel);

            Assert.IsInstanceOf<OutpostStockLevelCurrentTreeModel>(result.Data);

            var treeModelData = (OutpostStockLevelCurrentTreeModel)result.Data;

            Assert.AreEqual(treeModelData.children.Count, objectMother.outposts.Count);
            Assert.AreEqual(treeModelData.children[0].children.Count, 2);
            Assert.AreEqual(treeModelData.children[0].children[0].children[0].leaf, true);
            Assert.AreEqual(treeModelData.children[0].children[0].children[0].Id, objectMother.outpostStockLevels[0].Id);
        }

        [Test]
        public void Should_Return_OutpostStockLevelTreeData_With_OneOutpost_BecauseOf_GuidSpecificToThatOutpost_Provided()
        {
            var overviewModel = new OverviewInputModel()
            {
                OutpostId = objectMother.outposts[0].Id,
                DistrictId = objectMother.district.Id
            };

            objectMother.queryOutpostStockLevel.Expect(it => it.Query()).Return(objectMother.outpostStockLevels.Where(it=>it.Outpost.Id == objectMother.outposts[0].Id).AsQueryable());
            objectMother.queryOutpost.Expect(it => it.Load(objectMother.outposts[0].Id)).Return(objectMother.outposts[0]);
            foreach (var productGroup in objectMother.productGroups)
            {
                objectMother.queryProductGroup.Expect(it => it.Load(productGroup.Id)).Return(productGroup);
            }
            foreach (var product in objectMother.products)
            {
                objectMother.queryProduct.Expect(it => it.Load(product.Id)).Return(product);
            }

            var result = objectMother.controller.GetOutpostStockLevelData(overviewModel);

            Assert.IsInstanceOf<OutpostStockLevelCurrentTreeModel>(result.Data);

            var treeModelData = (OutpostStockLevelCurrentTreeModel)result.Data;

            Assert.AreEqual(treeModelData.children.Count, 1);
            Assert.AreEqual(treeModelData.children[0].children.Count, 2);
            Assert.AreEqual(treeModelData.children[0].children[0].children[0].leaf, true);
            Assert.AreEqual(treeModelData.children[0].children[0].children[0].Id, objectMother.outpostStockLevels[0].Id);
        }
        
        
    }
}
