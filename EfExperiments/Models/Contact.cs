using EfExperiments.Models.Common;

namespace EfExperiments.Models
{
    public class Contact : EntityBase
    {
        public Contact() : base() { }

        public Contact(string description, string value) : this()
        {
            Description = description;
            Value = value;
        }

        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        public virtual Person Person { get; set; }
    }
}
