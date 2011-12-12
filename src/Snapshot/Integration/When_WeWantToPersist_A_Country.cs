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
           [Test]
            public void It_ShouldSuccessfullyPersist_A_Country()
            {

                var country = Specs.CheckProperty(e => e.Name, "Romania").VerifyTheMappings();

                Assert.IsNotNull(country);
                Assert.AreEqual(country.Name, "Romania");

                session.Delete(country);
                session.Flush();


            }
        }
    }

