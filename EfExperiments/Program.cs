using EfExperiments.EfSettings;
using EfExperiments.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

using var context = new ExperimentContext();

SeedData(context);
RunQueryFilterExample(context);
RunQuerySplittingExample(context);

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

#endregion

#region Query Optimization

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
}

void SeedAccessHistory(Person person) 
{
    foreach (var i in GetRange(500))
    {
        var entity = new AccessHistory(DateTime.UtcNow);
        person.AccessHistories.Add(entity);
    }
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

IEnumerable<int> GetRange(int max) => Enumerable.Range(1, max);

#endregion