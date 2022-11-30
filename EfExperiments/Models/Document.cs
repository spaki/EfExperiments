using EfExperiments.Models.Common;

namespace EfExperiments.Models
{
    public class Document : EntityBase
    {
        public Document() : base() { }

        public Document(string name) : this() => Name = name;

        public virtual string Name { get; set; }
        public virtual Person Person { get; set; }
        public virtual List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
