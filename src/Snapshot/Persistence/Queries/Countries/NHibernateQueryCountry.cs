using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using NHibernate.Linq;

namespace Persistence.Queries.Countries
{
    public class NHibernateQueryCountry : IQueryCountry
    {
        private IQueryService<Country> query;

        public NHibernateQueryCountry(IQueryService<Country> query)
        {
            this.query = query;
        }

        //public IQueryable<Domain.Country> GetAll()
        //{
        //    return _query.Query().Fetch(it => it.Country);
        //}
    }
}
