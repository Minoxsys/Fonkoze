using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using NHibernate.Linq;

namespace Persistence.Queries.Outposts
{
    public class NHibernateQueryOutposts : IQueryOutposts
    {
        private IQueryService<Outpost> _query;

        public NHibernateQueryOutposts(IQueryService<Outpost> query)
        {
            this._query = query;
        }

        public IQueryable<Domain.Outpost> GetAllRegions()
        {
            return _query.Query().Fetch(it => it.Region);
        }

        public IQueryable<Domain.Outpost> GetAllDistricts()
        {
            return _query.Query().Fetch(it => it.District);
        }

        public IQueryable<Outpost> GetAllContacts()
        {
            return _query.Query().Fetch(it => it.Contacts);
        }
    }
}
