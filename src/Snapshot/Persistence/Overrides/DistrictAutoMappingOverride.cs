using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class DistrictAutoMappingOverride : IAutoMappingOverride<District>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<District> mapping)
        {
            mapping.References(p => p.Region).Not.LazyLoad();
            mapping.References(p => p.Client).Not.LazyLoad().Cascade.SaveUpdate();
        }
    }
}
