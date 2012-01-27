using System;
using System.Linq;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Domain;

namespace IntegrationTests
{
    [TestFixture]

    public class When_WeWantToPersist_A_Client : GivenAPersistenceSpecification<Client>
    {
        [Test]
        public void It_ShouldSuccessfullyPersist_A_Client()
        {

            var client = Specs.CheckProperty(e => e.Name, "Alin Stan").VerifyTheMappings();

            Assert.IsNotNull(client);
            Assert.AreEqual(client.Name, "Alin Stan");

            session.Delete(client);
            session.Flush();


        }
    }
}
