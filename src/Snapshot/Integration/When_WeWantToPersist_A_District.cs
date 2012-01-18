using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using Domain;
using NUnit.Framework;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_District : GivenAPersistenceSpecification<District>
    {
        private const string DISTRICT_NAME = "Cluj";
        private Region REGION = new Region { Name = "Romania" };
        private Client CLIENT = new Client { Name = "minoxsys" };


        [Test]
        public void It_Should_Successfully_Persist_A_Region()
        {
            var district = Specs.CheckProperty(e => e.Name, DISTRICT_NAME)
                .CheckReference(c => c.Region, REGION)
                .CheckReference(c => c.Client, CLIENT)
                .VerifyTheMappings();

            Assert.IsNotNull(district);
            Assert.IsInstanceOf<Guid>(district.Id);
            Assert.AreEqual(district.Name, DISTRICT_NAME);
            Assert.IsInstanceOf<Region>(district.Region);
            Assert.AreEqual(district.Region.Name, REGION.Name);

            session.Delete(district);
            session.Flush();
        }
    }
}
