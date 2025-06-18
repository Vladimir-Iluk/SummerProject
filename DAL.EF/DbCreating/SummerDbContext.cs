using DAL.EF.Configurations;
using DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.DbCreating
{
    public class SummerDbContext : DbContext
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
            modelBuilder.ApplyConfiguration(new WorkerConfiguration());
            modelBuilder.ApplyConfiguration(new CompanieConfiguration());
            modelBuilder.ApplyConfiguration(new VacancyConfiguration());
            modelBuilder.ApplyConfiguration(new ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new AgreementConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityTypeConfiguration());
        }
    }
}
