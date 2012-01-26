using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersist_An_EmailRequest : GivenAPersistenceSpecification<EmailRequest>
    {
        private Guid OUTPOSTID = Guid.NewGuid();
        private Guid PRODUCTGROUPID = Guid.NewGuid();
        private DateTime DATE = DateTime.Today;

        [Test]
        public void It_Should_Successfully_Persist_An_EMAILREQUEST()
        {
            var emailRequest = Specs.CheckProperty(c => c.OutpostId, OUTPOSTID)
                .CheckProperty(c => c.ProductGroupId, PRODUCTGROUPID)
                .CheckProperty(c => c.Date, DATE)
                .VerifyTheMappings();

            Assert.IsNotNull(emailRequest);
            Assert.IsInstanceOf<Guid>(emailRequest.Id);
            Assert.AreEqual(emailRequest.OutpostId, OUTPOSTID);
            Assert.AreEqual(emailRequest.ProductGroupId, PRODUCTGROUPID);
            Assert.AreEqual(emailRequest.Date, DATE);

            session.Delete(emailRequest);
            session.Flush();
        }
    }
}
