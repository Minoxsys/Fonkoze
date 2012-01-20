using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersist_A_OutpostStockLevelHistorical:GivenAPersistenceSpecification<OutpostHistoricalStockLevel>
    {
        Guid OUTPOST_ID = Guid.NewGuid();
        Guid PRODUCT_ID = Guid.NewGuid();
        Guid PRODUCT_GROUPID = Guid.NewGuid();

        const string PROD_SMS_REF = "dd";
        const string UPDATE_METHOD = "manually";
        DateTime UPDATE_DATE = DateTime.Today;
        const int STOCK_LEVEL = 1;
        const int PREV_STOCK_LEVEL = 33;

        [Test]
        public void It_Should_Successfully_Persist_An_OutpostStockLevelHistorical()
        {
            var outpostStockLevelHistorical = Specs.CheckProperty(it => it.OutpostId, OUTPOST_ID)
                .CheckProperty(it => it.PrevStockLevel, PREV_STOCK_LEVEL)
                .CheckProperty(it => it.ProdGroupId, PRODUCT_GROUPID)
                .CheckProperty(it => it.ProdSmsRef, PROD_SMS_REF)
                .CheckProperty(it => it.ProductId, PRODUCT_ID)
                .CheckProperty(it => it.StockLevel, STOCK_LEVEL)
                .CheckProperty(it=>it.UpdateDate,UPDATE_DATE)
                .CheckProperty(it => it.UpdateMethod, UPDATE_METHOD)
                .VerifyTheMappings();

            Assert.IsNotNull(outpostStockLevelHistorical);
            Assert.IsInstanceOf<Guid>(outpostStockLevelHistorical.Id);
            Assert.AreEqual(outpostStockLevelHistorical.OutpostId, OUTPOST_ID);
            Assert.AreEqual(outpostStockLevelHistorical.PrevStockLevel, PREV_STOCK_LEVEL);
            Assert.AreEqual(outpostStockLevelHistorical.ProdGroupId, PRODUCT_GROUPID);
            Assert.AreEqual(outpostStockLevelHistorical.ProdSmsRef, PROD_SMS_REF);
            Assert.AreEqual(outpostStockLevelHistorical.ProductId, PRODUCT_ID);
            Assert.AreEqual(outpostStockLevelHistorical.StockLevel, STOCK_LEVEL);


            session.Delete(outpostStockLevelHistorical);
            session.Flush();
        }

       



    }
}
