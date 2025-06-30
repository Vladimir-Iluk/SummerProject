using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class AgreementGeneration
    {
        public static List<Agreement> Generate(SummerDbContext context, List<Worker> workers, List<Companie> companies)
        {
            if (context.Agreements.Any())
                return context.Agreements.ToList();

            var faker = new Faker<Agreement>("uk")
                .RuleFor(a => a.Id, f => Guid.NewGuid())
                .RuleFor(a => a.WorkerId, f => f.PickRandom(workers).Id)
                .RuleFor(a => a.CompanieId, f => f.PickRandom(companies).Id)
                .RuleFor(a => a.Position, f => f.Name.JobTitle())
                .RuleFor(a => a.Commission, f => f.Random.Decimal(100, 1000))
                .RuleFor(a => a.AgreementDate, f => f.Date.Past(1));

            var agreements = faker.Generate(30);
            context.Agreements.AddRange(agreements);
            context.SaveChanges();

            return agreements;
        }
    }
}