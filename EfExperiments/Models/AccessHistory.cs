using EfExperiments.Models.Common;

namespace EfExperiments.Models
{
    public class AccessHistory : EntityBase
    {
        public AccessHistory() : base() { }

        public AccessHistory(DateTime dateUtc) : this() => DateUtc = dateUtc;

        public virtual DateTime DateUtc { get; set; }
        public virtual Person Person { get; set; }
    }
}
