using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using Domain;
using NUnit.Framework;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_Campaign: GivenAPersistenceSpecification<Campaign>
    {
        private const string CAMPAIGN_NAME = "Campaign1";
        public DateTime StartDate = DateTime.Today;
        public DateTime EndDate = DateTime.Today;
        public DateTime CreationDate = DateTime.Today;
        private const bool OPEN = true;
        private Client CLIENT = new Client { Name = "Minoxsys" };

        [Test]
        public void It_Should_Successfully_Persist_A_Campaign()
        {
            var campaign = Specs.CheckProperty(e => e.Name, CAMPAIGN_NAME)
                .CheckReference(c => c.Client, CLIENT)
                .CheckProperty(c=>c.Open,OPEN)
                .VerifyTheMappings();

            Assert.IsNotNull(campaign);
            Assert.IsInstanceOf<Guid>(campaign.Id);
           

            session.Delete(campaign);
            session.Flush();
        }
    }
}
