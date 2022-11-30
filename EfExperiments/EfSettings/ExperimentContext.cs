using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace EfExperiments.EfSettings
{
    public class ExperimentContext : DbContext
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=EfExperiments;Trusted_Connection=True;MultipleActiveResultSets=true;Max Pool Size=200;";

        //public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString)
                .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
                ;

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Person>().HasKey(a => a.Id);
            //modelBuilder.Entity<Person>().Property(a => a.Id).ValueGeneratedOnAdd();

            modelBuilder.HasSequence<int>("PersonRecordNumber").StartsAt(50000).IncrementsBy(1);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
