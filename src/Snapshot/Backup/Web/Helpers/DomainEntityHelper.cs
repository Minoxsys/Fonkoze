using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;

namespace Web.Helpers
{
    public static class DomainEntityHelper
    {
        public static bool Contains(List<DomainEntity> list, Guid id)
        {
            foreach (DomainEntity entity in list)
            {
                if (entity.Id == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}