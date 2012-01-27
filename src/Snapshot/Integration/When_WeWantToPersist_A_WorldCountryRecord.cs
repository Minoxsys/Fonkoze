using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersist_A_WorldCountryRecord : GivenAPersistenceSpecification<WorldCountryRecord>
    {
        [Test]
        public void It_IsPersisted_Succesfully()
        {
            const string COUNTRY_PHONE_PREFIX = "0040";
            const string COUNTRY_ISO_CODE = "RO";
            const string COUNTRY_NAME = "Romania";
            var country = Specs.CheckProperty(p => p.Name, COUNTRY_NAME)
                               .CheckProperty(p => p.ISOCode, COUNTRY_ISO_CODE)
                               .CheckProperty(p => p.PhonePrefix, COUNTRY_PHONE_PREFIX)
                               .VerifyTheMappings();

            session.Delete(country);
            session.Flush();
        }
    }
}
