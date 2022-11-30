using EfExperiments.EfSettings.Map.Common;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfExperiments.EfSettings.Map
{
    internal class AccessHistoryMap : EfMappingBase<AccessHistory>
    {
        public override void Configure(EntityTypeBuilder<AccessHistory> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.DateUtc).HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}
