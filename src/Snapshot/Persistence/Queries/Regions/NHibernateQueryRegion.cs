using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using NHibernate.Linq;

namespace Persistence.Queries.Countries
{
    public class NHibernateQueryRegion : IQueryRegion
    {
        private IQueryService<Region> _query;
        public NHibernateQueryRegion(IQueryService<Region> query)
        {
            this._query = query;
        }
        public IQueryable<Domain.Region> GetAll()
        {
            return _query.Query().Fetch(it => it.Country);
        }
    }
}
