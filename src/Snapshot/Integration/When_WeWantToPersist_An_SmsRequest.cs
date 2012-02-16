using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using Domain;
using NUnit.Framework;

namespace IntegrationTests
{
    public class When_WeWantToPersist_An_SmsRequest : GivenAPersistenceSpecification<SmsRequest>
    {
        private const string SMS_MESSAGE = "Malaria R0J0#1";
        private const string NUMBER = "1234567890";
        private Guid OUTPOST_ID = Guid.NewGuid();
        private Guid PRODUCT_GROUP_ID = Guid.NewGuid();

        [Test]
        public void It_Should_Successfully_Persist_An_SmsRequest()
        {
            var smsRequest = Specs
                .CheckProperty(e => e.Message, SMS_MESSAGE)
                .CheckProperty(e => e.Number, NUMBER)
                .CheckProperty(e => e.OutpostId, OUTPOST_ID)
                .CheckProperty(e => e.ProductGroupId, PRODUCT_GROUP_ID)
                .VerifyTheMappings();

            Assert.IsNotNull(smsRequest);
            Assert.IsInstanceOf<Guid>(smsRequest.Id);

            session.Delete(smsRequest);
            session.Flush();
        }
    }
}
