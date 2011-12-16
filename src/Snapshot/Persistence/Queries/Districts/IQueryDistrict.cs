using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.Districts
{
    public interface IQueryDistrict
    {
        IQueryable<District> GetAll();
    }
}
