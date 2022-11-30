using EfExperiments.EfSettings.Map.Common;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfExperiments.EfSettings.Map
{
    internal class PersonMap : EfMappingBase<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

            builder.HasMany(e => e.Contacts).WithOne(e => e.Person).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Documents).WithOne(e => e.Person).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.AccessHistories).WithOne(e => e.Person).OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Salary).HasColumnType("money");

            builder.HasIndex(e => e.RecordNumber).IsUnique();
            builder.Property(e => e.RecordNumber).HasDefaultValueSql("NEXT VALUE FOR PersonRecordNumber");

            builder.HasQueryFilter(e => e.Active);
        }
    }
}
