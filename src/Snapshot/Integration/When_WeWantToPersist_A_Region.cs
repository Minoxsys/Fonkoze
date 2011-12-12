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
        private Country COUNTRY = new Country { Name = "Romania" };


        [Test]
        public void It_Should_Successfully_Persist_A_Region()
        {
            var region = Specs.CheckProperty(e => e.Name, REGION_NAME)
                .CheckReference(c => c.Country,COUNTRY)
                .VerifyTheMappings();

            Assert.IsNotNull(region);
            Assert.IsInstanceOf<Guid>(region.Id);
            Assert.AreEqual(region.Name, REGION_NAME);
            Assert.IsInstanceOf<Country>(region.Country);
            Assert.AreEqual(region.Country.Name, COUNTRY.Name);

            session.Delete(region);
            session.Flush();
        }
    }
}
