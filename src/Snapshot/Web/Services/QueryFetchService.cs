using System.Collections.Generic;
using Domain;

namespace Web.Services
{
    public interface QueryFetchService
    {
        List<Region> Query();
    }

}