using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_RequestReminder : GivenAPersistenceSpecification<RequestReminder>
    {
        private const string PERIOD_TYPE = "Days";
        private const int PERIOD_VALUE = 1;

        [Test]
        public void It_Should_Successfully_Persist_A_RequestReminder()
        {
            var reminder = Specs
                .CheckProperty(e => e.PeriodType, PERIOD_TYPE)
                .CheckProperty(e => e.PeriodValue, PERIOD_VALUE)
                .VerifyTheMappings();

            Assert.IsNotNull(reminder);
            Assert.IsInstanceOf<Guid>(reminder.Id);

            session.Delete(reminder);
            session.Flush();
        }
    }
}
