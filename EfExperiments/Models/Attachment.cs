using EfExperiments.Models.Common;

namespace EfExperiments.Models
{
    public class Attachment : EntityBase
    {
        public Attachment() : base() { }

        public Attachment(string url) : this() => Url = url;

        public virtual string Url { get; set; }
        public virtual Document Document { get; set; }
    }
}
