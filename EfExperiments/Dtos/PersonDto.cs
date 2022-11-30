namespace EfExperiments.Dtos
{
    public class PersonDto
    {
        public Guid Id { get; set; }
        public int Age { get; set; }
        public int RecordNumber { get; set; }

        public string Name { get; set; }
        public decimal Salary { get; set; }
        public bool Active { get; set; }

        public AccessHistoryDto[] AccessHistories { get; set; }
    }
}
