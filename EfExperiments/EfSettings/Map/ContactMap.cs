using EfExperiments.EfSettings.Map.Common;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfExperiments.EfSettings.Map
{
    internal class ContactMap : EfMappingBase<Contact>
    {
        public override void Configure(EntityTypeBuilder<Contact> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Description).HasMaxLength(100);
            builder.Property(p => p.Value).HasMaxLength(500);

        }
    }
}
