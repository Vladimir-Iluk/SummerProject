using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class ResponseGeneration
    {
        public static List<Response> Generate(SummerDbContext context, List<Worker> workers, List<Vacancy> vacancies)
        {
            if (context.Responses.Any())
                return context.Responses.ToList();

            var faker = new Faker<Response>("uk")
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.SentAt, f => f.Date.Past(1)) 
                .RuleFor(r => r.Status, f => f.PickRandom<ResponseStatus>())
                .RuleFor(r => r.WorkerId, f => f.PickRandom(workers).Id)
                .RuleFor(r => r.VacancyId, f => f.PickRandom(vacancies).Id);

            var responses = faker.Generate(40);
            context.Responses.AddRange(responses);
            context.SaveChanges();

            return responses;
        }
    }
}
