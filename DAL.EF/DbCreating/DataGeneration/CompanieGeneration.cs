using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class CompanieGeneration
    {
        public static List<Companie> Generate(SummerDbContext context, List<ActivityType> activityTypes)
        {
            if (context.Companies.Any())
                return context.Companies.ToList();

            var faker = new Faker<Companie>("uk")
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.CompanyName, f => f.Company.CompanyName())
                .RuleFor(c => c.EmailCompany, f => f.Internet.Email())
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.ActivityTypeId, f => f.PickRandom(activityTypes).Id);

            var companies = faker.Generate(15);
            context.Companies.AddRange(companies);
            context.SaveChanges();

            return companies;
        }
    }
}
