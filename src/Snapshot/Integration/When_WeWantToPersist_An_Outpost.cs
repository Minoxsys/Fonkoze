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
       readonly string OUTPOST_DETAIL = "a.b@evozon.com";
       readonly string OUTPOST_METHOD = "e-mail";
       readonly Guid CLIENT_ID =new Guid("BEEC53CE-A73C-4F03-A354-C617F68BC813");
       readonly List<Contact> Phones;
       //Guid OUTPOST_ID = Guid.Empty;

       public Product product1 = new Product { Name = "product1" };
       public Product product2 = new Product { Name = "product2" };

       private IList<Product> list = new List<Product>();

       [Test]
       public void It_ShouldSuccessfullyPersist_An_Outpost()
       {

           list.Add(product1);
           list.Add(product2);

           var outpost = Specs
                    .CheckProperty(e => e.Name, OUTPOST_NAME)
                    .CheckProperty(e => e.OutpostType, OUTPOST_TYPE)
                    .CheckProperty(e => e.DetailMethod, OUTPOST_DETAIL)
                    .CheckList(e => e.Products,list)
                    //.CheckProperty(e => e.Client, OUTPOST_METHOD)

                    .VerifyTheMappings();

            Assert.IsNotNull(outpost);
            Assert.IsInstanceOf<Guid>(outpost.Id);
            Assert.AreEqual(outpost.Name, OUTPOST_NAME);
            Assert.AreEqual(outpost.OutpostType, OUTPOST_TYPE);
            Assert.AreEqual(outpost.DetailMethod, OUTPOST_DETAIL);
            //Assert.AreEqual(outpost.Client.Id, CLIENT_ID);
            Assert.AreEqual(outpost.Products.Count, 2);

            session.Delete(outpost);
            session.Flush();


       }

       [Test]
       public void It_ShouldSuccessfullyPersist_An_Outpost_WithOnePhone()
       {

           //var client = Specs.CheckProperty(e => e.Name, "Alin Stan").VerifyTheMappings();

           //Assert.IsNotNull(client);
           //Assert.AreEqual(client.Name, "Alin Stan");

           var outpost = Specs
                    .CheckProperty(e => e.Name, OUTPOST_NAME)
                    .CheckProperty(e => e.OutpostType, OUTPOST_TYPE)
                    .CheckProperty(e => e.DetailMethod, OUTPOST_DETAIL)
                    //.CheckProperty(e => e.Client.Id, CLIENT_ID)

                    .VerifyTheMappings();

           var phone = new Contact
           {
               ContactDetail = "07888888"
           };
           var phone1 = new Contact
           {
               ContactDetail = "0743 955034"
           };

           session.Save(phone);
           session.Save(phone1);
           outpost.AddContact(phone);
           outpost.AddContact(phone1);
           //session.Save(outpost);
           //session.Flush();

        //outpost = (from _outpost in session.Query<Outpost>().FetchMany(o => o.Name == OUTPOST_NAME)
        //            where _outpost.Id == outpost.Id
        //            select _outpost).FirstOrDefault();

           Assert.IsNotNull(outpost.Contacts);

           Assert.IsNotNull(outpost);
           Assert.IsInstanceOf<Guid>(outpost.Id);
           Assert.AreEqual(outpost.Name, OUTPOST_NAME);
           Assert.AreEqual(outpost.OutpostType, OUTPOST_TYPE);
           Assert.AreEqual(outpost.DetailMethod, OUTPOST_DETAIL);
           // Assert.AreEqual(outpost.Client, CLIENT_ID);


           session.Delete(outpost);
           session.Flush();


       }
    }
}
