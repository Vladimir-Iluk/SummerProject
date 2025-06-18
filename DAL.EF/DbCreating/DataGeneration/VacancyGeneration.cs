using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class VacancyGeneration
    {
        public static List<Vacancy> Generate(SummerDbContext context, List<Companie> companies)
        {
            if (context.Vacancies.Any())
                return context.Vacancies.ToList();

            var faker = new Faker<Vacancy>("uk")
                .RuleFor(v => v.Id, f => Guid.NewGuid())
                .RuleFor(v => v.Position, f => f.Name.JobTitle())
                .RuleFor(v => v.Description, f => f.Lorem.Paragraph())
                .RuleFor(v => v.Salary, f => f.Random.Decimal(10000, 40000))
                .RuleFor(v => v.CreatedAt, f => f.Date.Past(1))
                .RuleFor(v => v.IsOpen, f => f.Random.Bool(0.8f))
                .RuleFor(v => v.CompanieId, f => f.PickRandom(companies).Id);

            var vacancies = faker.Generate(25);
            context.Vacancies.AddRange(vacancies);
            context.SaveChanges();

            return vacancies;
        }
    }
}
