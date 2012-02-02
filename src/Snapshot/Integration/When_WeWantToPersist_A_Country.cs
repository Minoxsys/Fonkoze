using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Core.Domain;
using Domain;
using Persistence;
using Persistence.Queries.Employees;
using Persistence.Conventions;
using NHibernate.Linq;

namespace IntegrationTests
{
    [TestFixture]

    class When_WeWantToPersist_A_Country : GivenAPersistenceSpecification<Country>
    {
        readonly string COUNTRY_NAME = "Romania";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Country()
        {

            var country = Specs.CheckProperty(e => e.Name, COUNTRY_NAME).VerifyTheMappings();

            Assert.IsNotNull(country);
            Assert.IsInstanceOf<Guid>(country.Id);
            Assert.AreEqual(country.Name, COUNTRY_NAME);

            session.Delete(country);
            session.Flush();


        }
    }
}

