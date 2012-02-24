using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class CampaignAutoMappingOverride : IAutoMappingOverride<Campaign>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<Campaign> mapping)
        {
            mapping.References(p => p.Client).Not.LazyLoad().Cascade.SaveUpdate();          

        }
    }
}
