using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using NHibernate.Linq;
using FluentNHibernate.Testing;
using Core.Domain;
using System.Collections;

namespace IntegrationTests
{
    class Comparer : IEqualityComparer
    {

        public bool Equals(object x, object y)
        {
            return x == y;
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
    [TestFixture]
    public class When_WeWantToPersist_A_Region : GivenAPersistenceSpecification<Region>
    {
        private const string REGION_NAME = "Cluj";
        private const string COORDINATES = "22 44'";
        private Country COUNTRY = new Country { Name = "Romania" };

        [Test]
        public void It_Should_Successfully_Persist_A_Region()
        {
            var region = Specs
                .CheckProperty(e => e.Name, REGION_NAME)
                .CheckProperty(c => c.Coordinates, COORDINATES)
                .CheckReference(c => c.Country, COUNTRY)
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
