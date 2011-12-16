using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.Regions
{
    public interface IQueryOutposts
    {
        IQueryable<Outpost> GetAllCountries();
        IQueryable<Outpost> GetAllRegions();
        IQueryable<Outpost> GetAllDistricts();
    }
}
