using EfExperiments.EfSettings.Map.Common;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfExperiments.EfSettings.Map
{
    internal class DocumentMap : EfMappingBase<Document>
    {
        public override void Configure(EntityTypeBuilder<Document> builder)
        {
            base.Configure(builder);
            builder.HasMany(e => e.Attachments).WithOne(e => e.Document).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
