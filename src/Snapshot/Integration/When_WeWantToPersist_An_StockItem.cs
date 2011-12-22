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
    public class When_WeWantToPersist_An_StockItem : GivenAPersistenceSpecification<StockItem>
    {
        private const string NAME = "StockItem1";
        private const string DESCRIPTION = "Description1";
        private const string SMS_REFERENCE_CODE = "S";
        private const int UPPERLIMIT = 1000;
        private const int LOWERLIMIT = 3;

        private StockGroup StockGroup = new StockGroup();
        private Outpost Outpost = new Outpost { Name = "Outpost" };
        private List<Outpost> Outposts = new List<Outpost>();
        

        [Test]
        public void It_Should_Successfully_Persist_An_StockItem()
        {
            Outposts.Add(Outpost);
            var stockItem = Specs.CheckProperty(it => it.Description, DESCRIPTION)
                .CheckProperty(it => it.LowerLimit, LOWERLIMIT)
                .CheckProperty(it => it.UpperLimit, UPPERLIMIT)
                .CheckProperty(it => it.Name, NAME)
                .CheckComponentList(it=>it.Outposts,Outposts)
                .CheckProperty(it => it.SMSReferenceCode, SMS_REFERENCE_CODE)
                .CheckReference(it => it.StockGroup, StockGroup)
                .VerifyTheMappings();

            Assert.IsNotNull(stockItem);
            Assert.IsInstanceOf<Guid>(stockItem.Id);
            Assert.AreEqual(stockItem.Name, NAME);
            Assert.IsInstanceOf<StockGroup>(stockItem.StockGroup);
            
            session.Delete(stockItem);
            session.Flush();
        }
    }
}
