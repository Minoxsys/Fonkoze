using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Core.Domain;
using NHibernate.Linq;
using NHibernate;
using Domain;

namespace Persistence.Queries
{
    public class RegionQueryService
    {
        INHibernateUnitOfWork unitOfWork;

        public RegionQueryService(INHibernateUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<Region> Query()
        {
            var result = (from region in unitOfWork.CurrentSession.Query<Region>().Fetch(x => x.Country) select region).ToList();

            return result;
        }


    }
}
