using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence;
using NUnit.Framework;
using Domain;
using NHibernate.Linq;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections;

namespace IntegrationTests
{
    [TestFixture]
    public class When_Loading_A_Country_It_ShouldAlsoBring_TheRegionNo_InOneQuery 
    {
        private readonly NHibernateSessionFactory sessionFactory = SessionFactory.Instance;
        private NHibernate.ISession _session;

        [Test]
        public void Runs_A_QueryTo_TheDatabase_To_Retrieve_TheNoOfRegions()
        {
            var q1 = _session.QueryOver<Country>()
                             .JoinQueryOver<Region>(c => c.Regions)
                             .Select(
                Projections.Group<Country>(c => c.Id).As("Id"),
                Projections.Count<Country>(c => c.Name).As("RegionNo"))
                             .TransformUsing(Transformers.AliasToEntityMap)
                             .List<IDictionary>()
                             .Select(r => new
            {
                Name = r["Id"],
                RegionNo = r["RegionNo"]
            });
          
            _session.Flush();
        }

        [Test]
        public void Runs_AQuery_With_An_InnerSelect_To_Count_TheNumber_OfRegions()
        {
            var countryWithRegions = (from country in _session.Query<Country>()
                                     join region in _session.Query<Region>()
                                           on country equals region.Country
                                     where country.Client == null
                                     select new
                                     {
                                         country = country,
                                         regionNo = country.Regions.Count()
                                     }).ToList();
            _session.Flush();



        }

        [Test]

        public void foobar()
        {
            var countries = _session.Query<Country>().Where(c=>c.Name == "Romania").Where(c=>c.PhonePrefix== "RO").ToList();
            

            

        }

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            this._session = sessionFactory.CreateSession();

            var country = new Country
            {
                Name = "Romania",
                ISOCode = "RO",
                PhonePrefix = "0040"
            };

            _session.Save(country);

            for (int i = 0; i < 10; i++)
            {
                var region = new Region
                {
                    Name = "Region at index " + i,
                    Country = country
                };
                _session.Save(region);
            }

            country = new Country
            {
                Name = "Netherlands",
                ISOCode = "NL",
                PhonePrefix = "0026"
            };

            _session.Save(country);
            for (int i = 10; i < 30; i++)
            {
                var region = new Region
                {
                    Name = "Region at index " + i,
                    Country = country
                };
                _session.Save(region);
            }

            _session.Flush();
        }

        [TestFixtureTearDown]
        public void AfterAll()
        {
            var regions = _session.Query<Region>().ToList();
            var countries = _session.Query<Country>().ToList();

            foreach (var item in regions)
            {
                _session.Delete(item);
            }

            foreach (var item in countries)
            {
                _session.Delete(item);
            }

            _session.Flush();

            _session.Close();
            _session.Dispose();
        }
    }
}