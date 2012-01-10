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
    public class When_WeWantToPersist_An_Product : GivenAPersistenceSpecification<Product>
    {
        private const string NAME = "StockItem1";
        private const string DESCRIPTION = "Description1";
        private const string SMS_REFERENCE_CODE = "S";
        private const int UPPERLIMIT = 1000;
        private const int LOWERLIMIT = 3;
        private const int PREVIOUS_STOCK_LEVEL = 22;
        private const string UPDATE_METHOD = "SMS";
        private const int STOCK_LEVEL = 11;

        private ProductGroup ProductGroup = new ProductGroup();
        private Outpost Outpost = new Outpost();
               

        [Test]
        public void It_Should_Successfully_Persist_An_StockItem()
        {
           
            var product = Specs.CheckProperty(it => it.Description, DESCRIPTION)
                .CheckProperty(it => it.LowerLimit, LOWERLIMIT)
                .CheckProperty(it => it.UpperLimit, UPPERLIMIT)
                .CheckProperty(it => it.Name, NAME)
                .CheckProperty(it =>it.PreviousStockLevel, PREVIOUS_STOCK_LEVEL)
                .CheckProperty(it =>it.StockLevel,STOCK_LEVEL)
                .CheckProperty(it =>it.UpdateMethod,UPDATE_METHOD)
                .CheckProperty(it => it.SMSReferenceCode, SMS_REFERENCE_CODE)
                .CheckReference(it => it.ProductGroup, ProductGroup)
                .CheckReference(it=>it.Outpost,Outpost)
                .VerifyTheMappings();

            Assert.IsNotNull(product);
            Assert.IsInstanceOf<Guid>(product.Id);
            Assert.AreEqual(product.Name, NAME);
            Assert.IsInstanceOf<ProductGroup>(product.ProductGroup);
            
            session.Delete(product);
            session.Flush();
        }
    }
}
