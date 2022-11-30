namespace EfExperiments.Models.Common
{
    public abstract class EntityBase
    {
        public virtual Guid Id { get; set; }
        public virtual bool Active { get; set; } = true;
    }
}
