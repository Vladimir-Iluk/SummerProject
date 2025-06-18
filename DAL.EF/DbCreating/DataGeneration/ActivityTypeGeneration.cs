using Bogus;
using DAL.EF.Entities;
using DAL.EF.DbCreating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.EF.DbCreating.DataGeneration
{
    public class ActivityTypeGeneration
    {
        public static List<ActivityType> Generate(SummerDbContext context)
        {
            if (context.ActivityTypes.Any())
                return context.ActivityTypes.ToList();

            var faker = new Faker<ActivityType>("uk")
                .RuleFor(a => a.Id, f => Guid.NewGuid())
                .RuleFor(a => a.ActivityName, f => f.Commerce.Department());

            var activityTypes = faker.Generate(10);
            context.ActivityTypes.AddRange(activityTypes);
            context.SaveChanges();

            return activityTypes;
        }
    }
}
