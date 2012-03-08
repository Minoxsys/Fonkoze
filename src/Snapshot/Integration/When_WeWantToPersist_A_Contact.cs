using System;
using System.Linq;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Domain;

namespace IntegrationTests
{
    [TestFixture]

    class When_WeWantToPersist_A_Contact : GivenAPersistenceSpecification<Contact>
    {
       readonly string OUTPOST_ID = "D60C61CA-450D-40F1-9D81-9FB80099A245";
       readonly string CONTACT_DETAIL = "Outpost Test";
       readonly bool CONTACT_MAIN_METHOD = true;

       [Test]
        public void It_ShouldSuccessfullyPersist_An_Outpost()
        {

            var contact = Specs.CheckProperty(e => e.ContactType, Contact.MOBILE_NUMBER_CONTACT_TYPE)
                               .CheckProperty(e => e.ContactDetail, CONTACT_DETAIL)
                               .CheckProperty(e => e.IsMainContact, CONTACT_MAIN_METHOD)
                               .VerifyTheMappings();

            Assert.IsNotNull(contact);
            Assert.IsInstanceOf<Guid>(contact.Id);
            Assert.AreEqual(contact.ContactType, Contact.MOBILE_NUMBER_CONTACT_TYPE);
            Assert.AreEqual(contact.ContactDetail, CONTACT_DETAIL);
            Assert.AreEqual(contact.IsMainContact, CONTACT_MAIN_METHOD);

            session.Delete(contact);
            session.Flush();


        }
    }
}
