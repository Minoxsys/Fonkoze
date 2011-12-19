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

    class When_WeWantToPersist_An_Outpost : GivenAPersistenceSpecification<Outpost>
    {
       readonly string OUTPOST_NAME = "Outpost Test";
       readonly string OUTPOST_TYPE = "Facility";
       readonly string OUTPOST_EMAIL = "a.b@evozon.com";
       readonly string OUTPOST_MOBILE = "12345678";
       //readonly List<MobilePhone> Phones;
       //Guid OUTPOST_ID = Guid.Empty;

       [Test]
       public void It_ShouldSuccessfullyPersist_An_Outpost()
       {
           var outpost = Specs
                    .CheckProperty(e => e.Name, OUTPOST_NAME)
                    .CheckProperty(e => e.OutpostType, OUTPOST_TYPE)
                    .CheckProperty(e => e.Email, OUTPOST_EMAIL)
                    .CheckProperty(e => e.MainMobileNumber, OUTPOST_MOBILE)

                    .VerifyTheMappings();

            Assert.IsNotNull(outpost);
            Assert.IsInstanceOf<Guid>(outpost.Id);
            Assert.AreEqual(outpost.Name, OUTPOST_NAME);
            Assert.AreEqual(outpost.OutpostType, OUTPOST_TYPE);
            Assert.AreEqual(outpost.Email, OUTPOST_EMAIL);
            Assert.AreEqual(outpost.MainMobileNumber, OUTPOST_MOBILE);


            session.Delete(outpost);
            session.Flush();


       }

       //[Test]
       //public void It_ShouldSuccessfullyPersist_An_Outpost_WithOnePhone()
       //{
         
           
       //    var outpost = Specs
       //             .CheckProperty(e => e.Name, OUTPOST_NAME)
       //             .CheckProperty(e => e.OutpostType, OUTPOST_TYPE)
       //             .CheckProperty(e => e.Email, OUTPOST_EMAIL)
       //             .CheckProperty(e => e.MainMobileNumber, OUTPOST_MOBILE)
                   
       //             .VerifyTheMappings();
       //    var phone = new MobilePhone
       //    {
       //        MobileNumber = "07888888",
       //        //Outpost=outpost
       //    };
       //    session.Save(phone);
       //    //outpost.AddMobilePhone(phone);
       //    //session.Save(outpost);
       //    session.Flush();

       //    outpost = (from _outpost in session.Query<Outpost>().FetchMany(o=>o.MobilePhones)
       //              where _outpost.Id == outpost.Id
       //              select _outpost).FirstOrDefault();

       //    Assert.IsNotNull(outpost.MobilePhones);

       //    Assert.IsNotNull(outpost);
       //    Assert.IsInstanceOf<Guid>(outpost.Id);
       //    Assert.AreEqual(outpost.Name, OUTPOST_NAME);
       //    Assert.AreEqual(outpost.OutpostType, OUTPOST_TYPE);
       //    Assert.AreEqual(outpost.Email, OUTPOST_EMAIL);
       //    Assert.AreEqual(outpost.MainMobileNumber, OUTPOST_MOBILE);


       //    session.Delete(outpost);
       //    session.Flush();


       //}
    }
}
