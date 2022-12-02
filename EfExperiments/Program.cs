using EfExperiments.Dtos;
using EfExperiments.EfSettings;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

using var context = new ExperimentContext();


SeedData(context);

//var person = context.Set<Person>().FirstOrDefault();

RunQueryFilterExample(context);
//RunQuerySplittingExample(context);
//RunBulkUpdateExample(context);
//RunQueryOptimizationExample(context);

#region Query Filters

void RunQueryFilterExample(ExperimentContext context)
{
    var activePeople = ListPeople(context).Length;
    var everybody = ListPeopleIgnoringQueryFilter(context).Length;
    Console.WriteLine($"We have {activePeople} active people among {everybody}.");
}

Person[] ListPeople(ExperimentContext context) => context
        .Set<Person>()
        .Where(e => 
            e.Salary < 1000
            && e.Age  < 100
            //&& e.Active
        )
        .ToArray()
        ;

Person[] ListPeopleIgnoringQueryFilter(ExperimentContext context) => context
        .Set<Person>()
        .Where(e =>
            e.Salary < 1000
            && e.Age < 100
        )
        .IgnoreQueryFilters()
        .ToArray()
        ;

#endregion

#region Query Splitting

void RunQuerySplittingExample(ExperimentContext context)
{
    Stopwatch stopwatch = new Stopwatch();

    stopwatch.Start();
    ListPeopleWithDocumentsDetaisSingleQuery(context);
    stopwatch.Stop();
    var singleQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    stopwatch.Reset();

    stopwatch.Start();
    ListPeopleWithDocumentsDetaisSplittingQuery(context);
    stopwatch.Stop();
    var splittedQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    Console.WriteLine($"Single person query took {singleQueryElapsedSeconds} seconds. Splitted person query took {splittedQueryElapsedSeconds} seconds.");
}

Person[] ListPeopleWithDocumentsDetaisSingleQuery(ExperimentContext context) => context
        .Set<Person>()
        .IgnoreQueryFilters()
        .Include(e => e.Contacts)
        .Include(e => e.Documents)
        .ThenInclude(e => e.Attachments)
        .Where(e => e.Salary < 200)
        .ToArray()
        ;

Person[] ListPeopleWithDocumentsDetaisSplittingQuery(ExperimentContext context) => context
        .Set<Person>()
        .IgnoreQueryFilters()
        .Include(e => e.Contacts)
        .Include(e => e.Documents)
        .ThenInclude(e => e.Attachments)
        .Where(e => e.Salary < 200)
        .AsSplitQuery()
        .ToArray()
        ;

#endregion

#region Bulk Update

void RunBulkUpdateExample(ExperimentContext context)
{
    Stopwatch stopwatch = new Stopwatch();

    stopwatch.Start();
    TraditionalSalaryUpdate(context);
    stopwatch.Stop();
    var singleQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    stopwatch.Reset();

    stopwatch.Start();
    BulkSalaryUpdate(context);
    stopwatch.Stop();
    var splittedQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    Console.WriteLine($"Traditional salary update took {singleQueryElapsedSeconds} seconds. Bulk salary update took {splittedQueryElapsedSeconds} seconds.");
}

void TraditionalSalaryUpdate(ExperimentContext context)
{
    var promoted = context
        .Set<Person>()
        .Where(e => e.Age > 30 && e.Age < 190)
        .ToArray()
        ;

    foreach (var item in promoted)
        item.Salary *= 1.1m;

    context.SaveChanges();
}

void BulkSalaryUpdate(ExperimentContext context)
{
    var promoted = context
        .Set<Person>()
        .Where(e => e.Age > 30 && e.Age < 190)
        .ExecuteUpdate(u => 
            u.SetProperty(
                e => e.Salary,
                e => e.Salary * 1.1m
            )
        )
        ;

    context.SaveChanges();
}

#endregion

#region Query Optimization

void RunQueryOptimizationExample(ExperimentContext context)
{
    Stopwatch stopwatch = new Stopwatch();

    stopwatch.Start();
    ListPersonHeavyQuery(context);
    stopwatch.Stop();
    var singleQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    stopwatch.Reset();

    stopwatch.Start();
    ListPersonLightQuery(context);
    stopwatch.Stop();
    var splittedQueryElapsedSeconds = stopwatch.Elapsed.TotalSeconds;

    Console.WriteLine($"Heavy person query took {singleQueryElapsedSeconds} seconds. Light person query took {splittedQueryElapsedSeconds} seconds.");
}

/// <summary>
/// Get top 2 Person (Name, Salary) 
/// Active
/// With Salary > 2000
/// With more access
/// And access after 30 november 2022
/// </summary>
/// <returns></returns>
PersonDto[] ListPersonHeavyQuery(ExperimentContext context)
{
    var people = context
        .Set<Person>()
        .IgnoreQueryFilters()
        //.Include(e => e.Documents)
        //.ThenInclude(e => e.Attachments)
        .Include(e => e.AccessHistories)
        .Include(e => e.Contacts)
        //.AsNoTracking() // -> -5 seconds
        .ToList()
        .Select(e => new PersonDto
        {
            Id = e.Id,
            Age = e.Age,
            RecordNumber = e.RecordNumber,

            Name = e.Name,
            Salary = e.Salary,
            Active = e.Active,

            AccessHistories = e.AccessHistories.Select(ah => new AccessHistoryDto
            {
                Id = ah.Id,
                DateUtc = ah.DateUtc
            }).ToArray(),
        })
        .ToList()
        .Where(x => x.Salary > 2000 && x.Active)
        .ToList();

    var orderedPeople = people.OrderByDescending(e => e.AccessHistories.Length).ToList().Take(2).ToList();

    List<PersonDto> finalPeople = new List<PersonDto>();

    foreach (var person in orderedPeople)
    {
        List<AccessHistoryDto> accessHistory = new List<AccessHistoryDto>();

        var allAccessHistory = person.AccessHistories;

        foreach (var access in allAccessHistory)
        {
            if (access.DateUtc > new DateTime(2022, 11, 30))
            {
                accessHistory.Add(access);
            }
        }

        person.AccessHistories = accessHistory.ToArray();
        finalPeople.Add(person);
    }

    return finalPeople.ToArray();
}

PersonDto[] ListPersonLightQuery(ExperimentContext context)
{
    var targetDate = new DateTime(2022, 11, 30);

    var result = context
        .Set<Person>()
        .IgnoreQueryFilters()
        .OrderByDescending(x => x.AccessHistories.Count)
        .Where(x => x.Salary > 2000 && x.Active)
        .Include(e => e.AccessHistories.Where(ah => ah.DateUtc > targetDate))
        .Select(e => new PersonDto
        {
            Name = e.Name,
            Salary = e.Salary,
            Active = e.Active,

            AccessHistories = e.AccessHistories.Select(ah => new AccessHistoryDto
            {
                DateUtc = ah.DateUtc
            }).ToArray(),
        })
        .Take(2)
        .ToArray();

    return result;
}

#endregion

#region Seed Data

async void SeedData(ExperimentContext context)
{
    var set = context.Set<Person>();

    var any = set.Any();

    if (any)
        return; // -> db is populated already 

    const int total = 2_000;
    var range = GetRange(total);

    foreach (var i in range)
    {
        Console.WriteLine($"Generating data: {i}/{total}");

        var person = new Person($"Person {i}", i * 1.1m, i);
        person.Active = i % 2 == 0;

        SeedAccessHistory(person);
        SeedContact(person);
        SeedDocuments(person);

        set.Add(person);
    }

    Console.WriteLine($"Saving data...");

    context.SaveChanges();

    SeedMoreAccessHistory(context);
}

void SeedContact(Person person)
{
    foreach (var i in GetRange(10))
    {
        var entity = new Contact($"Contact Description ${i}", $"Contact Value {i}");
        person.Contacts.Add(entity);
    }
}

void SeedDocuments(Person person)
{
    foreach (var i in GetRange(100))
    {
        var entity = new Document($"Document ${i}");
        SeedAttachments(entity);
        person.Documents.Add(entity);
    }
}

void SeedAttachments(Document document)
{
    foreach (var i in GetRange(20))
    {
        var entity = new Attachment($"https://www.docrepo.com/doc{i}.pptx");
        document.Attachments.Add(entity);
    }
}

void SeedAccessHistory(Person person)
{
    foreach (var i in GetRange(500))
    {
        var entity = new AccessHistory(DateTime.UtcNow);
        person.AccessHistories.Add(entity);
    }
}

void SeedMoreAccessHistory(ExperimentContext context)
{ 
    var people = context
        .Set<Person>()
        .IgnoreQueryFilters()
        .Include(e => e.AccessHistories)
        .Where(e => e.Salary > 1600)
        .ToArray();

    var random = new Random();

    Parallel.ForEach(people, person => 
    {
        var quantity = random.Next(2, 20);

        for (int i = 0; i < quantity; i++)
        {
            var days = random.Next(1, 30);
            var addOrRemove = days % 2 == 0 ? 1 : -1;
            var accessDate = new DateTime(2022, 11, 30).AddDays(days * addOrRemove);

            person.AccessHistories.Add(new AccessHistory(accessDate));
        }
    });

    context.SaveChangesAsync();
}

IEnumerable<int> GetRange(int max) => Enumerable.Range(1, max);

#endregion