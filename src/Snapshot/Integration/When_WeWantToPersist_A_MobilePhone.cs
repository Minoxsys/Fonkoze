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

    class When_WeWantToPersist_A_MobilePhone : GivenAPersistenceSpecification<Contact>
    {
       readonly string OUTPOST_ID = "D60C61CA-450D-40F1-9D81-9FB80099A245";
       readonly string MOBILE_NUMBER = "Outpost Test";

       [Test]
        public void It_ShouldSuccessfullyPersist_An_Outpost()
        {

            var mobilePhone = Specs.CheckProperty(e => e.Outpost.Id, OUTPOST_ID).VerifyTheMappings();

            Assert.IsNotNull(mobilePhone);
            Assert.IsInstanceOf<Guid>(mobilePhone.Id);
            Assert.AreEqual(mobilePhone.ContactDetail, MOBILE_NUMBER);

            session.Delete(MOBILE_NUMBER);
            session.Flush();


        }
    }
}
