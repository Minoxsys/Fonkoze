using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Core.Domain;
using NHibernate.Linq;

namespace Persistence.Queries.Roles
{
    public class NHibernateQueryRole : IQueryRole
    {
        public IQueryService<Role> queryRole;

        public NHibernateQueryRole(IQueryService<Role> query)
        {
            this.queryRole = query;
        }
        public IQueryable<Role> GetPermissions()
        {
            return queryRole.Query().Fetch(it => it.Functions);
        }
    }
}
