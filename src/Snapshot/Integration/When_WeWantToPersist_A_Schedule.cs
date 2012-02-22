using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    class When_WeWantToPersist_A_Schedule: GivenAPersistenceSpecification<Schedule>
    {
        private const string FREQUENCY_TYPE = "Daily";
        private const int FREQUENCY_VALUE = 3;

        [Test]
        public void It_Should_Successfully_Persist_A_Schedule()
        {
            var schedule = Specs
                .CheckProperty(e => e.FrequencyType, FREQUENCY_TYPE)
                .CheckProperty(e => e.FrequencyValue, FREQUENCY_VALUE)
                .VerifyTheMappings();

            Assert.IsNotNull(schedule);
            Assert.IsInstanceOf<Guid>(schedule.Id);

            session.Delete(schedule);
            session.Flush();
        }
    }
}
