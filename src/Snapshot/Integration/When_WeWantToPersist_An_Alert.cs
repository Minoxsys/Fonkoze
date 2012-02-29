using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentNHibernate.Testing;
using Domain;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersist_An_Alert : GivenAPersistenceSpecification<Alert>
    {
        private const string CONTACT = "me@ya.com";
        private const string LOWSTOCKLEVEL = "R - 0";
        private Guid OUTPOSTID = Guid.NewGuid();
        private Guid STOCKLEVELID = Guid.NewGuid();
        private Guid PRODUCTGROUPID = Guid.NewGuid();
        private const string OUTPOSTNAME = "me@ya.com";
        private const string PRODUCTGROUPNAME = "me@ya.com";

        [Test]
        public void It_Should_Successfully_Persist_An_Alert()
        {
            var alert = Specs
                .CheckProperty(e => e.Contact, CONTACT)
                .CheckProperty(c => c.LowLevelStock, LOWSTOCKLEVEL)
                .CheckProperty(c => c.OutpostId, OUTPOSTID)
                .CheckProperty(c => c.OutpostName, OUTPOSTNAME)
                .CheckProperty(c => c.OutpostStockLevelId, STOCKLEVELID)
                .CheckProperty(c => c.ProductGroupId, PRODUCTGROUPID)
                .CheckProperty(c => c.ProductGroupName, PRODUCTGROUPNAME)
                .VerifyTheMappings();

            Assert.IsNotNull(alert);
            Assert.IsInstanceOf<Guid>(alert.Id);
            Assert.AreEqual(alert.Contact, CONTACT);
            Assert.AreEqual(alert.LowLevelStock, LOWSTOCKLEVEL);
            Assert.AreEqual(alert.OutpostId, OUTPOSTID);
            Assert.AreEqual(alert.OutpostName, OUTPOSTNAME);
            Assert.AreEqual(alert.OutpostStockLevelId, STOCKLEVELID);
            Assert.AreEqual(alert.ProductGroupId, PRODUCTGROUPID);
            Assert.AreEqual(alert.ProductGroupName, PRODUCTGROUPNAME);
            

            session.Delete(alert);
            session.Flush();
        }
    }
}
