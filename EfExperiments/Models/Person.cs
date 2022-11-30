using EfExperiments.Models.Common;

namespace EfExperiments.Models
{
    public class Person : EntityBase
    {
        public Person() : base() { }
        
        public Person(string name, decimal salary, int age) : this()
        {
            Name = name;
            Salary = salary;
            Age = age;
        }

        public virtual string Name { get; set; }
        public virtual decimal Salary { get; set; }
        public virtual int Age { get; set; }
        public virtual int RecordNumber { get; set; }
        public virtual List<AccessHistory> AccessHistories { get; set; } = new List<AccessHistory>();
        public virtual List<Document> Documents { get; set; } = new List<Document>();
        public virtual List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
