using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class WorkerGeneration
    {
        public static List<Worker> Generate(SummerDbContext context, List<ActivityType> activityTypes)
        {
            if (context.Workers.Any())
                return context.Workers.ToList();

            var faker = new Faker<Worker>("uk")
                .RuleFor(w => w.Id, f => Guid.NewGuid())
                .RuleFor(w => w.FirstName, f => f.Name.FirstName())
                .RuleFor(w => w.LastName, f => f.Name.LastName())
                .RuleFor(w => w.MiddleName, f => f.Name.FirstName())
                .RuleFor(w => w.Email, f => f.Internet.Email())
                .RuleFor(w => w.Qualification, f => f.Name.JobType())
                .RuleFor(w => w.ExpectedSalary, f => f.Random.Number(10000, 30000).ToString())
                .RuleFor(w => w.OtherInfo, f => f.Lorem.Sentence())
                .RuleFor(w => w.ActivityTypeId, f => f.PickRandom(activityTypes).Id);

            var workers = faker.Generate(20);
            context.Workers.AddRange(workers);
            context.SaveChanges();

            return workers;
        }
    }
}
