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
    public class When_WeWantToPersist_A_StockGroup : GivenAPersistenceSpecification<ProductGroup>
    {
        private const string NAME = "StockGroup1";
        private const string DESCRIPTION = "Description1";

        private Product StockItem = new Product();
        private List<Product> StockItems = new List<Product>();
        

        [Test]
        public void It_Should_Successfully_Persist_A_StockGroup()
        {
           var stockGroup = Specs.CheckProperty(it => it.Description, DESCRIPTION)
                .CheckProperty(it => it.Name, NAME)
                .VerifyTheMappings();

            Assert.IsNotNull(stockGroup);
            Assert.IsInstanceOf<Guid>(stockGroup.Id);
            Assert.AreEqual(stockGroup.Name, NAME);
           
            session.Delete(stockGroup);
            session.Flush();
        }
    }
}
