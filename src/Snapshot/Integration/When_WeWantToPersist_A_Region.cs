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
    public class When_WeWantToPersist_A_Region :GivenAPersistenceSpecification<Region>
    {
        private const string REGION_NAME = "Cluj";

        [Test]
        public void It_Should_Successfully_Persist_A_Region()
        {
            var region = Specs.CheckProperty(e => e.Name, REGION_NAME).VerifyTheMappings();

            Assert.IsNotNull(region);
            Assert.IsInstanceOf<Guid>(region.Id);
            Assert.AreEqual(region.Name, REGION_NAME);

            session.Delete(region);
            session.Flush();
        }
    }
}
