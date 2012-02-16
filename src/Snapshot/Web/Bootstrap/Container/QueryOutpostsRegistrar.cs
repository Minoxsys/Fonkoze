using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Persistence.Queries.Outposts;

namespace Web.Bootstrap.Container
{
    public class QueryOutpostsRegistrar
    {
        public static void Register(ContainerBuilder container)
        {
            container.RegisterType<NHibernateQueryOutposts>().As<IQueryOutposts>();
        }
    }
}