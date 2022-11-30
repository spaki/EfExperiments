using EfExperiments.EfSettings.Map.Common;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfExperiments.EfSettings.Map
{
    internal class AttachmentMap : EfMappingBase<Attachment>
    {
        public override void Configure(EntityTypeBuilder<Attachment> builder)
        {
            base.Configure(builder);
        }
    }
}
