using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    internal class RawSmsReceivedOverride : IAutoMappingOverride<RawSmsReceived>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<RawSmsReceived> mapping)
        {
            mapping.Map(e => e.OutpostType).CustomType<int>();
        }
    }
}
