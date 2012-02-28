using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using Domain;
using NUnit.Framework;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_Campaign : GivenAPersistenceSpecification<Campaign>
    {
        private const string CAMPAIGN_NAME = "Campaign1";
        public DateTime StartDate = DateTime.UtcNow;
        public DateTime EndDate = DateTime.UtcNow.AddDays(2);
        public DateTime CreationDate = DateTime.UtcNow;
        private const bool OPEN = true;
        private Client CLIENT = new Client { Name = "Minoxsys" };
      

        [Test]
        public void It_Should_Successfully_Persist_A_Campaign()
        {
            var s = Convert.ToBase64String(Encoding.UTF8.GetBytes("ana are mere"));
            byte[] OPTIONS = Convert.FromBase64String(s);

            var campaign = Specs.CheckProperty(e => e.Name, CAMPAIGN_NAME)
                .CheckReference(c => c.Client, CLIENT)
                .CheckProperty(c => c.Opened, OPEN)
                .CheckProperty(c => c.Options, OPTIONS)
                .VerifyTheMappings();

            Assert.IsNotNull(campaign);
            Assert.IsInstanceOf<Guid>(campaign.Id);


            session.Delete(campaign);
            session.Flush();
        }
    }
}
