using DAL.EF.Configurations;
using DAL.EF.Entities;
using DAL.EF.DbCreating.DataGeneration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DAL.EF.DbCreating
{
    public class SummerDbContext : IdentityDbContext<ApplicationUser>
    {
        public SummerDbContext(DbContextOptions<SummerDbContext> options) 
            : base(options) { }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Companie> Companies { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new WorkerConfiguration());
            modelBuilder.ApplyConfiguration(new CompanieConfiguration());
            modelBuilder.ApplyConfiguration(new VacancyConfiguration());
            modelBuilder.ApplyConfiguration(new ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new AgreementConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityTypeConfiguration());
        }

        /// <summary>
        /// Ініціалізує базу даних тестовими даними
        /// </summary>
        public void SeedData()
        {
            try
            {
                Console.WriteLine("Генерація ActivityTypes...");
                var activityTypes = ActivityTypeGeneration.Generate(this);
                Console.WriteLine($"Згенеровано {activityTypes.Count} ActivityTypes");

                Console.WriteLine("Генерація Companies...");
                var companies = CompanieGeneration.Generate(this, activityTypes);
                Console.WriteLine($"Згенеровано {companies.Count} Companies");

                Console.WriteLine("Генерація Workers...");
                var workers = WorkerGeneration.Generate(this, activityTypes);
                Console.WriteLine($"Згенеровано {workers.Count} Workers");

                Console.WriteLine("Генерація Vacancies...");
                var vacancies = VacancyGeneration.Generate(this, companies);
                Console.WriteLine($"Згенеровано {vacancies.Count} Vacancies");

                Console.WriteLine("Генерація Responses...");
                var responses = ResponseGeneration.Generate(this, workers, vacancies);
                Console.WriteLine($"Згенеровано {responses.Count} Responses");

                Console.WriteLine("Генерація Agreements...");
                var agreements = AgreementGeneration.Generate(this, workers, companies);
                Console.WriteLine($"Згенеровано {agreements.Count} Agreements");

                Console.WriteLine("Ініціалізація даних завершена успішно");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка в SeedData: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
