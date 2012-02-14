using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Persistence.Queries.Roles
{
    public interface IQueryRole
    {
        IQueryable<Role> GetPermissions();
    }
}
