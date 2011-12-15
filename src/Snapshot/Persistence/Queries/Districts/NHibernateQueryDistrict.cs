using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using NHibernate.Linq;

namespace Persistence.Queries.Districts
{
    public class NHibernateQueryDistrict : IQueryDistrict
    {
        public IQueryService<District> _query;

        public NHibernateQueryDistrict(IQueryService<District> query)
        {
            this._query = query;
        }
        public IQueryable<District> GetAll()
        {
            return _query.Query().Fetch(it => it.Region);
        }
    }
}
