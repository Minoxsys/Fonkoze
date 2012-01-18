using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using NHibernate.Linq;
using FluentNHibernate.Testing;
using Core.Domain;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersist_A_Region :GivenAPersistenceSpecification<Region>
    {
        private const string REGION_NAME = "Cluj";
        private const string COORDINATES = "22 44'";
        private Country COUNTRY = new Country { Name = "Romania" };
        private Client CLIENT = new Client { Name = "minoxsys" };


        [Test]
        public void It_Should_Successfully_Persist_A_Region()
        {
            var region = Specs.CheckProperty(e => e.Name, REGION_NAME)
                .CheckReference(c => c.Country, COUNTRY)
                .CheckReference(c => c.Client, CLIENT)
                .CheckProperty(c => c.Coordinates, COORDINATES)
                .VerifyTheMappings();

            Assert.IsNotNull(region);
            Assert.IsInstanceOf<Guid>(region.Id);
            Assert.AreEqual(region.Name, REGION_NAME);
            Assert.IsInstanceOf<Country>(region.Country);
            Assert.AreEqual(region.Country.Name, COUNTRY.Name);

            session.Delete(region);
            session.Flush();
        }
       
        //[Test]
        //public void it_should_add_many_countries()
        //{
           
        //    for (int i = 0; i < 2; i++)
        //    {

        //        var region = Specs.CheckProperty(e => e.Name, REGION_NAME + i)
        //           .CheckReference(c => c.Country, new Country {                       
        //               Name = "Country " + i
        //           })
        //           .VerifyTheMappings();

        //    }
        //    session.Flush();
        //    session.Dispose();

        //    session = _sessionFactory.CreateSession();

        //    var regions = (from region in session.Query<Region>().Fetch(x => x.Country)
        //                   select region).ToList();
        //   // var regions = session.Query<Region>().ToList();

        //    for (int i = 0; i <regions.Count; i++)
        //    {
        //        var region = regions[i];
        //        Console.WriteLine(region.Name);
        //        Console.WriteLine(region.Country.Name);
        //    }

        //    for (int i = 0; i < regions.Count; i++)
        //    {
        //        session.Delete(regions[i]);
        //    }
          
        //    session.Flush();
        //}
        }
}
